using System.Collections.Generic;
using Instituto.C.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace Instituto.C.Models
{
    public class Carrera
    {
        //propiedades de la Carrera
        public int Id { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ\s]+$", ErrorMessage = Messages.RestriccionLetras)]
        [Display(Name = Alias.Carrera)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(10, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = Alias.Codigo)]
        public string CodigoCarrera { get; set; }

        public List<Materia> Materias { get; set; } = []; //inicializo la lista para evitar null reference exception
        public List<Alumno> Alumnos { get; set; } = []; //inicializo la lista para evitar null reference exception

      

    }
}