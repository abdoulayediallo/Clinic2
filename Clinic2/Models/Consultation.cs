//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Clinic2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Consultation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Consultation()
        {
            this.Ordonnances = new HashSet<Ordonnance>();
            this.Vaccins = new HashSet<Vaccin>();
        }
    
        public int ID_Consultation { get; set; }
        public Nullable<System.DateTime> creatieDate { get; set; }
        public string createBy { get; set; }
        public Nullable<System.DateTime> changeDate { get; set; }
        public string changeBy { get; set; }
        public string motif { get; set; }
        public string note { get; set; }
        public Nullable<decimal> poids { get; set; }
        public Nullable<decimal> taille { get; set; }
        public Nullable<decimal> temperature { get; set; }
        public Nullable<decimal> systol { get; set; }
        public Nullable<decimal> diastol { get; set; }
        public string diagnostique { get; set; }
        public string maladie { get; set; }
        public string antecedent { get; set; }
        public Nullable<bool> statut { get; set; }
        public Nullable<int> ID_Patient { get; set; }
        public Nullable<int> ID_Staff { get; set; }
    
        public virtual Patient Patient { get; set; }
        public virtual Staff Staff { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ordonnance> Ordonnances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vaccin> Vaccins { get; set; }
    }
}
