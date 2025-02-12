using GestionTarea.Data;
using GestionTarea.Models;
using Microsoft.AspNetCore.Identity;

namespace GestionTarea.Services
{
    public class SeedService
    {
        public static async Task SeedDataBase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService <UserManager<Usuario>>();
            var logger = scope.ServiceProvider.GetRequiredService <ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Confirmando la base de datos");
                await context.Database.EnsureCreatedAsync();

                logger.LogInformation("Asignando Roles");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                logger.LogInformation("Creando AdminUser");
                var adminEmail = "admin@admin.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null) 
                {
                    var adminUser = new Usuario
                    {
                        Nombre = "Admin",
                        UserName = adminEmail,
                        NormalizedUserName = adminEmail.ToUpper(),
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Asignando rol de administrador al usuario");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("No se pudo crear el usuario administrador: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Ocurrio un error cargando la base de datos.");
            }
        }
        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded) 
                {
                    throw new Exception($"No se pudo crear el rol '{roleName}': {string.Join(",", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
