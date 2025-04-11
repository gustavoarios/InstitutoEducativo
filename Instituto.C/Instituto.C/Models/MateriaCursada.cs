namespace Instituto.C.Models
{
    public class MateriaCursada
    {
        //propiedades de la MateriaCursada
        public int Id { get; set; } //Id de la materia cursada
        public Materia Materia { get; set; } //Materia que se cursa
        public string CodigoCursada { get; set; } //Ej: "A", "B", "C", etc.
        public int Anio { get; set; } //Ej: 2025
        public int Cuatrimestre { get; set; } //Ej: 1 o 2
        public bool Activo { get; set; } //Indica si la cursada esta activa o no
        public Profesor Profesor { get; set; } //Profesor a cargo de la materia
        public List<Inscripcion> Inscripciones { get; set; } //Inscripciones a la materia 

        //propiedad calculada
        public string Nombre
        {
            get
            {
               return $"{Materia.CodigoMateria} {Anio} {Cuatrimestre}{CodigoCursada} ";
            }
        }

    }
}
