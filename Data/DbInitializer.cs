using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Data.Entitites;

namespace Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);

        if (!context.Clients.Any())
        {
            context.Clients.AddRange(
                new ClientEntity
                {
                    ClientName = "Default Client",
                    Email = "Default@domain.com",
                    PhoneNumber = "1234567890",
                }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Statuses.Any())
        {
            context.Statuses.AddRange(
                new StatusEntity
                {
                    StatusName = "Started"
                },
                new StatusEntity
                {
                    StatusName = "Completed"
                }
            );
            await context.SaveChangesAsync();
        }

        if (!context.NotificationTargets.Any())
        {
            context.NotificationTargets.AddRange(
                new NotificationTargetEntity
                {
                    TargetName = "Admin"
                },
                new NotificationTargetEntity
                {
                    TargetName = "User"
                }
            );
            await context.SaveChangesAsync();
        }

        if (!context.NotificationTypes.Any())
        {
            context.NotificationTypes.AddRange(
                new NotificationTypeEntity
                {
                    TypeName = "Admin"
                },
                new NotificationTypeEntity
                {
                    TypeName = "User"
                }
            );
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (await roleManager.RoleExistsAsync("Admin") == false)
        {
            var adminRole = new IdentityRole("Admin")
            {
                Id = "1"
            };
            await roleManager.CreateAsync(adminRole);
        }

        if (await roleManager.RoleExistsAsync("User") == false)
        {
            var userRole = new IdentityRole("User")
            {
                Id = "2"
            };
            await roleManager.CreateAsync(userRole);
        }
    }
}
