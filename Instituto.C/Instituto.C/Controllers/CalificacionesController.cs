using Instituto.C.Data;
using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instituto.C.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly InstitutoDb _context;

        public CalificacionesController(InstitutoDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var calificaciones = _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Profesor)
                .Include(c => c.MateriaCursada)
                .ThenInclude(mc => mc.Materia);
            return View(await calificaciones.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Profesor)
                .Include(c => c.MateriaCursada)
                .ThenInclude(mc => mc.Materia)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (calificacion == null) return NotFound();

            return View(calificacion);
        }


        [Authorize(Roles = "ProfesorRol")]
        public IActionResult Create()
        {
            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));

            ViewBag.AlumnoId = new SelectList(_context.Alumnos.Select(a => new
            {
                a.Id,
                Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
            }), "Id", "Nombre");

            ViewBag.MateriaCursadaId = new SelectList(_context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.Materia.CodigoMateria + " - " + mc.CodigoCursada + " (" + mc.Anio + ")"
                }), "Id", "Nombre");

            ViewBag.ProfesorId = new SelectList(_context.Profesores.Select(p => new
            {
                p.Id,
                Nombre = p.Nombre + " " + p.Apellido
            }), "Id", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> Create([Bind("Fecha,Nota,ProfesorId,AlumnoId,MateriaCursadaId")] Calificacion calificacion)
        {
            var cursada = await _context.MateriasCursadas.FindAsync(calificacion.MateriaCursadaId);
            if (cursada == null)
            {
                ModelState.AddModelError("", "La materia cursada no existe.");
                return View(calificacion);
            }

            if (!cursada.Activo)
            {
                ModelState.AddModelError("", "No se puede calificar una materia que ya finalizó.");
                return View(calificacion);
            }

            if (calificacion.Fecha == DateTime.MinValue)
                calificacion.Fecha = DateTime.Now;

            // Validar si el alumno está inscripto
            bool estaInscripto = await _context.Inscripciones.AnyAsync(i =>
                i.AlumnoId == calificacion.AlumnoId &&
                i.MateriaCursadaId == calificacion.MateriaCursadaId);

            if (!estaInscripto)
                ModelState.AddModelError("", "El alumno no está inscripto en esa materia cursada.");

            // Validar que el profesor sea el titular de la cursada
            var userName = User.Identity.Name;
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == userName);
            if (profesor == null || cursada.ProfesorId != profesor.Id)
            {
                ModelState.AddModelError("", "No podés calificar esta materia porque no sos el profesor asignado.");
            }

            // Si todo ok, guardar
            if (ModelState.IsValid)
            {
                _context.Add(calificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si hubo errores, recargar combos
            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));

            ViewBag.AlumnoId = new SelectList(_context.Alumnos.Select(a => new
            {
                a.Id,
                Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
            }), "Id", "Nombre", calificacion.AlumnoId);

            ViewBag.MateriaCursadaId = new SelectList(_context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.Materia.CodigoMateria + " - " + mc.CodigoCursada + " (" + mc.Anio + ")"
                }), "Id", "Nombre", calificacion.MateriaCursadaId);

            ViewBag.ProfesorId = new SelectList(_context.Profesores.Select(p => new
            {
                p.Id,
                Nombre = p.Nombre + " " + p.Apellido
            }), "Id", "Nombre", calificacion.ProfesorId);

            return View(calificacion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones.FindAsync(id);
            if (calificacion == null) return NotFound();

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);

            ViewBag.AlumnoId = new SelectList(_context.Alumnos.Select(a => new
            {
                a.Id,
                Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
            }), "Id", "Nombre", calificacion.AlumnoId);

            ViewBag.MateriaCursadaId = new SelectList(_context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.Materia.CodigoMateria + " - " + mc.CodigoCursada + " (" + mc.Anio + ")"
                }), "Id", "Nombre", calificacion.MateriaCursadaId);

            ViewBag.ProfesorId = new SelectList(_context.Profesores.Select(p => new
            {
                p.Id,
                Nombre = p.Nombre + " " + p.Apellido
            }), "Id", "Nombre", calificacion.ProfesorId);

            return View(calificacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Nota,ProfesorId,AlumnoId,MateriaCursadaId")] Calificacion calificacion)
        {
            if (id != calificacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalificacionExists(calificacion.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // En caso de error volver a cargar combos
            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);

            ViewBag.AlumnoId = new SelectList(_context.Alumnos.Select(a => new
            {
                a.Id,
                Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
            }), "Id", "Nombre", calificacion.AlumnoId);

            ViewBag.MateriaCursadaId = new SelectList(_context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.Materia.CodigoMateria + " - " + mc.CodigoCursada + " (" + mc.Anio + ")"
                }), "Id", "Nombre", calificacion.MateriaCursadaId);

            ViewBag.ProfesorId = new SelectList(_context.Profesores.Select(p => new
            {
                p.Id,
                Nombre = p.Nombre + " " + p.Apellido
            }), "Id", "Nombre", calificacion.ProfesorId);

            return View(calificacion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Profesor)
                .Include(c => c.MateriaCursada)
                .ThenInclude(mc => mc.Materia)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (calificacion == null) return NotFound();

            return View(calificacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calificacion = await _context.Calificaciones.FindAsync(id);
            if (calificacion != null)
            {
                _context.Calificaciones.Remove(calificacion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CalificacionExists(int id)
        {
            return _context.Calificaciones.Any(e => e.Id == id);
        }

        [Authorize(Roles = "AlumnoRol")]
        public async Task<IActionResult> MiPromedio()
        {
            var userName = User.Identity.Name;

            var alumno = await _context.Alumnos
                .Include(a => a.Calificaciones)
                .FirstOrDefaultAsync(a => a.UserName == userName);

            if (alumno == null)
            {
                return NotFound("Alumno no encontrado.");
            }

            var notas = alumno.Calificaciones
                .Where(c => NotasHelper.EsNotaValidaParaPromedio(c.Nota))
                .Select(c => c.Nota);

            var promedio = NotasHelper.CalcularPromedio(notas);

            ViewBag.AlumnoNombre = alumno.Nombre + " " + alumno.Apellido;
            ViewBag.Promedio = promedio;

            return View();
        }
    }
}
