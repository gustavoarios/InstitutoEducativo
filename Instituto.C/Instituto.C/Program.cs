using System;
using System.Linq;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Instituto.C
{
    public class Program
    {
        public static void Main(string[] args)
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

            // === SOLO EN DESARROLLO: aplicar migraciones y precargar si base est� vac�a y ya hay roles ===
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<InstitutoDb>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Persona>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Rol>>();

                context.Database.Migrate(); // Aplica migraciones pendientes

                // Verificamos si existen los roles necesarios
                bool rolesExisten =
                    roleManager.RoleExistsAsync("EmpleadoRol").Result &&
                    roleManager.RoleExistsAsync("ProfesorRol").Result &&
                    roleManager.RoleExistsAsync("AlumnoRol").Result;

                // Solo precargar si la base est� vac�a Y los roles ya existen
                if (!context.Users.Any() && rolesExisten)
                {
                    Console.WriteLine("?? Precargando datos autom�ticamente...");
                    SeedData.InitializeAsync(context, userManager).Wait();
                }
                else
                {
                    Console.WriteLine("?? Datos ya existentes o roles no creados. Precarga autom�tica omitida.");
                }
            }

            // Configuraci�n del pipeline
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

            app.Run();
        }
    }
}


