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
            //builder.Services.AddControllersWithViews();

            // Add services to the container.
            //builder.Services.AddDbContext<InstitutoDb>(options => options.UseInMemoryDatabase("InstitutoDb"));
            builder.Services.AddDbContext<InstitutoDb>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("InstitutoDb-C")));


            builder.Services.AddIdentity<Persona, Rol>()
                .AddEntityFrameworkStores<InstitutoDb>();


            builder.Services.Configure<IdentityOptions>(opciones =>
            {
                opciones.Password.RequireLowercase = false;
                opciones.Password.RequireNonAlphanumeric = false;
                opciones.Password.RequireUppercase = false;
                opciones.Password.RequireDigit = false;
                opciones.Password.RequiredLength = 5; //Antes era 6, también se puede hacer en AddIdentity.
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

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
