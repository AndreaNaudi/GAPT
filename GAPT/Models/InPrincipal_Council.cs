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
    
    public partial class InPrincipal_Council
    {
        public int Id { get; set; }
        public int InPrincipalId { get; set; }
        public int CouncilDecisionId { get; set; }
    
        public virtual CouncilDecision CouncilDecision { get; set; }
        public virtual InPrincipal InPrincipal { get; set; }
    }
}
