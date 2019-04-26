using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic2.Models
{
    [MetadataType(typeof(StaffMetadata))]
    public partial class Staff
    {
        //partial void Initialize()
        //{
        //    this.adress = new Adresse();
        //}

        public DateTime creationDate { get; set; }
        public virtual Adresse adress { get; set; }

    }
}