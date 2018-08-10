//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GAPT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PvcApproval
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PvcApproval()
        {
            this.InPrincipal_Pvc = new HashSet<InPrincipal_Pvc>();
        }
    
        public int Id { get; set; }
        public Nullable<bool> Selection { get; set; }
        public string SignedBy { get; set; }
        public Nullable<System.DateTime> SignedDate { get; set; }
        public string Upload { get; set; }
        public Nullable<bool> CouncilRef { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InPrincipal_Pvc> InPrincipal_Pvc { get; set; }
    }
}
