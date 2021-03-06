﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Clinic2.Models
{
    [MetadataType(typeof(ConsultationMetadata))]
    public partial class Consultation
    {
        //partial void Initialize()
        //{
        //    this.vaccin = new Vaccin();
        //    this.ordonnance = new Ordonnance();
        //}
        
        public virtual Ordonnance ordonnance { get; set; }
        public virtual Vaccin vaccin { get; set; }

    }
}