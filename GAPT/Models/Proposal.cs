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
    
    public partial class Proposal
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> GeneralId { get; set; }
        public Nullable<int> ProgrammeRationaleId { get; set; }
        public Nullable<int> ExternalReviewId { get; set; }
        public Nullable<int> IncomeExpenditureId { get; set; }
        public Nullable<int> ApprovalId { get; set; }
        public Nullable<int> InPrincipalId { get; set; }
        public bool Submitted { get; set; }
        public Nullable<bool> InPrincipalApproved { get; set; }
        public bool RequiresModification { get; set; }
        public Nullable<bool> IsInEdit { get; set; }
        public string UserEditing { get; set; }
    
        public virtual Approval Approval { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual ExternalReview ExternalReview { get; set; }
        public virtual General General { get; set; }
        public virtual IncomeExpenditure IncomeExpenditure { get; set; }
        public virtual InPrincipal InPrincipal { get; set; }
        public virtual ProgrammeRationale ProgrammeRationale { get; set; }
    }
}