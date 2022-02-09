using Individual_Identity.Core.Domain;
using Microsoft.AspNetCore.Identity;

namespace Individual_Identity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                try
                {
                    var context = sp.GetService<DataContext>();
                    var userManager = sp.GetService<UserManager<User>>();
                    var roleManager = sp.GetService<RoleManager<Role>>();

                    context.Database.EnsureCreated();

                    // Look for any students.
                    if (context.Users.Any())
                    {
                        return;   // DB has been seeded
                    }

                    var roles = new List<Role>
                {
                    new Role{Name = "Admin"},
                };

                    foreach (var role in roles)
                    {
                        roleManager.CreateAsync(role).Wait();
                    }

                    var adminUser = new User
                    {
                        UserName = "Admin"
                    };

                    IdentityResult result = userManager.CreateAsync(adminUser, "@Admin12345").Result;

                    if (result.Succeeded)
                    {
                        var admin = userManager.FindByNameAsync("Admin").Result;
                        userManager.AddToRolesAsync(admin, new[] { "Admin" }).Wait();
                    }

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }
        }
    }
}
