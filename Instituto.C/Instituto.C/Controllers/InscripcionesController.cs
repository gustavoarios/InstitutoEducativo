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
        public async Task<IActionResult> Create([Bind("AlumnoId,MateriaCursadaId")] Inscripcion inscripcion)
        {
            if (inscripcion == null)
                return BadRequest();

            // Seteo la fecha de inscripción
            inscripcion.FechaInscripcion = DateTime.Now;
            inscripcion.Activa = true;

            // Traigo el alumno y sus inscripciones activas o pasadas para validaciones
            var alumno = await _context.Alumnos
                .Include(a => a.Inscripciones)
                .ThenInclude(i => i.MateriaCursada)
                .FirstOrDefaultAsync(a => a.Id == inscripcion.AlumnoId);

            if (alumno == null)
            {
                ModelState.AddModelError("", "Alumno no encontrado.");
                return View(inscripcion);
            }

            // Traigo la cursada seleccionada con sus inscripciones
            var cursada = await _context.MateriasCursadas
                .Include(mc => mc.Inscripciones)
                .Include(mc => mc.Materia)
                .Include(mc => mc.Profesor)
                .FirstOrDefaultAsync(mc => mc.Id == inscripcion.MateriaCursadaId);

            if (cursada == null)
            {
                ModelState.AddModelError("", "Materia cursada no encontrada.");
                return View(inscripcion);
            }

            // 1. Validar que el alumno no esté cursando ni haya cursado la materia
            bool yaCursada = alumno.Inscripciones.Any(i =>
                i.MateriaCursada.MateriaId == cursada.MateriaId &&
                (i.Activa || i.Calificacion != null));

            if (yaCursada)
            {
                ModelState.AddModelError("", "Ya cursaste o estás cursando esta materia.");
                return View(inscripcion);
            }

            // 2. Validar que no supere 5 materias activas
            int materiasActivas = alumno.Inscripciones.Count(i => i.Activa);
            if (materiasActivas >= 5)
            {
                ModelState.AddModelError("", "No podés inscribirte en más de 5 materias a la vez."); //debería ser contante el 5??
                return View(inscripcion);
            }

            // 3. Validar cupo
            if (cursada.EstaLleno())
            {
                // Crear nueva cursada automáticamente
                var nuevaCursada = cursada.CrearNuevaCursadaSiEstaLleno();

                if (nuevaCursada == null)
                {
                    ModelState.AddModelError("", "No se pudo crear una nueva cursada automática.");
                    return View(inscripcion);
                }

                // Asignar profesor automáticamente (ejemplo simple)
                nuevaCursada.ProfesorId = await ObtenerProfesorDisponible(nuevaCursada.MateriaId);
                _context.MateriasCursadas.Add(nuevaCursada);
                await _context.SaveChangesAsync();

                // Inscribirse en la nueva cursada
                inscripcion.MateriaCursadaId = nuevaCursada.Id;
                cursada = nuevaCursada;
            }

            // 4. Verificar que no exista inscripción duplicada con esa cursada final (por las dudas)
            bool inscripcionDuplicada = await _context.Inscripciones.AnyAsync(i =>
                i.AlumnoId == inscripcion.AlumnoId &&
                i.MateriaCursadaId == inscripcion.MateriaCursadaId);

            if (inscripcionDuplicada)
            {
                ModelState.AddModelError("", "Ya estás inscripto en esta materia cursada.");
                return View(inscripcion);
            }

            // 5. Guardar inscripción
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //obtener profesor
        private async Task<int> ObtenerProfesorDisponible(int materiaId)
        {
            //devuelve el primer profesor que tenga la materia asignada
            var profesor = await _context.Profesores
                .Include(p => p.MateriasCursada)
                .FirstOrDefaultAsync(p => p.MateriasCursada.Any(m => m.Id == materiaId));

            return profesor?.Id ?? 0; // o manejar caso sin profesor asignado
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