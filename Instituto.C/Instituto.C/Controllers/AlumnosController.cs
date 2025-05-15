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
    public class AlumnosController : Controller
    {
        private readonly InstitutoDb _context;

        public AlumnosController(InstitutoDb context)
        {
            _context = context;
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

        public async Task<IActionResult> Create([Bind("CarreraId,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alumno);
                await _context.SaveChangesAsync(); // acá el alumno ya tiene Id asignado

                //asigno el número de matrícula
                var gestor = new GestorAlumnos(); //instancio al gestor para usar el metodo que asigna la matricula, pasandole el alumno recien creado
                gestor.AsignarNumeroMatricula(alumno, _context); //aparte del alumno, le paso el campo privado de acceso a la base de datos que se usa en AlumnosController

                //guardo nuevamente para actualizar la matrícula
                _context.Update(alumno);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
            return View(alumno);
        }


        /*public async Task<IActionResult> Create([Bind("NumeroMatricula,CarreraId,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
            return View(alumno);
        }*/


        // GET: Alumnos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Edit(int id, [Bind("NumeroMatricula,CarreraId,Id,UserName,Email,FechaAlta,Nombre,Apellido,DNI,Telefono,Direccion,Activo")] Alumno alumno)
        {
            if (id != alumno.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.Id))
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
            ViewData["CarreraId"] = new SelectList(_context.Carreras, "Id", "CodigoCarrera", alumno.CarreraId);
            return View(alumno);
        }

        // GET: Alumnos/Delete/5
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
