using eTermini.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eTermini.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FieldsEntity> Fields { get; set; }
        public DbSet<MatchesEntity> Matches { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }

}
