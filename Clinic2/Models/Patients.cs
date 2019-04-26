using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic2.Models
{
    [MetadataType(typeof(PatientMetadata))]
    public partial class Patient
    {
        //partial void Initialize()
        //{
        //    this.adresse = new Adresse();
        //    this.consultation = new Consultation();
        //}

        public virtual Adresse adresse { get; set; }
        public virtual Consultation consultation { get; set; }
    }
}