using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.DataSeeding
{
    public static class GymDataSeeder
    {
        public static async Task SeedDataAsync(ILogger logger, string folderPath, GymDbContext dbContext, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!dbContext.Plans.Any())
                {
                    var plans = await ReadDataFromJson<Plan>(folderPath, "plans.json");
                    dbContext.Plans.AddRange(plans);
                    if (dbContext.ChangeTracker.HasChanges())
                    {
                        await dbContext.SaveChangesAsync(cancellationToken);
                        logger.LogInformation($"Plans data seeded successfully with Count: {plans.Count}");
                    }
                    else
                    {
                        logger.LogInformation("No changes detected in Plans data. Skipping seeding.");
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while seeding data: {ex.Message}");
                throw;
            }
        }
        private static async Task<List<T>> ReadDataFromJson<T>(string folderPath, string fileName)
        {
            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file '{fileName}' was not found in the folder '{folderPath}'.");
            }
            var jsonData = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<T>>(jsonData, options) ?? [];
        }

    }
}
