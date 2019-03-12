using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic2.Models
{
    public partial class Patient
    {
        partial void Initialize()
        {
            this.adresse = new Adresse();
            this.consultation = new Consultation();
        }

        public virtual Adresse adresse { get; set; }
        public virtual Consultation consultation { get; set; }
    }
}