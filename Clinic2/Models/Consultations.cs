using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clinic2.Models
{
    public partial class Consultation
    {
        partial void Initialize()
        {
            this.vaccin = new Vaccin();
            this.ordonnance = new Ordonnance();
        }

        
        public Nullable<int> ID_Ordonnance { get; set; }
        public Nullable<int> ID_Vaccin { get; set; }

        public virtual Ordonnance ordonnance { get; set; }
        public virtual Vaccin vaccin { get; set; }

    }
}