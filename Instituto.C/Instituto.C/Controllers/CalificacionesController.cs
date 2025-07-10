using Instituto.C.Data;
using Instituto.C.Helpers;
using Instituto.C.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instituto.C.Controllers
{

    [Authorize]
    public class CalificacionesController : Controller
    {
        private readonly InstitutoDb _context;

        public CalificacionesController(InstitutoDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<Calificacion> query = _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Profesor)
                .Include(c => c.MateriaCursada)
                    .ThenInclude(mc => mc.Materia);

            if (User.IsInRole("AlumnoRol"))
            {
                var userName = User.Identity.Name;
                var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.UserName == userName);

                if (alumno == null)
                {
                    TempData["Error"] = "No se pudo identificar al alumno.";
                    return RedirectToAction("Index", "Home");
                }

                query = query.Where(c => c.AlumnoId == alumno.Id);
            }
            else if (User.IsInRole("ProfesorRol"))
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.Id == userId);

                if (profesor == null)
                {
                    TempData["Error"] = "No se pudo identificar al profesor.";
                    return RedirectToAction("Index", "Home");
                }

                query = query.Where(c => c.ProfesorId == profesor.Id);
            }

            var calificaciones = await query.ToListAsync();
            return View(calificaciones);
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

            var viewModel = new CalificacionDetalleViewModel
            {
                Calificacion = calificacion,
                NotaTexto = calificacion.Nota switch
                {
                    Nota.Ausente => "Ausente",
                    Nota.Baja => "Baja",
                    Nota.Pendiente => "Pendiente",
                    _ => ((int)calificacion.Nota).ToString()
                }

            };

            return View(viewModel);
        }


        //GET CREATE

        [Authorize(Roles = "ProfesorRol")]
        public IActionResult Create(int? alumnoId, int? materiaCursadaId)
        {
            var userName = User.Identity.Name;
            var profesor = _context.Profesores.FirstOrDefault(p => p.UserName == userName);
            if (profesor == null) return Unauthorized();

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));
            ViewBag.ProfesorNombre = profesor.Nombre + " " + profesor.Apellido;
            ViewBag.ProfesorIdValor = profesor.Id;

            // Obtener materias activas del profesor
            var materiasDelProfesor = _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .Where(mc => mc.ProfesorId == profesor.Id && mc.Activo)
                .ToList();

            var materiasIds = materiasDelProfesor.Select(m => m.Id).ToList();

            if (alumnoId.HasValue && materiaCursadaId.HasValue)
            {
                var inscripcion = _context.Inscripciones
                    .Include(i => i.Alumno)
                    .Include(i => i.MateriaCursada)
                        .ThenInclude(mc => mc.Materia)
                    .FirstOrDefault(i =>
                        i.AlumnoId == alumnoId.Value &&
                        i.MateriaCursadaId == materiaCursadaId.Value &&
                        i.MateriaCursada.ProfesorId == profesor.Id &&
                        i.Activa);

                if (inscripcion != null)
                {
                    ViewBag.MateriaNombre = inscripcion.MateriaCursada.Nombre;
                    ViewBag.AlumnoNombre = inscripcion.Alumno.NombreCompleto;

                    ViewBag.MateriaCursadaIdValor = inscripcion.MateriaCursadaId;
                    ViewBag.AlumnoIdValor = inscripcion.AlumnoId;

                    return View(new Calificacion
                    {
                        AlumnoId = inscripcion.AlumnoId,
                        MateriaCursadaId = inscripcion.MateriaCursadaId,
                        Fecha = DateTime.Now
                    });
                }
            }

            // caso general, no debería usarse con flujo actual
            ViewBag.AlumnoNombre = null;
            ViewBag.MateriaNombre = null;
            ViewBag.AlumnoIdValor = 0;
            ViewBag.MateriaCursadaIdValor = 0;

            return View(new Calificacion());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> Create(Calificacion calificacion) //al asignar el calificacion.ProfesorId = profesor.Id, sacamos el bind, sino nos tira que el modelstate no va a ser válido
        {
            var userName = User.Identity.Name;
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == userName);
            if (profesor == null) return Unauthorized();

            calificacion.ProfesorId = profesor.Id;

            var cursada = await _context.MateriasCursadas
                                        .Include(mc => mc.Materia)
                                        .FirstOrDefaultAsync(mc => mc.Id == calificacion.MateriaCursadaId);

            if (cursada == null)
            {
                ModelState.AddModelError("", "La materia cursada no existe.");
                return RecargarVistaConCombos(calificacion);
            }

            if (!cursada.Activo)
            {
                ModelState.AddModelError("", "No se puede calificar una materia que ya finalizó.");
                return RecargarVistaConCombos(calificacion);
            }

            DateTime hoy = DateTime.Now;

            if (cursada.Cuatrimestre == 1)
            {
                if (hoy < new DateTime(hoy.Year, 1, 1) || hoy > new DateTime(hoy.Year, 7, 31))
                {
                    ModelState.AddModelError("", "Solo se pueden cargar calificaciones del 1er Cuatrimestre entre el 1 de enero y el 31 de julio.");
                    return RecargarVistaConCombos(calificacion);
                }
            }
            else if (cursada.Cuatrimestre == 2)
            {
                if (hoy < new DateTime(hoy.Year, 8, 1) || hoy > new DateTime(hoy.Year, 12, 31))
                {
                    ModelState.AddModelError("", "Solo se pueden cargar calificaciones del 2do Cuatrimestre entre el 1 de agosto y el 31 de diciembre.");
                    return RecargarVistaConCombos(calificacion);
                }
            }
            else
            {
                ModelState.AddModelError("", "El cuatrimestre de la materia no es válido.");
                return RecargarVistaConCombos(calificacion);
            }



            if (calificacion.Fecha == DateTime.MinValue)
                calificacion.Fecha = DateTime.Now;

            // Validar si el alumno está inscripto
            bool estaInscripto = await _context.Inscripciones.AnyAsync(i =>
                i.AlumnoId == calificacion.AlumnoId &&
                i.MateriaCursadaId == calificacion.MateriaCursadaId);

            if (!estaInscripto)
            {
                ModelState.AddModelError("", "El alumno no está inscripto en esa materia cursada.");
            }

            // Validar que el profesor sea el titular de la cursada
            if (cursada.ProfesorId != profesor.Id)
            {
                ModelState.AddModelError("", "No podés calificar esta materia porque no sos el profesor asignado.");
            }

            // Validar si ya fue calificado
            bool yaCalificado = await _context.Calificaciones
                .AnyAsync(c => c.AlumnoId == calificacion.AlumnoId && c.MateriaCursadaId == calificacion.MateriaCursadaId);

            if (yaCalificado)
            {
                ModelState.AddModelError("", "El alumno ya fue calificado en esta cursada.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(calificacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "La calificación se registró correctamente.";
                return RedirectToAction("DetallesMateriaProfesor", "Profesores", new { id = calificacion.MateriaCursadaId });
            }

            return RecargarVistaConCombos(calificacion);
        }


        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Buscar calificación incluyendo relaciones necesarias
            var calificacion = await _context.Calificaciones
                .Include(c => c.Alumno)
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.MateriaCursada)
                        .ThenInclude(mc => mc.Materia)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calificacion == null)
                return NotFound();

            // Obtener profesor logueado
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == User.Identity.Name);

            // Validar que la materia cursada de esta calificación pertenezca al profesor logueado
            if (calificacion.Inscripcion.MateriaCursada.ProfesorId != profesor.Id)
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            // Cargar datos para la vista
            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);

            ViewBag.AlumnoNombre = $"{calificacion.Alumno?.NumeroMatricula} - {calificacion.Alumno?.Nombre} {calificacion.Alumno?.Apellido}";
            ViewBag.MateriaNombre = $"{calificacion.Inscripcion?.MateriaCursada?.Materia?.CodigoMateria} - {calificacion.Inscripcion?.MateriaCursada?.CodigoCursada} ({calificacion.Inscripcion?.MateriaCursada?.Anio})";
            ViewBag.MateriaCursadaId = calificacion.MateriaCursadaId;

            return View(calificacion);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ProfesorRol")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Nota,ProfesorId,AlumnoId,MateriaCursadaId")] Calificacion calificacion)
        {
            if (id != calificacion.Id) return NotFound();

            // Obtener el profesor actual
            var profesor = await _context.Profesores.FirstOrDefaultAsync(p => p.UserName == User.Identity.Name);
            if (profesor == null) return Unauthorized();

            // Revalidar que la materia cursada pertenezca al profesor
            var materiaCursada = await _context.MateriasCursadas.FindAsync(calificacion.MateriaCursadaId);
            if (materiaCursada == null || materiaCursada.ProfesorId != profesor.Id)
            {
                return RedirectToAction("AccesoDenegado", "Account");
            }

            // Forzar que la calificación quede asociada al profesor correcto
            calificacion.ProfesorId = profesor.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calificacion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "La calificación fue actualizada correctamente.";
                    return RedirectToAction("DetallesMateriaProfesor", "Profesores", new { id = calificacion.MateriaCursadaId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalificacionExists(calificacion.Id)) return NotFound();
                    throw;
                }
            }

            // Recargar los textos en caso de error
            var alumno = await _context.Alumnos.FindAsync(calificacion.AlumnoId);
            var materia = await _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .FirstOrDefaultAsync(mc => mc.Id == calificacion.MateriaCursadaId);

            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)), calificacion.Nota);
            ViewBag.AlumnoNombre = $"{alumno?.NumeroMatricula} - {alumno?.Nombre} {alumno?.Apellido}";
            ViewBag.MateriaNombre = $"{materia?.Materia?.CodigoMateria} - {materia?.CodigoCursada} ({materia?.Anio})";
            ViewBag.MateriaCursadaId = calificacion.MateriaCursadaId;

            return View(calificacion);
        }




        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
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
        [Authorize(Roles = "Admin")] //aunque no existe, potencialmente sí y nadie los puede borrar
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
        private IActionResult RecargarVistaConCombos(Calificacion calificacion)
        {
            ViewBag.Notas = new SelectList(Enum.GetValues(typeof(Nota)));

            //seteamos los ids crudos para evitar errores en campos hidden
            ViewBag.AlumnoIdValor = calificacion.AlumnoId;
            ViewBag.MateriaCursadaIdValor = calificacion.MateriaCursadaId;

            var alumno = _context.Alumnos.FirstOrDefault(a => a.Id == calificacion.AlumnoId);
            var materia = _context.MateriasCursadas
                .Include(mc => mc.Materia)
                .FirstOrDefault(mc => mc.Id == calificacion.MateriaCursadaId);

            ViewBag.AlumnoNombre = alumno?.NombreCompleto;
            ViewBag.MateriaNombre = materia?.Nombre;

            var userName = User.Identity.Name;
            var profesor = _context.Profesores.FirstOrDefault(p => p.UserName == userName);
            if (profesor != null)
            {
                ViewBag.ProfesorNombre = profesor.Nombre + " " + profesor.Apellido;
                ViewBag.ProfesorIdValor = profesor.Id;
            }

            return View("Create", calificacion);
        }



    }
}
