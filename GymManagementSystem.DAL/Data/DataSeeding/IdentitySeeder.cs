using GymManagementSystem.DAL.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.DataSeeding
{
    public class IdentitySeeder
    {

        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger,CancellationToken cancellationToken = default!)
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    FirstName = "Nora",
                    LastName = "Nagdi",
                    Email = "superadmin@gmail.com",
                    PasswordHash = "P@SsW0rd"
                },
                new ApplicationUser
                {   
                    FirstName = "Alaa",
                    LastName = "Nagdi",
                    Email = "admin@gmail.com",
                    PasswordHash = "P@SsW0rd"
                }
            };
            var roles = new List<string> { "SuperAdmin", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Error creating role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

        }
    }
}
