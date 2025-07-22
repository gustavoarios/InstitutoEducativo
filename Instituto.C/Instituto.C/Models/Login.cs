using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;

namespace Instituto.C.Models
{
    public class Login
    {

        //esto sería un modelo dedicado para manejar la vista

        [Required]
        [Display(Name = Alias.UserName)]

        public string UserName { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = Alias.Contrasenia)]
        public string Password { get; set; }


        public bool Recordarme { get; set; }


    }
}
