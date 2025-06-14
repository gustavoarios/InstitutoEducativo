using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Instituto.C.Controllers
{


    [Authorize]

    public class AlumnosController : Controller
    {
        private readonly InstitutoDb _context;
        private readonly UserManager<Persona> _userManager;

        public AlumnosController(InstitutoDb context, UserManager<Persona> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }

        // GET: Alumnos
        public async Task<IActionResult> Index()
        {
            var institutoDb = _context.Alumnos.Include(a => a.Carrera);
            return View(await institutoDb.ToListAsync());
        }

        // GET: Alumnos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Carrera)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumnos/Create
        [Authorize(Roles = "EmpleadoRol")]
        public IActionResult Create()
        {
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera");
            return View();
        }

        // POST: Alumnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]

        public async Task<IActionResult> Create([Bind("CarreraId,UserName,Email,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {

                IdentityResult resultado;

                if (User.IsInRole("EmpleadoRol"))
                {
                    resultado = await _userManager.CreateAsync(alumno, "Password1!");
                }
                else
                {
                    resultado = await _userManager.CreateAsync(alumno);
                }




                if (resultado.Succeeded)
                {
                    await _userManager.AddToRoleAsync(alumno, "AlumnoRol"); //asigno el rol de AlumnoRol al usuario recien creado



                    //asigno el número de matrícula
                    var gestor = new GestorAlumnos(); //instancio al gestor para usar el metodo que asigna la matricula, pasandole el alumno recien creado
                    gestor.AsignarNumeroMatricula(alumno, _context); //aparte del alumno, le paso el campo privado de acceso a la base de datos que se usa en AlumnosController

                    //guardo nuevamente para actualizar la matrícula
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();

                }

                return RedirectToAction(nameof(Index));

            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
            return View(alumno);
        }


        // GET: Alumnos/Edit/5

        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            //cuando es alumno, evaluo

            if (User.IsInRole("AlumnoRol"))
            {

                var userid = Int32.Parse(_userManager.GetUserId(User));

                //validamos si es si mismo, sino lo redireccionamos

                if (userid != id)
                {
                    return RedirectToAction("Edit", new { id = userid });

                }
            }

            var alumno = await _context.Alumnos.FindAsync(id);

            if (alumno == null)
            {
                return NotFound();
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
            return View(alumno);
        }

        // POST: Alumnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Edit(int id, [Bind("UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo,CarreraId")] Alumno alumno)
        {
            Alumno alumnoDb;

            if (User.IsInRole("AlumnoRol"))
            {
                var userId = Int32.Parse(_userManager.GetUserId(User));

                // Seguridad: el alumno solo puede editarse a sí mismo
                if (userId != id)
                {
                    return Forbid();
                }

                alumnoDb = await _context.Alumnos.FindAsync(userId);
            }
            else
            {
                alumnoDb = await _context.Alumnos.FindAsync(id);
            }

            if (alumnoDb == null)
            {
                return NotFound();
            }

            // Campos editables por todos


            // Campos solo editables por empleados (no alumnos)
            if (User.IsInRole("EmpleadoRol"))
            {
                alumnoDb.Email = alumno.Email;
                alumnoDb.Telefono = alumno.Telefono;
                alumnoDb.Activo = alumno.Activo;
                alumnoDb.FechaAlta = alumno.FechaAlta;
                alumnoDb.UserName = alumno.UserName;
                alumnoDb.Nombre = alumno.Nombre;
                alumnoDb.Apellido = alumno.Apellido;
                alumnoDb.DNI = alumno.DNI;
                alumnoDb.Direccion = alumno.Direccion;
                alumnoDb.CarreraId = alumno.CarreraId;
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem("Hubo un problema de concurrencia al intentar guardar los cambios.");
            }
        }


        // GET: Alumnos/Delete/5
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .Include(a => a.Carrera)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "EmpleadoRol")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);
            if (alumno != null)
            {
                _context.Alumnos.Remove(alumno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.Id == id);
        }
    }
}
