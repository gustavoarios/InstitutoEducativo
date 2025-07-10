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

        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Index()
        {
            var institutoDb = _context.Alumnos.Include(a => a.Carrera);
            return View(await institutoDb.ToListAsync());
        }

        // GET: Alumnos/Details/5
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                        .Include(a => a.Carrera)
                        .Include(a => a.Inscripciones.Where(i => i.Activa))
                        .ThenInclude(i => i.MateriaCursada)
                        .ThenInclude(mc => mc.Materia)
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

            var alumno = await _context.Alumnos
                .Include(a => a.Carrera)
                .FirstOrDefaultAsync(a => a.Id == id);

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
            var alumnoDb = await _context.Alumnos
                .Include(a => a.Inscripciones)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alumnoDb == null)
                return NotFound();

            bool cambioCarrera = alumno.CarreraId != alumnoDb.CarreraId;
            bool tieneInscripciones = alumnoDb.Inscripciones.Any();

            if (cambioCarrera && tieneInscripciones)
            {
                ModelState.AddModelError("CarreraId", "No se puede cambiar de carrera porque el alumno ya tiene inscripciones.");
                alumno.CarreraId = alumnoDb.CarreraId;
                ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
                return View(alumno);
            }

            //actualizamos campos personales comunes
            alumnoDb.Telefono = alumno.Telefono;
            alumnoDb.Activo = alumno.Activo;
            alumnoDb.FechaAlta = alumno.FechaAlta;
            alumnoDb.Nombre = alumno.Nombre;
            alumnoDb.Apellido = alumno.Apellido;
            alumnoDb.DNI = alumno.DNI;
            alumnoDb.Direccion = alumno.Direccion;

            //solo cambiamos la carrera si está permitido
            if (!tieneInscripciones && cambioCarrera)
            {
                alumnoDb.CarreraId = alumno.CarreraId;
            }

            // actualizamos UserName y Email usando Identity
            alumnoDb.Email = alumno.Email;
            alumnoDb.UserName = alumno.UserName;

            var result = await _userManager.UpdateAsync(alumnoDb);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
                return View(alumno);
            }

            try
            {
                await _context.SaveChangesAsync(); // si hay otros cambios en contexto además del usuario
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Problem("Hubo un problema de concurrencia al intentar guardar los cambios.");
            }
        }


        // GET: Alumnos/Delete/5
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar

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
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
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

        [HttpPost]
        [Authorize(Roles = "EmpleadoRol")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);

            if (alumno == null)
                return NotFound();

            alumno.Activo = !alumno.Activo;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = $"El alumno fue {(alumno.Activo ? "activado" : "desactivado")} correctamente.";
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al intentar actualizar el estado del alumno.";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.Id == id);
        }
    }
}
