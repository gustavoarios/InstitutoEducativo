using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Instituto.C.Data;
using Instituto.C.Models;

namespace Instituto.C.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly InstitutoDb _context;

        public CalificacionesController(InstitutoDb context)
        {
            _context = context;
        }

        // GET: Calificaciones
        public async Task<IActionResult> Index()
        {
            var institutoDb = _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Inscripcion)
                .Include(c => c.Profesor);
            return View(await institutoDb.ToListAsync());
        }

        // GET: Calificaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Inscripcion)
                .Include(c => c.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (calificacion == null) return NotFound();

            return View(calificacion);
        }

        // GET: Calificaciones/Create
        public IActionResult Create()
        {
            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre");

            ViewData["ProfesorId"] = new SelectList(
                _context.Profesores.Select(p => new
                {
                    p.Id,
                    Nombre = p.Nombre + " " + p.Apellido
                }),
                "Id", "Nombre");

            ViewData["InscripcionId"] = new SelectList(
                _context.Inscripciones.Include(i => i.Alumno).Include(i => i.MateriaCursada)
                .Select(i => new
                {
                    i.Id,
                    Descripcion = i.Alumno.Nombre + " " + i.Alumno.Apellido + " - " + i.MateriaCursada.CodigoCursada
                }),
                "Id", "Descripcion");

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));
            return View();
        }

        // POST: Calificaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Nota,ProfesorId,AlumnoId,InscripcionId")] Calificacion calificacion)
        {
            if (calificacion.Fecha == DateTime.MinValue)
            {
                calificacion.Fecha = DateTime.Now;
            }
            if (ModelState.IsValid)
            {
                _context.Add(calificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", calificacion.AlumnoId);

            ViewData["ProfesorId"] = new SelectList(
                _context.Profesores.Select(p => new
                {
                    p.Id,
                    Nombre = p.Nombre + " " + p.Apellido
                }),
                "Id", "Nombre", calificacion.ProfesorId);

            ViewData["InscripcionId"] = new SelectList(
                _context.Inscripciones.Include(i => i.Alumno).Include(i => i.MateriaCursada)
                .Select(i => new
                {
                    i.Id,
                    Descripcion = i.Alumno.Nombre + " " + i.Alumno.Apellido + " - " + i.MateriaCursada.CodigoCursada
                }),
                "Id", "Descripcion", calificacion.InscripcionId);

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));
            return View(calificacion);
        }

        // GET: Calificaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones.FindAsync(id);
            if (calificacion == null) return NotFound();

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", calificacion.AlumnoId);

            ViewData["ProfesorId"] = new SelectList(
                _context.Profesores.Select(p => new
                {
                    p.Id,
                    Nombre = p.Nombre + " " + p.Apellido
                }),
                "Id", "Nombre", calificacion.ProfesorId);

            ViewData["InscripcionId"] = new SelectList(
                _context.Inscripciones.Include(i => i.Alumno).Include(i => i.MateriaCursada)
                .Select(i => new
                {
                    i.Id,
                    Descripcion = i.Alumno.Nombre + " " + i.Alumno.Apellido + " - " + i.MateriaCursada.CodigoCursada
                }),
                "Id", "Descripcion", calificacion.InscripcionId);

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);
            return View(calificacion);
        }

        // POST: Calificaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Nota,ProfesorId,AlumnoId,InscripcionId")] Calificacion calificacion)
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
                    if (!CalificacionExists(calificacion.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", calificacion.AlumnoId);

            ViewData["ProfesorId"] = new SelectList(
                _context.Profesores.Select(p => new
                {
                    p.Id,
                    Nombre = p.Nombre + " " + p.Apellido
                }),
                "Id", "Nombre", calificacion.ProfesorId);

            ViewData["InscripcionId"] = new SelectList(
                _context.Inscripciones.Include(i => i.Alumno).Include(i => i.MateriaCursada)
                .Select(i => new
                {
                    i.Id,
                    Descripcion = i.Alumno.Nombre + " " + i.Alumno.Apellido + " - " + i.MateriaCursada.CodigoCursada
                }),
                "Id", "Descripcion", calificacion.InscripcionId);

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);
            return View(calificacion);
        }

        // GET: Calificaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var calificacion = await _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Inscripcion)
                .Include(c => c.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (calificacion == null) return NotFound();

            return View(calificacion);
        }

        // POST: Calificaciones/Delete/5
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
    }
}
