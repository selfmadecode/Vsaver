namespace VSaver.Web.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using VSaver.Web.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<VSaver.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VSaver.Web.Models.ApplicationDbContext context)
        {
            //Seed Admin

            string[] appRoles = new string[] { "Admin", "PaymentAgent", "AccountAgent" };

            foreach (var role in appRoles)
            {
                if (!context.Roles.Any(r => r.Name == role))
                {
                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var newRole = new IdentityRole
                    {
                        Name = role
                    };

                    roleManager.Create(newRole);
                }
            }
            
            
            //Super Admin
            if (!context.Users.Any(u => u.UserName == "admin@vsaver.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                
                var passwordHasher = new PasswordHasher();

                var admin = new ApplicationUser
                {
                    UserName = "admin@vsaver.com",
                    Email = "admin@vsaver.com",
                    EmailConfirmed = true
                };
                userManager.Create(admin, "Admin@vsaver1_");
                userManager.AddToRole(admin.Id, "Admin");
            }

            //Payment Admin
           /*
            if (!context.Users.Any(u => u.UserName == "paymentagent@vsaver.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var passwordHasher = new PasswordHasher();

                var admin = new ApplicationUser
                {
                    UserName = "paymentagent@vsaver.com",
                    Email = "paymentagent@vsaver.com",
                    EmailConfirmed = true
                };
                userManager.Create(admin, "Admin@vsaver1_");
                userManager.AddToRole(admin.Id, "PaymentAgent");
            }
           */
           /*
            //Account Creation Admin
            if (!context.Users.Any(u => u.UserName == "accountagent@vsaver.com"))
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var passwordHasher = new PasswordHasher();

                var admin = new ApplicationUser
                {
                    UserName = "accountagent@vsaver.com",
                    Email = "accountagent@vsaver.com",
                    EmailConfirmed = true
                };
                userManager.Create(admin, "Admin@vsaver1_");
                userManager.AddToRole(admin.Id, "AccountAgent");
            }
           */







            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }

    }
}
