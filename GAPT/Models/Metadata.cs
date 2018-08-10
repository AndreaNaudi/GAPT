using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace GAPT.Models
{

    public class ProposalMetadata
    {
    }

    public class GeneralMetadata
    {
        [Display(Name = "A1: Proposed Title of Award")]
        public string Title { get; set; }
        [Display(Name = "A2: Level")]
        public Nullable<int> LevelId { get; set; }
        [Display(Name = "A3: Area/s of Study")]
        public string AreasStudy { get; set; }
        [Display(Name = "A7: Faculty/Institute/Centre")]
        public Nullable<int> FacultyId { get; set; }
        [Display(Name = "A10: Mode of Delivery")]
        public Nullable<int> DeliveryId { get; set; }
        [Display(Name = "A11: Duration in semesters (include summer periods, if any)")]
        public Nullable<int> DurationSemesters { get; set; }
        [Display(Name = "A12: Proposed Date of First Intake")]
        public string FirstDateIntake { get; set; }
        [Display(Name = "A13: Expected Student Numbers")]
        public Nullable<int> ExpectedStudents { get; set; }
        [Display(Name = "A14: Maximum Student Numbers (if applicable - please give reasons for capping)")]
        public Nullable<int> MaxStudents { get; set; }
        [Display(Name = "Reason for Capping")]
        public string CappingReason { get; set; }

    }

    public class ProgrammeRationaleMetadata
    {
    }

    public class RationaleMetadata
    {
        [Display(Name = "(a) Give the justification for the introduction of this programme of studies at the University of Malta.")]
        public string Justification { get; set; }
        [Display(Name = "(b) How does this programme fit into the existing plans and scope of the Department, Faculty, and/or the University strategy?")]
        public string Fit { get; set; }
        [Display(Name = "(c) How does this programme differ from existing programmes in the same or related areas of study offered by this University?")]
        public string Differences { get; set; }
    }

    public class DemandMetadata
    {
        [Display(Name = "(a) A brief description of the expected student profile, if applicable, and market demand is to be included in this section. You might wish to consider the following points:" +
            " Is the programme targeted at any particular type of student ?"+
            " Is there any evidence of employer support / demand ?" +
            " Have the major stakeholders been consulted / involved in the planning stage ? ")]
        public string Description { get; set; }

        [Display(Name = "(b) Will the demand for this programme be ongoing or for a specific period? (In the latter instance please indicate period)")]
        public string Period { get; set; }
    }

    public class ProgrammeOfStudyMetadata
    {
        [Display(Name = "(a) Subject Knowledge and Understanding")]
        public string KnowledgeUnderstanding { get; set; }
        [Display(Name = "(b) Intellectual Development")]
        public string IntellectualDevelopment { get; set; }
        [Display(Name = "(c) Key / Transferable Skills")]
        public string KeyTransferableSkills { get; set; }
        [Display(Name = "(d) Other skills relevant to employability and personal development")]
        public string OtherSkills { get; set; }
    }

    public class TentativePMetadata
    {
    }

    public class YearMetadata
    {
        [Display(Name ="Total ECTS")]
        public Nullable<int> TotalEcts { get; set; }
    }

    public class ApprovalMetadata
    {
    }

    public class ReviewerMetadata
    {
        [MaxLength(8, ErrorMessage = "Please enter an 8 digit number.")]
        [MinLength(8, ErrorMessage = "Please enter an 8 digit number.")]
        public string Telephone { get; set; }

        [Display(Name = "Affilation and Position")]
        public string Affiliation { get; set; }
    }

    public class Ref_DepartmentMetadata
    {
    }

    public class CommentMetadata
    {
    }

    public class Ref_UnitMetadata
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int DepartmentId { get; set; }
    }

    public class Ref_FacultyMetadata
    {

    }

    public class Department_GeneralMetadata
    {
        public int DepartmentId { get; set; }
        public int GeneralId { get; set; }
        public int Type { get; set; }

    }

    public class Proposer_GeneralMetadata
    {
        public string UserId { get; set; }
        public int GeneralId { get; set; }

    }

    public class Shared_GeneralMetadata
    {
        public string UserId { get; set; }
        public int GeneralId { get; set; }

    }

    public class Type_GeneralMetadata
    {
        public string TypeId { get; set; }
        public int GeneralId { get; set; }

    }

    public class Year_UnitMetadata
    {
    }

    public class ExternalReviewMetadata
    {
    }

    public class IncomeExpenditureMetadata
    {
    }

    public class StatementIEMetadata
    {
        [Required]
        public string Upload { get; set; }
    }

    public class StatementServMetadata
    {
        [Required]
        public string Reservations { get; set; }
    }

    public class PvcApprovalMetadata
    {
        [Required]
        public string Upload { get; set; }
    }


}