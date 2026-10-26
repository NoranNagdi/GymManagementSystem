using GymManagementSystem.DAL.Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.DataSeeding
{
    public static class IdentitySeeder
    {

        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger, CancellationToken cancellationToken = default!)
        {
            try
            {
                var rolesExist = await roleManager.Roles.AnyAsync(cancellationToken);
                var usersExist = await userManager.Users.AnyAsync(cancellationToken);

                if (rolesExist && usersExist) return;

                if (!rolesExist)
                {
                    var roles = new List<IdentityRole>
                {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
                };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            var roleResult = await roleManager.CreateAsync(role);
                            if (roleResult.Succeeded)
                            {
                                logger.LogInformation($"Role '{role.Name}' created successfully.");
                            }
                            else
                            {
                                logger.LogError($"Error creating role '{role.Name}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                            }
                        }
                    }
                }
                if (!usersExist)
                {
                    var superAdmin = new ApplicationUser
                    {
                        FirstName = "Nora",
                        LastName = "Nagdi",
                        UserName = "NoraNagdi",
                        Email = "superadmin@gmail.com",
                        PhoneNumber = "01123456789"
                    };
                    await userManager.CreateAsync(superAdmin, "P@SsW0rd");
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                    var admin = new ApplicationUser
                    {
                        FirstName = "Alaa",
                        LastName = "Nagdi",
                        UserName = "AlaaNagdi",
                        Email = "admin@gmail.com",
                        PhoneNumber = "01223456789"
                    };

                    await userManager.CreateAsync(admin, "P@SsW0rd");
                    await userManager.AddToRoleAsync(admin, "admin");

                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return;
            }
        }
    }
}

