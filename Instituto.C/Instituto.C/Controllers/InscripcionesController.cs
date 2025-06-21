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

    [Authorize]
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
            if (User.IsInRole("AlumnoRol")) //para que solo vea las suyas
            {
                var userName = User.Identity.Name;

                var alumno = await _context.Alumnos
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.MateriaCursada)
                            .ThenInclude(mc => mc.Materia)
                    .FirstOrDefaultAsync(a => a.UserName == userName);

                if (alumno == null)
                    return NotFound("No se encontró al alumno logueado.");

                return View(alumno.Inscripciones);
            }

            // Empleados o roles con más permisos ven todo
            var inscripciones = await _context.Inscripciones
                .Include(i => i.Alumno)
                .Include(i => i.MateriaCursada)
                    .ThenInclude(mc => mc.Materia)
                .ToListAsync();

            return View(inscripciones);
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
        [Authorize(Roles = "EmpleadoRol,AlumnoRol")]
        public IActionResult Create()
        {
            if (User.IsInRole("EmpleadoRol"))
            {
                // Empleado: puede elegir cualquier alumno ACTIVO
                var alumnosActivos = _context.Alumnos
                    .Where(a => a.Activo)
                    .ToList(); // eso es para evitar errores por herencia en propiedades, por las dudas

                ViewData["AlumnoId"] = new SelectList(
                    alumnosActivos.Select(a => new
                    {
                        a.Id,
                        Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                    }), "Id", "Nombre");

                // Puede ver todas las cursadas disponibles
                var todasCursadas = _context.MateriasCursadas
                    .Include(mc => mc.Materia)
                    .ToList();

                ViewData["MateriaCursadaId"] = new SelectList(
                    todasCursadas.Select(mc => new
                    {
                        mc.Id,
                        Nombre = mc.Nombre
                    }), "Id", "Nombre");

                return View();
            }
            else if (User.IsInRole("AlumnoRol"))
            {
                // Buscar al alumno logueado y validar que esté activo
                var alumno = _context.Alumnos
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.MateriaCursada)
                            .ThenInclude(mc => mc.Materia)
                    .FirstOrDefault(a => a.UserName == User.Identity.Name);

                if (alumno == null)
                {
                    TempData["Error"] = "No se pudo encontrar tu usuario.";
                    return RedirectToAction("Index");
                }

                if (!alumno.Activo)
                {
                    TempData["Error"] = "Tu cuenta aún no está activa. No podés inscribirte.";
                    return RedirectToAction("Index");
                }

                // Traer todas las cursadas activas
                var todasLasCursadas = _context.MateriasCursadas
                    .Include(mc => mc.Materia)
                    .Where(mc => mc.Activo)
                    .ToList();

                // Usar el helper para filtrar solo las que puede inscribirse
                var disponibles = MateriasHelper.FiltrarMateriasDisponiblesParaAlumno(alumno, todasLasCursadas);

                ViewData["AlumnoId"] = new SelectList(new[] {
            new {
                alumno.Id,
                Nombre = alumno.NumeroMatricula + " - " + alumno.Nombre + " " + alumno.Apellido
            }
        }, "Id", "Nombre", alumno.Id);

                ViewData["MateriaCursadaId"] = new SelectList(
                    disponibles.Select(mc => new
                    {
                        mc.Id,
                        Nombre = mc.Nombre
                    }), "Id", "Nombre");

                // Ya seteamos la inscripción con el alumno preseleccionado
                return View(new Inscripcion { AlumnoId = alumno.Id });
            }

            // algo de seguridad extra, igual en teoria sabemos q no debería llegar acá nadie más
            return Unauthorized();
        }


        // POST: Inscripciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EmpleadoRol,AlumnoRol")]
        public async Task<IActionResult> Create([Bind("AlumnoId,MateriaCursadaId")] Inscripcion inscripcion)
        {
            if (inscripcion == null)
                return BadRequest();

            inscripcion.FechaInscripcion = DateTime.Now;
            inscripcion.Activa = true;

            var alumno = await _context.Alumnos
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.MateriaCursada)
                .FirstOrDefaultAsync(a => a.Id == inscripcion.AlumnoId);

            if (alumno == null)
            {
                ModelState.AddModelError("", "Alumno no encontrado.");
                return View(inscripcion);
            }

            if (!alumno.Activo)
            {
                ModelState.AddModelError("", "Solo los alumnos activos pueden inscribirse.");
                return View(inscripcion);
            }

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

            if (alumno.CarreraId != cursada.Materia.CarreraId)
            {
                ModelState.AddModelError("", "No podés inscribirte a materias que no pertenecen a tu carrera.");
                return View(inscripcion);
            }

            bool yaCursada = alumno.Inscripciones.Any(i =>
                i.MateriaCursada.MateriaId == cursada.MateriaId &&
                (i.Activa || i.Calificacion != null));

            if (yaCursada)
            {
                ModelState.AddModelError("", "Ya cursaste o estás cursando esta materia.");
                return View(inscripcion);
            }

            int materiasActivas = alumno.Inscripciones.Count(i => i.Activa);
            if (materiasActivas >= 5)
            {
                ModelState.AddModelError("", "No podés inscribirte en más de 5 materias a la vez.");
                return View(inscripcion);
            }

            // uso helper si está llena
            if (MateriasHelper.EstaLleno(cursada))
            {
                var nuevaCursada = MateriasHelper.CrearNuevaCursadaSiEstaLleno(cursada);

                if (nuevaCursada == null)
                {
                    ModelState.AddModelError("", "No se pudo crear una nueva cursada automática.");
                    return View(inscripcion);
                }

                // Asignar objeto Materia para poder generar nombre
                nuevaCursada.Materia = cursada.Materia;

                // Asignar nombre con helper
                nuevaCursada.Nombre = MateriasHelper.GenerarNombreCursada(nuevaCursada);

                // Asignar profesor automáticamente
                nuevaCursada.ProfesorId = await ObtenerProfesorDisponible(nuevaCursada.MateriaId);

                _context.MateriasCursadas.Add(nuevaCursada);
                await _context.SaveChangesAsync();

                // Redirigir inscripción a la nueva cursada
                inscripcion.MateriaCursadaId = nuevaCursada.Id;
                cursada = nuevaCursada;
            }

            // Chequear duplicado exacto
            bool inscripcionDuplicada = await _context.Inscripciones.AnyAsync(i =>
                i.AlumnoId == inscripcion.AlumnoId &&
                i.MateriaCursadaId == inscripcion.MateriaCursadaId);

            if (inscripcionDuplicada)
            {
                ModelState.AddModelError("", "Ya estás inscripto en esta materia cursada.");
                return View(inscripcion);
            }

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task<int> ObtenerProfesorDisponible(int materiaId)
        {
            var profesor = await _context.Profesores
                .Include(p => p.MateriasCursada)
                .FirstOrDefaultAsync(p => p.MateriasCursada.Any(m => m.MateriaId == materiaId));

            return profesor?.Id ?? 0;
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
                .ThenInclude(mc => mc.Materia)
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
                .Include(i => i.Calificacion)
                .Include(i => i.MateriaCursada)
                .FirstOrDefaultAsync(i => i.AlumnoId == alumnoId && i.MateriaCursadaId == materiaCursadaId);

            if (inscripcion == null)
            {
                return NotFound();
            }

            // si ya tiene una calificacion, el alumno no se puede dar de baja de la inscripcion hecha
            if (inscripcion.Calificacion != null)
            {
                TempData["Error"] = "No podés cancelar la inscripción porque ya tenés una calificación registrada.";
                return RedirectToAction(nameof(Index));
            }

            // obtenemos el profesor asignado a esa materia
            var profesorId = inscripcion.MateriaCursada?.ProfesorId ?? 0;

            // creamos una calificación de baja
            var calificacion = new Calificacion
            {
                AlumnoId = alumnoId,
                MateriaCursadaId = materiaCursadaId,
                ProfesorId = profesorId,
                Fecha = DateTime.Now,
                Nota = Nota.Baja
            };

            _context.Calificaciones.Add(calificacion);

            // Guardamos sin eliminar la inscripción
            await _context.SaveChangesAsync();

            TempData["Success"] = "Te diste de baja correctamente. Se registró una calificación como 'Baja'.";
            return RedirectToAction(nameof(Index));
        }





        private bool InscripcionExists(int alumnoId, int materiaCursadaId)
        {
            return _context.Inscripciones.Any(e => e.AlumnoId == alumnoId && e.MateriaCursadaId == materiaCursadaId);
        }


        [Authorize(Roles = "AlumnoRol")]
        public async Task<IActionResult> MateriasActuales()
        {
            var userName = User.Identity.Name;

            var alumno = await _context.Alumnos
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.MateriaCursada)
                        .ThenInclude(mc => mc.Materia)
                .Include(a => a.Inscripciones)
                    .ThenInclude(i => i.MateriaCursada)
                        .ThenInclude(mc => mc.Profesor)
                .FirstOrDefaultAsync(a => a.UserName == userName);

            if (alumno == null)
                return NotFound();

            var actuales = alumno.Inscripciones
                .Where(i => i.Activa)
                .ToList();

            return View(actuales); //esto conecta directo con la vista
        }

        // Acción para ver los compañeros de una cursada
        [Authorize(Roles = "AlumnoRol,EmpleadoRol")]
        public async Task<IActionResult> Companieros(int materiaCursadaId)
        {
            // Validamos que exista la materia cursada
            var cursada = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .FirstOrDefaultAsync(mc => mc.Id == materiaCursadaId);

            if (cursada == null)
            {
                return NotFound("La materia cursada no existe.");
            }

            // Traemos las inscripciones activas de esa cursada, con los datos de los alumnos
            var inscripciones = await _context.Inscripciones
                .Where(i => i.MateriaCursadaId == materiaCursadaId && i.Activa)
                .Include(i => i.Alumno)
                .ToListAsync();

            // Mandamos la info a la vista
            ViewBag.NombreMateria = cursada.Materia.Nombre;
            ViewBag.NombreCursada = cursada.Nombre;

            return View(inscripciones);
        }

    }
}
