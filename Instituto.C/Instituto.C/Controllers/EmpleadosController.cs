using Instituto.C.Data;
using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instituto.C.Controllers
{

    [Authorize(Roles = "EmpleadoRol")]
    public class EmpleadosController : Controller
    {
        private readonly InstitutoDb _context;
        private readonly UserManager<Persona> _userManager;

        public EmpleadosController(InstitutoDb context, UserManager<Persona> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {

            var soloEmpleados = await _context.Empleados.Where(e => !(e is Profesor)).ToListAsync(); // Filtra los que NO son profesores

            return View(soloEmpleados);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Legajo,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                empleado.Legajo = GeneradorDeLegajo.GenerarLegajoParaEmpleado(empleado);
                empleado.FechaAlta = DateTime.Now;
                empleado.EmailConfirmed = true;

                var resultado = await _userManager.CreateAsync(empleado, "Password1!"); // contraseña temporal

                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(empleado, "EmpleadoRol");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(empleado);
        }

        // GET: Empleados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Legajo,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Empleado empleado)
        {
            if (id != empleado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var empleadoDb = await _context.Empleados.FindAsync(id);

                if (empleadoDb == null)
                {
                    return NotFound();
                }

                //acualizamos los campos que no requieren UserManager
                empleadoDb.Legajo = empleado.Legajo;
                empleadoDb.FechaAlta = empleado.FechaAlta;
                empleadoDb.Nombre = empleado.Nombre;
                empleadoDb.Apellido = empleado.Apellido;
                empleadoDb.DNI = empleado.DNI;
                empleadoDb.Telefono = empleado.Telefono;
                empleadoDb.Direccion = empleado.Direccion;
                empleadoDb.Activo = empleado.Activo;

                //actualizamos el Email y UserName usando Identity
                empleadoDb.Email = empleado.Email;
                empleadoDb.UserName = empleado.UserName;

                var result = await _userManager.UpdateAsync(empleadoDb);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(empleado);
                }

                try
                {
                    await _context.SaveChangesAsync(); // por si hay otros cambios en el contexto
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(empleado);
        }


        // GET: Empleados/Delete/5
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}

