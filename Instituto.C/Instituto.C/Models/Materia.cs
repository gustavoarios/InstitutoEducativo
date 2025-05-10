using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Materia
    {
        //propiedades de la Materia
        public int Id { get; set; } //Id de la materia

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(30, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = "Nombre de Materia")]
        public string Nombre { get; set; } //Ej: "Programacion I"

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(10, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = "Código de Materia")]
        public string CodigoMateria { get; set; } //Ej: "P1"
       
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        public string Descripcion { get; set; } //Texto libre
       
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, 40, ErrorMessage = Messages.Rango)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = Messages.RestriccionNumeros)]
        [Display(Name = "Máximo de alumnos por comision")]
        public int CupoMaximo { get; set; } //Capacidad maxima de alumnos por comision

        public Carrera Carrera { get; set; } //Relacion con la carrera
        public int CarreraId { get; set; } //Id de la carrera, propiedad relacional
        public List<MateriaCursada> Cursadas { get; set; } = new List<MateriaCursada>(); //inicializo la lista para evitar null reference exception 
    }
}
