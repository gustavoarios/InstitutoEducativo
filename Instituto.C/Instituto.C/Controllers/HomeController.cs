using System.Diagnostics;
using Instituto.C.Data;
using Instituto.C.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Instituto.C.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InstitutoDb _context;




        public HomeController(InstitutoDb context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult Index(string message) //no va null porque por defecto el string es null
        {

            ViewBag.Message = message;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "EmpleadoRol")]
        public IActionResult Dashboard()
        {
            var model = new DashboardViewModel
            {
                CantidadAlumnosActivos = _context.Alumnos.Count(a => a.Activo),
                CantidadProfesoresActivos = _context.Profesores.Count(p => p.Activo),
                CantidadMateriasEnCurso = _context.MateriasCursadas.Count(m => m.Activo ),
                CantidadInscripcionesActivas = _context.Inscripciones.Count(i => i.Activa)
            };

            return View(model);
        }

        public IActionResult Nosotros()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
