using Instituto.C.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        //relacion con Profesor
        public Profesor Profesor { get; set; } // Propiedad de navegación: relación con la entidad Profesor (clave foránea implícita)
        public int ProfesorId { get; set; } //Id del profesor a cargo de la cursada, propiedad relacional, Clave foránea explícita para la entidad Profesor

        //relacion con Materia
        public Materia Materia { get; set; } // Propiedad de navegación: relación con la entidad Materia (clave foránea implícita)
        public int MateriaId { get; set; } //Id de la materia, propiedad relacional, Clave foránea explícita para la entidad Materia

        public List<Inscripcion> Inscripciones { get; set; } = new(); //Inscripciones a la materia 

        //propiedad calculada
        [Required(ErrorMessage = Messages.CampoObligatorio)]
        [StringLength(100, MinimumLength = 2, ErrorMessage = Messages.StrMaxMin)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = Messages.RestriccionAlfanumerica)]
        [Display(Name = "Nombre de la Cursada")]
        public string Nombre => $"{Materia?.CodigoMateria ?? "SinMateria"} {Anio} {Cuatrimestre} {CodigoCursada}"; //BIO101-2025-1C-A

        // Método helper para saber si alcanzó el cupo
        public bool EstaLleno()
        {
            if (Inscripciones == null || Materia == null)
                return false;

            int inscripcionesActivas = Inscripciones.Count(i => i.Activa);

            return inscripcionesActivas >= Materia.CupoMaximo;
        }

        // metodo para crear una nueva cursada si la actual está llena
        public MateriaCursada CrearNuevaCursadaSiEstaLleno()
        {
            if (EstaLleno())
            {
                // Crear una nueva cursada con código siguiente
                var nuevaCursada = new MateriaCursada
                {
                    Materia = this.Materia,
                    Anio = this.Anio,
                    Cuatrimestre = this.Cuatrimestre,
                    // El código de cursada siguiente (A->B->C...)
                    CodigoCursada = ObtenerSiguienteCodigoCursada(this.CodigoCursada),
                    Activo = true,
                    Profesor = this.Profesor
                };

                // Aquí podrías agregar la nueva cursada a una colección si manejas una lista global, o devolverla para que la manejes afuera.
                return nuevaCursada;
            }

            // Si no está lleno, no se crea nada (podrías devolver null o throw una excepción, según lo que quieras)
            return null;
        }

        // Método privado helper para obtener la siguiente letra de cursada
        public string ObtenerSiguienteCodigoCursada(string codigoActual)
        {
            if (string.IsNullOrEmpty(codigoActual)) return "A";

            char codigo = codigoActual[0];

            if (codigo == 'Z')
                throw new InvalidOperationException("No se pueden crear más cursadas, límite alcanzado.");

            codigo++;

            return codigo.ToString();
        }
    }
    }

