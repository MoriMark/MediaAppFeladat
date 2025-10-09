using MediaAppFeladat.Models;
using MediaAppFeladat.Services;
using MediaAppFeladat.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediaAppFeladat.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IEmailService emailService;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "User not found!");
                        return View(model);
                    }

                    var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ChangePassword", "Account", new { email = model.Email, token = resetToken }, Request.Scheme);

                    var subject = "Locked out account information";
                    var body = $"Your Account is Locked. Please Reset your password by clicking here: <a href='{resetLink}'> Reset Password</a>";

                    await emailService.SendEmailAsync(model.Email, subject, body);
                    ModelState.AddModelError("", "The account is locked out");
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult EmailSent()
        {
            return View();
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ChangePassword", "Account", new {email=model.Email,token=resetToken}, Request.Scheme);

            var subject = "Reset Password";
            var body = $"Please Reset your password by clicking here: <a href='{resetLink}'> Reset Password</a>";

            await emailService.SendEmailAsync(model.Email, subject, body);

            return RedirectToAction("EmailSent", "Account");
        }

        [HttpGet]
        public IActionResult ChangePassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            var model = new ChangePasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) 
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            var resetResult = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if ( !resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
    }
}
