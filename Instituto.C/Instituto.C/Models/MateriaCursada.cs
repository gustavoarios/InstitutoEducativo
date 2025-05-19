using System;
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
        [StringLength(1, ErrorMessage = Messages.StringMax)]
        [RegularExpression(@"^[A-Z]+$", ErrorMessage = Messages.RestriccionLetras)]
        [Display(Name = "Código de Cursada (A, B, C, etc)")]
        public string CodigoCursada { get; set; } //Ej: "A", "B", "C", etc.

        [Range(2023, 2100, ErrorMessage = Messages.Rango)]
        [Display(Name = "Año de Cursada")]
        public int Anio { get; set; } // Ej: 2025

        [Range(1, 2, ErrorMessage = Messages.Rango)]
        [Display(Name = "Cuatrimestre de Cursada")]
        public int Cuatrimestre { get; set; } //Ej: 1 o 2

        public bool Activo { get; set; } = false; //Indica si la cursada esta activa o no

        public Profesor Profesor { get; set; } // Propiedad de navegación: relación con la entidad Profesor (clave foránea implícita)
        public Materia Materia { get; set; } // Propiedad de navegación: relación con la entidad Materia (clave foránea implícita)
        public int MateriaId { get; set; } //Id de la materia, propiedad relacional, Clave foránea explícita para la entidad Materia
        public int ProfesorId { get; set; } //Id del profesor a cargo de la cursada, propiedad relacional, Clave foránea explícita para la entidad Profesor
        public List<Inscripcion> Inscripciones { get; set; } = new(); //Inscripciones a la materia 

        //propiedad calculada
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = "Nombre de la Cursada")]
        public string Nombre => $"{Materia?.CodigoMateria ?? "SinMateria"} {Anio} {Cuatrimestre}{CodigoCursada}";
    }

}

