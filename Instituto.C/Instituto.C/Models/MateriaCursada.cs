using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class MateriaCursada
    {
        //propiedades de la MateriaCursada
        public int Id { get; set; } //Id de la materia cursada

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(1, MinimumLength = 1, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = Messages.RestriccionLetras)]
        [Display(Name = "Código de Cursada (A, B, C, etc)")]
        public string CodigoCursada { get; set; } //Ej: "A", "B", "C", etc.

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(2023, 2100, ErrorMessage = Messages.Rango)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = Messages.RestriccionNumeros)]
        [Display(Name = "Año de Cursada")]
        [DisplayFormat(DataFormatString = "{0:yyyy}")]
        [DataType(DataType.Date)]
        public int Anio { get; set; } //Ej: 2025

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [Range(1, 2, ErrorMessage = Messages.Rango)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = Messages.RestriccionNumeros)]
        [Display(Name = "Cuatrimestre de Cursada")]
        public int Cuatrimestre { get; set; } //Ej: 1 o 2

        [Required(ErrorMessage = Messages.CampoObligatorio)]
        public bool Activo { get; set; } //Indica si la cursada esta activa o no

        public Profesor Profesor { get; set; } //Profesor a cargo de la materia
        public Materia Materia { get; set; } //Materia que se cursa
        public List<Inscripcion> Inscripciones { get; set; } //Inscripciones a la materia 

        //propiedad calculada
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = "Nombre de la Cursada")]
        public string Nombre
        {
            get
            {
               return $"{Materia.CodigoMateria} {Anio} {Cuatrimestre}{CodigoCursada} ";
            }
        }

    }
}
