using System.ComponentModel.DataAnnotations;
using Instituto.C.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Instituto.C.Models
{
    public class Rol : IdentityRole<int>

    {

        public Rol() : base(){}

        public Rol(string name) : base(name) { }


        [Display(Name = Alias.RolName)]

        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override string NormalizedName
        {
            get { return base.NormalizedName; }
            set { base.NormalizedName = value; }
        }






    }
}
