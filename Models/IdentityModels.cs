using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VSaver.Web.Models.Entities;
using VSaver.Web.Models.Models;

namespace VSaver.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().Property(p => p.Interest).HasPrecision(12, 10);
           
            modelBuilder.Entity<Transaction>()
                .HasRequired(a => a.Agent)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Transaction>()
                .HasRequired(a => a.Customer)
                .WithMany()
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    } 
}