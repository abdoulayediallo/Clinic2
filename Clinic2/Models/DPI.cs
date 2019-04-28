using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic2.Models
{
    public class DPI
    {
        public Patient patient { get; set; }
        public List<Consultation> consultation { get; set; }
    }
}