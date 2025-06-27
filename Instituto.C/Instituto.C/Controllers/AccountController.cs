using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Instituto.C.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        //POR INYECCION DE DEPENCIA PIDO UNA BASE DE DATOS, CON EL SERVICIO LA TENGO DISPONIBLE


        private readonly InstitutoDb _context;
        private readonly UserManager<Persona> _userManager;
        private readonly SignInManager<Persona> _signInManager;
        private readonly RoleManager<Rol> _roleManager;

       


        public AccountController(InstitutoDb context, UserManager<Persona> userManager, SignInManager<Persona> signInManager, RoleManager<Rol> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }


        [AllowAnonymous] //para que no se necesite autenticacion para ver el formulario de inicio de sesion
        public IActionResult IniciarSesion(string returnurl)//este muestra el formulario
        {

            TempData["returnurl"] = returnurl; //enviamos informacion entre los action metods, por eso usamos tempData


            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Login model)//metodo para procesar los datos del formulario
        {
            //lo primero es validar el modelstate


            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.Recordarme, false);

                if (resultado.Succeeded)
                {
                    string returnurl = TempData["returnurl"] as string;

                    if (returnurl is not null)
                    {
                        return Redirect(returnurl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                //si no fue exitoso le mando mensaje

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");

            }
            return View(model);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            //devuleve task y equivale a un void, al ser asincronico es una tarea y le ponemos el await y async al metodo
            await _signInManager.SignOutAsync();
            //cierro sesión y lo mando al home index
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult Registrar()
        {
            ViewBag.Carreras = new SelectList(_context.Carreras, "Id", "Nombre");
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registrar(Registrar model)
        {
            if (ModelState.IsValid)
            {
                Alumno alumno = new Alumno();
                {
                    alumno.UserName = model.UserName;
                    alumno.Nombre = model.Nombre;
                    alumno.Apellido = model.Apellido;
                    alumno.Email = model.Email;
                    alumno.Direccion = model.Direccion;
                    alumno.DNI = model.DNI;
                    alumno.Telefono = model.Telefono;
                    alumno.CarreraId = model.CarreraId;
                    alumno.Activo = false; //ponemos y seteamos el activo en false, porque el alumno no se activa hasta que un empleado lo de de alta

                }

                var resultado = await _userManager.CreateAsync(alumno, model.Password);

                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(alumno, "AlumnoRol");
                    await _signInManager.SignInAsync(alumno, false);

                    var gestor = new GestorAlumnos();
                    gestor.AsignarNumeroMatricula(alumno, _context);
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                    //si está ok registro
                    //aca se asignan los roles
                    return RedirectToAction("Index", "Home");


                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            //lo repito si hay errores
            ViewBag.Carreras = new SelectList(_context.Carreras, "Id", "Nombre");
            return View();
        }
       
        //pasar este metodo a seeddata
        /*public async Task<IActionResult> CrearRoles()
        {
            if (!await _context.MisRoles.AnyAsync())
            {
                await _roleManager.CreateAsync(new Rol("AlumnoRol"));
                await _roleManager.CreateAsync(new Rol("EmpleadoRol"));
                await _roleManager.CreateAsync(new Rol("ProfesorRol"));
            }
            return RedirectToAction("Index", "Home", new { message = "Roles creados" });
        }*/

        public IActionResult AccesoDenegado()
        {
            return View();
        }

    }
}
