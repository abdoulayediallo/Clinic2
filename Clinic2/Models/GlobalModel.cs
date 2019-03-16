using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic2.Models
{
    public class GlobalModel
    {
        public Patient patient { get; set; }
        public Staff staff { get; set; }
        public Adresse adresse { get; set; }
    }
}