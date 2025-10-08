using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MediaAppFeladat.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaAppFeladat.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
