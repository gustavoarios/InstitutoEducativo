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
    public class InscripcionesController : Controller
    {
        private readonly InstitutoDb _context;

        public InscripcionesController(InstitutoDb context)
        {
            _context = context;
        }

        // GET: Inscripciones
        public async Task<IActionResult> Index()
        {
            var inscripciones = _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada);
            return View(await inscripciones.ToListAsync());
        }

        // GET: Inscripciones/Details
        public async Task<IActionResult> Details(int? alumnoId, int? materiaCursadaId)
        {
            if (alumnoId == null || materiaCursadaId == null)
                return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.MateriaCursadaId == materiaCursadaId);

            if (inscripcion == null)
                return NotFound();

            return View(inscripcion);
        }

        // GET: Inscripciones/Create
        public IActionResult Create()
        {
            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }), "Id", "Nombre");

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas.Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.CodigoCursada
                }), "Id", "Nombre");

            return View();
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoId,MateriaCursadaId,FechaInscripcion,Activa")] Inscripcion inscripcion)
        {
            if (inscripcion.FechaInscripcion == DateTime.MinValue)
                inscripcion.FechaInscripcion = DateTime.Now;

            if (_context.Inscripciones.Any(i =>
                i.AlumnoId == inscripcion.AlumnoId &&
                i.MateriaCursadaId == inscripcion.MateriaCursadaId))
            {
                ModelState.AddModelError("", "Ya existe una inscripción con esa combinación.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlumnoId"] = new SelectList(_context.Alumnos, "Id", "Nombre", inscripcion.AlumnoId);
            ViewData["MateriaCursadaId"] = new SelectList(_context.MateriasCursadas, "Id", "CodigoCursada", inscripcion.MateriaCursadaId);
            return View(inscripcion);
        }

        // GET: Inscripciones/Edit
        public async Task<IActionResult> Edit(int? alumnoId, int? materiaCursadaId)
        {
            if (alumnoId == null || materiaCursadaId == null)
                return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(i => i.AlumnoId == alumnoId && i.MateriaCursadaId == materiaCursadaId);

            if (inscripcion == null)
                return NotFound();

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas, "Id", "CodigoCursada", inscripcion.MateriaCursadaId);

            return View(inscripcion);
        }

        // POST: Inscripciones/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("AlumnoId,MateriaCursadaId,FechaInscripcion,Activa")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.AlumnoId, inscripcion.MateriaCursadaId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas, "Id", "CodigoCursada", inscripcion.MateriaCursadaId);
            return View(inscripcion);
        }

        // GET: Inscripciones/Delete
        public async Task<IActionResult> Delete(int? alumnoId, int? materiaCursadaId)
        {
            if (alumnoId == null || materiaCursadaId == null)
                return NotFound();

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(m => m.AlumnoId == alumnoId && m.MateriaCursadaId == materiaCursadaId);

            if (inscripcion == null)
                return NotFound();

            return View(inscripcion);
        }

        // POST: Inscripciones/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int alumnoId, int materiaCursadaId)
        {
            var inscripcion = await _context.Inscripciones
                .FirstOrDefaultAsync(i => i.AlumnoId == alumnoId && i.MateriaCursadaId == materiaCursadaId);

            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InscripcionExists(int alumnoId, int materiaCursadaId)
        {
            return _context.Inscripciones.Any(e => e.AlumnoId == alumnoId && e.MateriaCursadaId == materiaCursadaId);
        }
    }
}
