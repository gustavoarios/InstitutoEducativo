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
    public class MateriasCursadasController : Controller
    {
        private readonly InstitutoDb _context;

        public MateriasCursadasController(InstitutoDb context)
        {
            _context = context;
        }

        // GET: MateriasCursadas
        public async Task<IActionResult> Index()
        {
            var institutoDb = _context.MateriaCursada.Include(m => m.Materia).Include(m => m.Profesor);
            return View(await institutoDb.ToListAsync());
        }

        // GET: MateriasCursadas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriaCursada
                .Include(m => m.Materia)
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaCursada == null)
            {
                return NotFound();
            }

            return View(materiaCursada);
        }

        // GET: MateriasCursadas/Create
        public IActionResult Create()
        {
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria");
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido");
            return View();
        }

        // POST: MateriasCursadas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodigoCursada,Anio,Cuatrimestre,Activo,MateriaId,ProfesorId")] MateriaCursada materiaCursada)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materiaCursada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }

        // GET: MateriasCursadas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriaCursada.FindAsync(id);
            if (materiaCursada == null)
            {
                return NotFound();
            }
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }

        // POST: MateriasCursadas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodigoCursada,Anio,Cuatrimestre,Activo,MateriaId,ProfesorId")] MateriaCursada materiaCursada)
        {
            if (id != materiaCursada.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materiaCursada);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaCursadaExists(materiaCursada.Id))
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
            ViewData["MateriaId"] = new SelectList(_context.Materias, "Id", "CodigoMateria", materiaCursada.MateriaId);
            ViewData["ProfesorId"] = new SelectList(_context.Profesores, "Id", "Apellido", materiaCursada.ProfesorId);
            return View(materiaCursada);
        }

        // GET: MateriasCursadas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materiaCursada = await _context.MateriaCursada
                .Include(m => m.Materia)
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (materiaCursada == null)
            {
                return NotFound();
            }

            return View(materiaCursada);
        }

        // POST: MateriasCursadas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materiaCursada = await _context.MateriaCursada.FindAsync(id);
            if (materiaCursada != null)
            {
                _context.MateriaCursada.Remove(materiaCursada);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaCursadaExists(int id)
        {
            return _context.MateriaCursada.Any(e => e.Id == id);
        }
    }
}
