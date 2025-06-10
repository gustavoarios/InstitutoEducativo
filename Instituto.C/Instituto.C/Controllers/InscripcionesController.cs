using System;
using System.Collections.Generic;
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
            var institutoDb = _context.Inscripciones.Include(i => i.Alumno).Include(i => i.MateriaCursada);
            return View(await institutoDb.ToListAsync());
        }

        // GET: Inscripciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inscripcion == null)
            {
                return NotFound();
            }

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
                }),
                "Id", "Nombre");

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas.Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.CodigoCursada
                }),
                "Id", "Nombre");

            return View();
        }

        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AlumnoId,MateriaCursadaId,FechaInscripcion,Activa")] Inscripcion inscripcion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inscripcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", inscripcion.AlumnoId);

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas.Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.CodigoCursada
                }),
                "Id", "Nombre", inscripcion.MateriaCursadaId);

            return View(inscripcion);
        }

        // GET: Inscripciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", inscripcion.AlumnoId);

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas.Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.CodigoCursada
                }),
                "Id", "Nombre", inscripcion.MateriaCursadaId);

            return View(inscripcion);
        }

        // POST: Inscripciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AlumnoId,MateriaCursadaId,FechaInscripcion,Activa")] Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscripcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscripcionExists(inscripcion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AlumnoId"] = new SelectList(
                _context.Alumnos.Select(a => new
                {
                    a.Id,
                    Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                }),
                "Id", "Nombre", inscripcion.AlumnoId);

            ViewData["MateriaCursadaId"] = new SelectList(
                _context.MateriasCursadas.Select(mc => new
                {
                    mc.Id,
                    Nombre = mc.CodigoCursada
                }),
                "Id", "Nombre", inscripcion.MateriaCursadaId);

            return View(inscripcion);
        }

        // GET: Inscripciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscripcion = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inscripcion == null)
            {
                return NotFound();
            }

            return View(inscripcion);
        }

        // POST: Inscripciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion != null)
            {
                _context.Inscripciones.Remove(inscripcion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripciones.Any(e => e.Id == id);
        }
    }
}
