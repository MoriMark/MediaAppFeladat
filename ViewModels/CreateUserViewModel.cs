using System.ComponentModel.DataAnnotations;

namespace MediaAppFeladat.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
