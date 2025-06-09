using System.ComponentModel.DataAnnotations;

namespace Instituto.C.Models
{
    public class Login
    {

        //esto sería un modelo dedicado para manejar la vista

        [Required]
        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public bool Recordarme { get; set; }


    }
}
