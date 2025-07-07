//using System;
//using System.Linq;
//using Instituto.C.Data;
//using Instituto.C.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace Instituto.C
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            builder.Services.AddDbContext<InstitutoDb>(options =>
//                options.UseSqlServer(builder.Configuration.GetConnectionString("InstitutoDb-C")));

//            builder.Services.AddIdentity<Persona, Rol>()
//                .AddEntityFrameworkStores<InstitutoDb>();

//            builder.Services.Configure<IdentityOptions>(opciones =>
//            {
//                opciones.Password.RequireLowercase = false;
//                opciones.Password.RequireNonAlphanumeric = false;
//                opciones.Password.RequireUppercase = false;
//                opciones.Password.RequireDigit = false;
//                opciones.Password.RequiredLength = 5;
//                opciones.User.RequireUniqueEmail = true;
//            });

//            builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
//                opciones =>
//                {
//                    opciones.LoginPath = "/Account/IniciarSesion";
//                    opciones.AccessDeniedPath = "/Account/AccesoDenegado";
//                    opciones.Cookie.Name = "InstitutoCookie";
//                });

//            builder.Services.AddControllersWithViews();

//            var app = builder.Build();

//            // === SOLO EN DESARROLLO: aplicar migraciones y precargar si base está vacía y ya hay roles ===
//            using (var scope = app.Services.CreateScope())
//            {
//                var context = scope.ServiceProvider.GetRequiredService<InstitutoDb>();
//                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Persona>>();
//                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Rol>>();

//                context.Database.Migrate(); // Aplica migraciones pendientes

//                //Verificamos si existen los roles necesarios
//                bool rolesExisten =
//                    roleManager.RoleExistsAsync("EmpleadoRol").Result &&
//                    roleManager.RoleExistsAsync("ProfesorRol").Result &&
//                    roleManager.RoleExistsAsync("AlumnoRol").Result;

//                // Solo precargar si la base está vacía Y los roles ya existen
//                if (!context.Users.Any() && rolesExisten)
//                {
//                    Console.WriteLine("?? Precargando datos automáticamente...");
//                    SeedData.InitializeAsync(context, userManager).Wait();
//                }
//                else
//                {
//                    Console.WriteLine("?? Datos ya existentes o roles no creados. Precarga automática omitida.");
//                }

//            }

//            // Configuración del pipeline
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            app.UseRouting();

//            app.UseAuthorization();

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            app.Run();
//        }
//    }
//}

using System;
using System.Linq;
using System.Threading.Tasks;
using Instituto.C.Data;
using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Instituto.C
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<InstitutoDb>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("InstitutoDb-C")));

            builder.Services.AddIdentity<Persona, Rol>()
                .AddEntityFrameworkStores<InstitutoDb>();

            builder.Services.Configure<IdentityOptions>(opciones =>
            {
                opciones.Password.RequireLowercase = false;
                opciones.Password.RequireNonAlphanumeric = false;
                opciones.Password.RequireUppercase = false;
                opciones.Password.RequireDigit = false;
                opciones.Password.RequiredLength = 5;
                opciones.User.RequireUniqueEmail = true;
            });

            builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                opciones =>
                {
                    opciones.LoginPath = "/Account/IniciarSesion";
                    opciones.AccessDeniedPath = "/Account/AccesoDenegado";
                    opciones.Cookie.Name = "InstitutoCookie";
                });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // === SOLO EN DESARROLLO: aplicar migraciones y precargar si la base está vacía ===
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<InstitutoDb>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Persona>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Rol>>();

                // ✅ Aplica migraciones solo si hay pendientes (más seguro)
                if (context.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Se detectaron migraciones pendientes. Aplicando...");
                    context.Database.Migrate();
                }
                else
                {
                    Console.WriteLine("No hay migraciones pendientes.");
                }

                // Crear roles si no existen
                await RoleSeeder.CrearRolesAsync(roleManager);

                // Precargar solo si no hay usuarios
                if (!context.Users.Any())
                {
                    Console.WriteLine("Precargando datos automáticamente...");
                    await SeedData.InitializeAsync(context, userManager);
                }
                else
                {
                    Console.WriteLine("Datos ya existentes. Precarga automática omitida.");
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}



