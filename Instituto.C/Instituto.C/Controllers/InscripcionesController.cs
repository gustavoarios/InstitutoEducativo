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

        [Authorize(Roles = "EmpleadoRol")]
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
        [Authorize(Roles = "EmpleadoRol")]
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
                var alumnosActivos = _context.Alumnos
                    .Where(a => a.Activo)
                    .ToList();

                ViewData["AlumnoId"] = new SelectList(
                    alumnosActivos.Select(a => new
                    {
                        a.Id,
                        Nombre = a.NumeroMatricula + " - " + a.Nombre + " " + a.Apellido
                    }), "Id", "Nombre");

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
                var alumno = _context.Alumnos
                    .AsNoTracking()
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.MateriaCursada)
                            .ThenInclude(mc => mc.Materia)
                    .FirstOrDefault(a => a.UserName == User.Identity.Name);

                if (alumno == null)
                {
                    TempData["Error"] = "No se pudo encontrar tu usuario.";
                    return RedirectToAction("MateriasActuales");
                }

                if (!alumno.Activo)
                {
                    TempData["Error"] = "Tu cuenta aún no está activa. No podés inscribirte.";
                    return RedirectToAction("MateriasActuales");
                }

                var todasLasCursadas = _context.MateriasCursadas
                    .Include(mc => mc.Materia)
                    .Include(mc => mc.Inscripciones)
                    .Where(mc => mc.Activo && mc.Materia.CarreraId == alumno.CarreraId)
                    .ToList();

                var yaCursadas = alumno.Inscripciones
                    .Where(i => i.Calificacion != null || i.Activa)
                    .Select(i => i.MateriaCursada.MateriaId)
                    .ToHashSet();

                var opcionesFinales = todasLasCursadas
                    .Where(mc => !yaCursadas.Contains(mc.MateriaId))
                    .GroupBy(mc => new { mc.MateriaId, mc.Anio, mc.Cuatrimestre })
                    .Select(grupo => grupo
                        .OrderByDescending(mc => mc.CodigoCursada)
                        .FirstOrDefault(mc => !MateriasHelper.EstaLleno(mc))
                            ?? grupo.OrderByDescending(mc => mc.CodigoCursada).First()
                    )
                    .Select(mc => new
                    {
                        mc.Id,
                        Nombre = mc.Nombre
                    })
                    .ToList();

                ViewData["AlumnoId"] = new SelectList(new[] {
                    new {
                        alumno.Id,
                        Nombre = alumno.NumeroMatricula + " - " + alumno.Nombre + " " + alumno.Apellido
                    }
                }, "Id", "Nombre", alumno.Id);

                ViewData["MateriaCursadaId"] = new SelectList(opcionesFinales, "Id", "Nombre");

                return View(new Inscripcion { AlumnoId = alumno.Id });
            }

            return Unauthorized();
        }



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
                TempData["Error"] = "Alumno no encontrado.";
                return RedirigirSegunRol();
            }

            if (!alumno.Activo)
            {
                TempData["Error"] = "Solo los alumnos activos pueden inscribirse.";
                return RedirigirSegunRol();
            }

            var cursada = await _context.MateriasCursadas
                .Include(mc => mc.Inscripciones)
                .Include(mc => mc.Materia)
                .Include(mc => mc.Profesor)
                .FirstOrDefaultAsync(mc => mc.Id == inscripcion.MateriaCursadaId);

            if (cursada == null)
            {
                TempData["Error"] = "Materia cursada no encontrada.";
                return RedirigirSegunRol();
            }

            if (alumno.CarreraId != cursada.Materia.CarreraId)
            {
                TempData["Error"] = User.IsInRole("EmpleadoRol")
                    ? "El alumno no puede inscribirse a materias que no pertenecen a su carrera."
                    : "No podés inscribirte a materias que no pertenecen a tu carrera.";
                return RedirigirSegunRol();
            }

            // 🔍 Unificación de validación
            bool yaLaCurso = alumno.Inscripciones.Any(i =>
                i.MateriaCursada.MateriaId == cursada.MateriaId &&
                (i.Activa || i.Calificacion != null));

            bool yaEstaInscriptoExacto = alumno.Inscripciones.Any(i =>
                i.MateriaCursadaId == inscripcion.MateriaCursadaId);

            if (yaLaCurso || yaEstaInscriptoExacto)
            {
                TempData["Error"] = User.IsInRole("EmpleadoRol")
                    ? "El alumno ya cursó o está inscripto en esta materia."
                    : "Ya cursaste o estás inscripto en esta materia.";
                return RedirigirSegunRol();
            }

            int materiasActivas = alumno.Inscripciones.Count(i => i.Activa);
            if (materiasActivas >= 5)
            {
                TempData["Error"] = User.IsInRole("EmpleadoRol")
                    ? "El alumno no puede inscribirse en más de 5 materias a la vez."
                    : "No podés inscribirte en más de 5 materias a la vez.";
                return RedirigirSegunRol();
            }

            // 🧪 Verificar si la cursada está llena
            if (MateriasHelper.EstaLleno(cursada))
            {
                var nuevaCursada = MateriasHelper.CrearNuevaCursadaSiEstaLleno(cursada, _context);

                if (nuevaCursada == null)
                {
                    TempData["Error"] = "No se pudo crear una nueva cursada automática.";
                    return RedirigirSegunRol();
                }

                nuevaCursada.Materia = cursada.Materia;
                nuevaCursada.Nombre = MateriasHelper.GenerarNombreCursada(nuevaCursada);
                nuevaCursada.ProfesorId = await ObtenerProfesorDisponible(nuevaCursada.MateriaId);

                _context.MateriasCursadas.Add(nuevaCursada);
                await _context.SaveChangesAsync();

                inscripcion.MateriaCursadaId = nuevaCursada.Id;
                cursada = nuevaCursada;
            }

            // ✅ Crear inscripción final
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            TempData["Success"] = User.IsInRole("EmpleadoRol")
                ? "Inscripción creada correctamente para el alumno."
                : "Te inscribiste correctamente a la materia.";

            return RedirigirSegunRol();
        }




        private async Task<int> ObtenerProfesorDisponible(int materiaId)
        {
            var profesor = await _context.Profesores
                .Include(p => p.MateriasCursada)
                .FirstOrDefaultAsync(p => p.MateriasCursada.Any(m => m.MateriaId == materiaId));

            return profesor?.Id ?? 0;
        }

        // GET: Inscripciones/Edit
        [Authorize(Roles = "EmpleadoRol")]
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
        [Authorize(Roles = "EmpleadoRol")]
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
        [Authorize(Roles = "EmpleadoRol,AlumnoRol")]
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
        [Authorize(Roles = "EmpleadoRol,AlumnoRol")]
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

            // si ya tiene una calificación, el alumno no se puede dar de baja
            if (inscripcion.Calificacion != null)
            {
                TempData["Error"] = User.IsInRole("EmpleadoRol")
                    ? "No se puede dar de baja la inscripción porque el alumno ya tiene una calificación registrada."
                    : "No podés cancelar la inscripción porque ya tenés una calificación registrada.";
                return User.IsInRole("AlumnoRol")
                    ? RedirectToAction("MateriasActuales")
                    : RedirectToAction(nameof(Index));
            }

            // obtenemos el profesor asignado
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

            inscripcion.Activa = false;
            _context.Calificaciones.Add(calificacion);

            await _context.SaveChangesAsync();

            TempData["Success"] = User.IsInRole("EmpleadoRol")
                ? "La inscripción fue dada de baja correctamente. Se registró una calificación como 'Baja'."
                : "Te diste de baja correctamente. Se registró una calificación como 'Baja'.";

            return User.IsInRole("AlumnoRol")
                ? RedirectToAction("MateriasActuales")
                : RedirectToAction(nameof(Index));
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

        //usamos este metodo para redirigir dependiendo de que rol creé la inscripcion
        private IActionResult RedirigirSegunRol()
        {
            return User.IsInRole("EmpleadoRol")
                ? RedirectToAction("Index", "Inscripciones")
                : RedirectToAction("MateriasActuales");
        }


    }
}
