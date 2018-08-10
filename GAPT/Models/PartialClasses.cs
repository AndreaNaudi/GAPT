using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace GAPT.Models
{
    
    [MetadataType(typeof(ProposalMetadata))]
    public partial class Proposal
    {
        public string GetCreatedBy()
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            var user = adb.Users.SingleOrDefault(m => m.Id == CreatedBy);
            return user.Email;
        }

        public string GetBeingUsedBy() {
            ApplicationDbContext adb = new ApplicationDbContext();
            var user = adb.Users.SingleOrDefault(m => m.Id == UserEditing);
            return user.Email;
        }

        public void Unlock() {
            UserEditing = null;
            IsInEdit = null;
        }

        public bool IsViewable(string userId, string email = null) {
            ApplicationDbContext adb = new ApplicationDbContext();
            GaptDbContext db = new GaptDbContext();

            if (userId == CreatedBy)
            {
                return true;
            }

            var pg = db.Proposer_General.SingleOrDefault(m => m.UserId == userId && m.GeneralId == GeneralId);
            if (pg != null)
            {
                return true;
            }

            var sg = db.Shared_General.SingleOrDefault(m => m.UserId == userId && m.GeneralId == GeneralId);
            if (sg != null)
            {
                return true;
            }

            var query = "Select Distinct r.Name From dbo.AspNetRoles r, dbo.AspNetUserRoles ur Where r.Id = ur.RoleId and ur.UserId = '" + userId + "'";            
            var roleNames = adb.Database.SqlQuery<string>(query).ToList();
            if (roleNames.Contains(RoleName.Lecturer) && roleNames.Contains(RoleName.Dean) && roleNames.Contains(RoleName.HoD)) {
                // if user is in these 3 roles, give permission where appropriate

                // give lecturer permission
                if (LecturerCanView(userId)) {
                    return true;
                }

                //give dean permission
                if (DeanCanView(userId)) {
                    return true;
                }

                // give hod permission
                if (HodCanView(userId)) {
                    return true;
                }

                //deny permission
                return false;

            }
            else if (roleNames.Contains(RoleName.Lecturer) && roleNames.Contains(RoleName.Dean))
            {
                // give lecturer permission
                if (LecturerCanView(userId))
                {
                    return true;
                }

                //give dean permission
                if (DeanCanView(userId))
                {
                    return true;
                }
                return false;
            }
            else if (roleNames.Contains(RoleName.Lecturer) && roleNames.Contains(RoleName.HoD))
            {
                // give lecturer permission
                if (LecturerCanView(userId))
                {
                    return true;
                }

                // give hod permission
                if (HodCanView(userId))
                {
                    return true;
                }
                return false;
            }
            else if (roleNames.Contains(RoleName.Dean) && roleNames.Contains(RoleName.HoD))
            {
                //give dean permission
                if (DeanCanView(userId))
                {
                    return true;
                }

                // give hod permission
                if (HodCanView(userId))
                {
                    return true;
                }
                return false;
            }
            else if (roleNames.Contains(RoleName.Lecturer))
            {
                return LecturerCanView(userId);
            }
            else if (roleNames.Contains(RoleName.Dean))
            {
                return DeanCanView(userId);

            }
            else if (roleNames.Contains(RoleName.HoD))
            {
                return HodCanView(userId);
            }
            else if (roleNames.Contains(RoleName.ER))
            {
                if (ExternalReviewId == null) {
                    return false;
                }
                // only give permission to external reviewers linked to the proposal
                var er = db.ExternalReviews.SingleOrDefault(m => m.Id == ExternalReviewId);
                var reviewers = er.GetReviewers();
                foreach (Reviewer reviewer in reviewers)
                {
                    if (reviewer.Email == email)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsEditable(string userId)
        {
            ApplicationDbContext adb = new ApplicationDbContext();
            GaptDbContext db = new GaptDbContext();

            if (IsInEdit == true)
            {
                if (UserEditing == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (userId == CreatedBy) {
                return true;
            }

            var pg = db.Proposer_General.SingleOrDefault(m => m.UserId == userId && m.GeneralId == GeneralId);
            if (pg != null) {
                return true;
            }

            var sg = db.Shared_General.SingleOrDefault(m => m.UserId == userId && m.GeneralId == GeneralId);
            if (sg != null)
            {
                return true;
            }

            var query = "Select Distinct r.Name From dbo.AspNetRoles r, dbo.AspNetUserRoles ur Where r.Id = ur.RoleId and ur.UserId = '" + userId + "'";
            var roleNames = adb.Database.SqlQuery<string>(query).ToList();
            //if (roleNames.Contains(RoleName.Lecturer))
            //{
            //    var pg = db.Proposer_General.SingleOrDefault(m => m.GeneralId == GeneralId && m.UserId == userId);
            //    if (pg != null)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            if (roleNames.Contains(RoleName.Apqru))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool LecturerCanView(string userId)
        {
            // return true if lecturer is one of the proposers
            GaptDbContext db = new GaptDbContext();
            var pg = db.Proposer_General.SingleOrDefault(m => m.GeneralId == GeneralId && m.UserId == userId);
            if (pg != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool HodCanView(string userId)
        {
            // if user is HoD of any linked department, return true
            var general = GetGeneral();
            if (general != null)
            {
                var allDepts = general.GetAllDepts();
                if (allDepts != null)
                {
                    foreach (Ref_Department dept in allDepts)
                    {
                        if (dept.HoD == userId)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool DeanCanView(string userId)
        {
            // if user is dean of the faculty or dean of the faculty of any dept, return true
            var general = GetGeneral();
            if (general != null)
            {
                if (general.FacultyId != null)
                {
                    var faculty = general.Ref_Faculty;
                    if (faculty.Dean == userId)
                    {
                        return true;
                    }
                }

                var allDepts = general.GetAllDepts();
                if (allDepts != null)
                {
                    foreach (Ref_Department dept in allDepts)
                    {
                        var facultyOfDept = dept.Ref_Faculty;
                        if (facultyOfDept.Dean == userId)
                        {
                            return true;
                        }
                    }
                }

                //var collabDepts = general.GetCollabDepts();
                //if (collabDepts != null)
                //{
                //    foreach (Ref_Department dept in collabDepts)
                //    {
                //        var facultyOfDept = dept.Ref_Faculty;
                //        if (facultyOfDept.Dean == userId)
                //        {
                //            return true;
                //        }
                //    }
                //}

            }
            return false;
        }

        public General GetGeneral()
        {
            GaptDbContext db = new GaptDbContext();
            var general = db.Generals.SingleOrDefault(t => t.Id == GeneralId);
            return general;
        }

        public PvcApproval GetPvcApproval()
        {
            GaptDbContext db = new GaptDbContext();
            var ipp = db.InPrincipal_Pvc.SingleOrDefault(m => m.InPrincipalId == InPrincipalId);
            if (ipp != null)
            {
                return ipp.PvcApproval;
            }
            else {
                return null;
            }
            
        }

        public SenateDecision GetSenateDecision()
        {
            GaptDbContext db = new GaptDbContext();
            var ips = db.InPrincipal_Senate.SingleOrDefault(m => m.InPrincipalId == InPrincipalId);
            if (ips != null)
            {
                return ips.SenateDecision;
            }
            else {
                return null;
            }
            
        }

        public CouncilDecision GetCouncilDecision()
        {
            GaptDbContext db = new GaptDbContext();
            var ipc = db.InPrincipal_Council.SingleOrDefault(m => m.InPrincipalId == InPrincipalId);
            if (ipc != null)
            {
                return ipc.CouncilDecision;
            }
            else {
                return null;
            }
            
        }

        //returns true if ALL faculties departments accepted
        public bool HasFacultyApproval() {
            if (ApprovalId == null) {
                return false;
            }

            GaptDbContext db = new GaptDbContext();
            var general = db.Generals.SingleOrDefault(t => t.Id == GeneralId);

            var faculty = general.Ref_Faculty;
            if (faculty == null) {
                return false;
            }
            var recommendation = faculty.GetRecommendation(Id);
            if (recommendation.Selection == false || recommendation.Selection == null) {
                return false;
            }

            var collabDepts = general.GetCollabDepts();
            foreach (Ref_Department dept in collabDepts) {
                var endorsement = dept.GetCollabEndorsement(Id);
                if (endorsement.Selection == false || endorsement.Selection == null) {
                    return false;
                }
            }

            var servDepts = general.GetServDepts();
            foreach (Ref_Department dept in servDepts)
            {
                var statement = dept.GetServStatement(Id);
                if (statement.Selection == false || statement.Selection == null)
                {
                    return false;
                }
            }

            return true;
        }

        //returns true if one of faculty/departments rejected
        public bool HasFacultyApprovalRejection()
        {
            if (ApprovalId == null)
            {
                return false;
            }

            GaptDbContext db = new GaptDbContext();
            var general = db.Generals.SingleOrDefault(t => t.Id == GeneralId);

            var faculty = general.Ref_Faculty;
            var recommendation = faculty.GetRecommendation(Id);
            if (recommendation.Selection == false)
            {
                return true;
            }

            var collabDepts = general.GetCollabDepts();
            foreach (Ref_Department dept in collabDepts)
            {
                var endorsement = dept.GetCollabEndorsement(Id);
                if (endorsement.Selection == false)
                {
                    return true;
                }
            }

            var servDepts = general.GetServDepts();
            foreach (Ref_Department dept in servDepts)
            {
                var statement = dept.GetServStatement(Id);
                if (statement.Selection == false)
                {
                    return true;
                }
            }

            return false;
        }

        //returns true if one of  pvc/senate/council rejected
        public bool HasInPrincipleRejection()
        {
            if (InPrincipalId == null)
            {
                return false;
            }

            GaptDbContext db = new GaptDbContext();
            var general = db.Generals.SingleOrDefault(t => t.Id == GeneralId);
            var pvc = GetPvcApproval();
            var senate = GetSenateDecision();
            var council = GetCouncilDecision();
            
            if (pvc != null) {
                if (pvc.Selection == false)
                {
                    return true;
                }
            }
            
            if (senate != null)
            {
                if (senate.Selection == false && senate.SignedBy != null)
                {
                    return true;
                }
            }
            
            if (council != null) {
                if (council.Selection == false && council.SignedBy != null)
                {
                    return true;
                }
            }
            

            return false;
        }

        public ProgrammeRationale GetProgrammeRationale()
        {
            GaptDbContext db = new GaptDbContext();
            var pr = db.ProgrammeRationales.SingleOrDefault(t => t.Id == ProgrammeRationaleId);
            return pr;
        }

    }

    [MetadataType(typeof(GeneralMetadata))]
    public partial class General
    {
        public List<Comment> GetComments() {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs) {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public string GetErrorMessage() {
            string s = null;
            if (String.IsNullOrWhiteSpace(Title))
            {
                s = "Section A : Title";
            }
            else if (LevelId == null)
            {
                s = "Section A : Level";
            }
            else if (String.IsNullOrWhiteSpace(AreasStudy))
            {
                s = "Section A : Area/s of Study";
            }
            else if (FacultyId == null)
            {
                s = "Section A : Faculty";
            }
            else if (DeliveryId == null)
            {
                s = "Section A : Mode of Delivery";
            }
            else if (DurationSemesters == null || DurationSemesters <= 0)
            {
                s = "Section A : Duration in Semesters";
            }
            else if (String.IsNullOrWhiteSpace(FirstDateIntake))
            {
                s = "Section A : Proposed Date of First Intake";
            }
            else if (ExpectedStudents == null || ExpectedStudents <= 0)
            {
                s = "Section A : Expected Student Numbers";
            }
            else if (MaxStudents <= 0)
            {
                s = "Section A : Maximum Student Numbers";
            }
            else if (MaxStudents > 0 && String.IsNullOrWhiteSpace(CappingReason))
            {
                s = "Section A : Maximum Student Capping Reason";
            }
            else
            {
                if (GetInitDepts().Count == 0)
                {
                    s = "Section A : Initiating Department";
                }
                //else if (GetCollabDepts().Count == 0) {
                //    s = "Section A : Collaborating Department";
                //}
                //else if (GetServDepts().Count == 0)
                //{
                //    s = "Section A : Servicing Department/s";
                //}
                else if (GetProposers().Count == 0)
                {
                    s = "Section A : Proposer/s";
                }
                else if (GetTypes().Count == 0)
                {
                    s = "Section A : Programme Type/s";
                }
                //repeat for proposers and programme types
            }
            return s;
        }

        public Proposal GetProposal() {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.GeneralId == Id);
            return proposal;
        }

        public string GetLevelName() {
            GaptDbContext db = new GaptDbContext();
            if (LevelId == null) {
                return null;
            }
            var level = db.Ref_Level.SingleOrDefault(t => t.Id == LevelId);
            return level.Name;
        }

        public string GetDeliveryName()
        {
            GaptDbContext db = new GaptDbContext();
            if (DeliveryId == null)
            {
                return null;
            }
            var mode = db.Ref_Delivery.SingleOrDefault(t => t.Id == DeliveryId);
            return mode.Name;
        }

        public string GetFacultyName() {
            GaptDbContext db = new GaptDbContext();
            if (FacultyId == null)
            {
                return null;
            }
            var faculty = db.Ref_Faculty.SingleOrDefault(t => t.Id == FacultyId);
            return faculty.Name;
        }

        public List<Ref_Department> GetAllDepts()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id).ToList();
            if (ids == null)
            {
                return null;
            }
            var depts = new List<Ref_Department>();
            foreach (int id in ids)
            {
                var dept = db.Ref_Department.SingleOrDefault(m => m.Id == id);
                depts.Add(dept);
            }
            return depts;
        }

        public List<Ref_Department> GetInitDepts()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 1").ToList();
            if (ids == null)
            {
                return null;
            }
            var depts = new List<Ref_Department>();
            foreach (int id in ids) {
                var dept = db.Ref_Department.SingleOrDefault(m => m.Id == id);
                depts.Add(dept);
            }
            return depts;
        }

        public List<int> GetInitDeptsIds()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 1").ToList();
            if (ids == null)
            {
                return null;
            }
            return ids;
        }

        public List<Ref_Department> GetCollabDepts()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 2").ToList();
            if (ids == null)
            {
                return null;
            }
            var depts = new List<Ref_Department>();
            foreach (int id in ids)
            {
                var dept = db.Ref_Department.SingleOrDefault(m => m.Id == id);
                depts.Add(dept);
            }
            return depts;
        }

        public List<int> GetCollabDeptsIds()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 2").ToList();
            if (ids == null)
            {
                return null;
            }
            return ids;
        }

        public List<Ref_Department> GetServDepts()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 3").ToList();
            if (ids == null)
            {
                return null;
            }
            var depts = new List<Ref_Department>();
            foreach (int id in ids)
            {
                var dept = db.Ref_Department.SingleOrDefault(m => m.Id == id);
                depts.Add(dept);
            }
            return depts;
        }

        public List<Ref_Type> GetTypes()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select TypeId From dbo.Type_General Where GeneralId = " + Id).ToList();
            if (ids == null)
            {
                return null;
            }
            var types = new List<Ref_Type>();
            foreach (int id in ids)
            {
                var type = db.Ref_Type.SingleOrDefault(m => m.Id == id);
                types.Add(type);
            }
            return types;
        }

        public List<int> GetServDeptsIds()
        {
            GaptDbContext db = new GaptDbContext();
            var ids = db.Database.SqlQuery<int>("Select DepartmentId From dbo.Department_General Where GeneralId = " + Id + " and Type = 3").ToList();
            if (ids == null)
            {
                return null;
            }
            return ids;
        }

        public List<ApplicationUser> GetProposers()
        {
            GaptDbContext db = new GaptDbContext();
            ApplicationDbContext adb = new ApplicationDbContext();
            var ids = db.Database.SqlQuery<string>("Select UserId From dbo.Proposer_General Where GeneralId = '" + Id + "'").ToList();
            if (ids == null)
            {
                return null;
            }
            var users = new List<ApplicationUser>();
            foreach (string id in ids)
            {
                var user = adb.Database.SqlQuery<ApplicationUser>("Select * from AspNetUsers Where Id = '" + id + "'").First();
                users.Add(user);
            }
            return users;
        }

        public List<ApplicationUser> GetShared()
        {
            GaptDbContext db = new GaptDbContext();
            ApplicationDbContext adb = new ApplicationDbContext();
            var ids = db.Database.SqlQuery<string>("Select UserId From dbo.Shared_General Where GeneralId = '" + Id + "'").ToList();
            if (ids == null)
            {
                return null;
            }
            var users = new List<ApplicationUser>();
            foreach (string id in ids)
            {
                var user = adb.Database.SqlQuery<ApplicationUser>("Select * from AspNetUsers Where Id = '" + id + "'").First();
                users.Add(user);
            }
            return users;
        }
    }

    [MetadataType(typeof(ProgrammeRationaleMetadata))]
    public partial class ProgrammeRationale
    {
        public string GetErrorMessage()
        {
            string s = null;
            if (RationaleId == null)
            {
                s = "Section B : Rationale section not filled in";
            }
            else if (DemandId == null)
            {
                s = "Section B : Demand section not filled in";
            }
            else if (PsId == null)
            {
                s = "Section B : Programme of Study section not filled in";
            }
            else if (TentativePsId == null)
            {
                s = "Section B : Tenatative Programme of Study section not filled in";
            }
            else {
                if (Rationale.GetErrorMessage() != null) {
                    s = Rationale.GetErrorMessage();
                }
                else if (Demand.GetErrorMessage() != null)
                {
                    s = Demand.GetErrorMessage();
                }
                else if (ProgrammeOfStudy.GetErrorMessage() != null)
                {
                    s = ProgrammeOfStudy.GetErrorMessage();
                }
                else if (TentativeP.GetErrorMessage() != null)
                {
                    s = TentativeP.GetErrorMessage();
                }
            }
            return s;
        }
    }

    [MetadataType(typeof(RationaleMetadata))]
    public partial class Rationale
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public string GetErrorMessage()
        {
            string s = null;
            if (String.IsNullOrWhiteSpace(Justification))
            {
                s = "Section B Rationale : Justification";
            }
            else if (String.IsNullOrWhiteSpace(Fit))
            {
                s = "Section B Rationale : Fit";
            }
            else if (String.IsNullOrWhiteSpace(Differences))
            {
                s = "Section B Rationale : Differences";
            }
            return s;
        }
    }

    [MetadataType(typeof(DemandMetadata))]
    public partial class Demand
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public string GetErrorMessage()
        {
            string s = null;
            if (String.IsNullOrWhiteSpace(Description))
            {
                s = "Section B Demand : Description";
            }
            else if (String.IsNullOrWhiteSpace(Period))
            {
                s = "Section B Demand : Period";
            }
            return s;
        }
    }

    [MetadataType(typeof(ProgrammeOfStudyMetadata))]
    public partial class ProgrammeOfStudy
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public string GetErrorMessage()
        {
            string s = null;
            if (String.IsNullOrWhiteSpace(KnowledgeUnderstanding))
            {
                s = "Section B Programme of Study : Knowledge and Understanding";
            }
            else if (String.IsNullOrWhiteSpace(IntellectualDevelopment))
            {
                s = "Section B Programme of Study : Intellectual Development";
            }
            else if (String.IsNullOrWhiteSpace(KeyTransferableSkills))
            {
                s = "Section B Programme of Study : Key/Transferable Skills";
            }
            else if (String.IsNullOrWhiteSpace(OtherSkills))
            {
                s = "Section B Programme of Study : Other Skills";
            }
            return s;
        }
    }

    [MetadataType(typeof(TentativePMetadata))]
    public partial class TentativeP
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public List<Year> GetYears() {
            var db = new GaptDbContext();
            return db.Years.Where(m => m.TentativePsId == Id).OrderBy(m => m.YearNo).ToList();
        }

        public string GetErrorMessage()
        {
            var years = GetYears();
            string s = null;
            if (years.Count() == 0)
            {
                s = "Section B Tentative Programme of Study : No years added";
            }
            else
            {
                foreach (Year year in years) {
                    if (year.GetErrorMessage() != null)
                    {
                        s = year.GetErrorMessage();
                        break;
                    }
                }
            }
            return s;
        }
    }

    [MetadataType(typeof(YearMetadata))]
    public partial class Year
    {
        public List<Year_Unit> GetYearUnits()
        {
            var db = new GaptDbContext();
            return db.Year_Unit.Where(m => m.YearId == Id).ToList();
        }

        public string GetErrorMessage()
        {
            string s = null;
            if (TotalEcts == null || TotalEcts < 0)
            {
                s = "Section B Tentative Programme of Study : Year " + YearNo + " Total ECTs";
            }
            else
            {
                var units = GetYearUnits();
                if (units.Count() == 0)
                {
                    s = "Section B Tentative Programme of Study : Year " + YearNo + " No units added";
                }
                else
                {
                    foreach (Year_Unit yu in units)
                    {
                        if (yu.GetErrorMessage() != null)
                        {
                            s = yu.GetErrorMessage();
                            break;
                        }
                    }
                }
                
            }
            return s;
        }
    }

    [MetadataType(typeof(Ref_FacultyMetadata))]
    public partial class Ref_Faculty
    {
        public RecommendationFic GetRecommendation(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var ars = db.Approval_Recommendation.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (ars == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Recommendation ar in ars)
                {
                    var recommendation = db.RecommendationFics.SingleOrDefault(m => m.Id == ar.RecommendationId && m.FacultyId == Id);
                    if (recommendation != null)
                    {
                        return recommendation;
                    }
                }
                return null;
            }
        }
    }

    [MetadataType(typeof(Ref_UnitMetadata))]
    public partial class Ref_Unit
    {
    }

    [MetadataType(typeof(Ref_DepartmentMetadata))]
    public partial class Ref_Department
    {
        public EndorsementCollab GetCollabEndorsement(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var aes = db.Approval_Endorsement.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (aes == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Endorsement ae in aes) {
                    var endCollab = db.EndorsementCollabs.SingleOrDefault(m => m.Id == ae.EndorsementId && m.DepartmentId == Id);
                    if (endCollab != null) {
                        return endCollab;
                    }
                }
                return null;
            }
        }

        public StatementServ GetServStatement(int pid)
        {
            GaptDbContext db = new GaptDbContext();
            var proposal = db.Proposals.SingleOrDefault(t => t.Id == pid);

            var apps = db.Approval_Statement.Where(m => m.ApprovalId == proposal.ApprovalId).ToList();

            if (apps == null)
            {
                return null;
            }
            else
            {
                foreach (Approval_Statement app in apps)
                {
                    var stmServ = db.StatementServs.SingleOrDefault(m => m.Id == app.StatementId && m.DepartmentId == Id);
                    if (stmServ != null)
                    {
                        return stmServ;
                    }
                }
                return null;
            }
        }


    }

    [MetadataType(typeof(Proposer_GeneralMetadata))]
    public partial class Proposer_General
    {
        public Proposer_General(string UserId, int GeneralId)
        {
            this.UserId = UserId;
            this.GeneralId = GeneralId;
        }

        public Proposer_General()
        {
        }

    }

    [MetadataType(typeof(Shared_GeneralMetadata))]
    public partial class Shared_General
    {
        public Shared_General(string UserId, int GeneralId)
        {
            this.UserId = UserId;
            this.GeneralId = GeneralId;
        }

        public Shared_General()
        {
        }

    }

    [MetadataType(typeof(Type_GeneralMetadata))]
    public partial class Type_General
    {
        public Type_General(int TypeId, int GeneralId)
        {
            this.TypeId = TypeId;
            this.GeneralId = GeneralId;
        }

        public Type_General()
        {
        }

    }

    [MetadataType(typeof(Department_GeneralMetadata))]
    public partial class Department_General
    {
        public Department_General(int DepartmentId, int GeneralId, int Type)
        {
            this.DepartmentId = DepartmentId;
            this.GeneralId = GeneralId;
            this.Type = Type;
        }

        public Department_General()
        {
        }

    }

    [MetadataType(typeof(ApprovalMetadata))]
    public partial class Approval
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

    }

    [MetadataType(typeof(CommentMetadata))]
    public partial class Comment
    {
        public int GetReplyCount(){
            GaptDbContext db = new GaptDbContext();
            var replies = db.Comments.Where(m => m.ReplyTo == Id).ToList();
            return replies.Count();
        }
    }

    [MetadataType(typeof(Year_UnitMetadata))]
    public partial class Year_Unit
    {
        public Ref_Unit GetUnit() {
            GaptDbContext db = new GaptDbContext();
            var unit = db.Ref_Unit.SingleOrDefault(m => m.Id == UnitId);
            return unit;
        }
        public string GetLecturerEmail()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (Lecturer != null) {
                var user = db.Users.Single(m => m.Id == Lecturer);
                return user.Email;
            }
            return "TBA";
        }
        public Year GetYear()
        {
            GaptDbContext db = new GaptDbContext();
            var year = db.Years.SingleOrDefault(m => m.Id == YearId);
            return year;
        }
        public string GetPeriodName()
        {
            if (Period == null)
            {
                return null;
            }
            switch (Period) {
                case 1:
                    {
                        return "Sem 1";
                    }
                case 2:
                    {
                        return "Sem 2";
                    }
                case 3:
                    {
                        return "YR";
                    }
                case 4:
                    {
                        return "SP";
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public string GetCoeName()
        {
            if (Coe == null)
            {
                return null;
            }
            switch (Coe)
            {
                case 1:
                    {
                        return "Compulsory";
                    }
                case 2:
                    {
                        return "Optional";
                    }
                case 3:
                    {
                        return "Elective";
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public string GetCompensatingName()
        {
            if (Compensating == null)
            {
                return null;
            }
            switch (Compensating)
            {
                case 1:
                    {
                        return "Yes";
                    }
                case 0:
                    {
                        return "No";
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public string GetErrorMessage()
        {
            GaptDbContext db = new GaptDbContext();
            var year = GetYear();
            var tentativePsId = year.TentativePsId;
            var pr = db.ProgrammeRationales.SingleOrDefault(m => m.TentativePsId == tentativePsId);
            var proposal = db.Proposals.SingleOrDefault(m => m.ProgrammeRationaleId == pr.Id);
            string s = null;
            if (Ects == null || Ects < 1)
            {
                s = "Section B Tentative Programme of Study : Year " + GetYear().YearNo + " Unit " + GetUnit().Code + " ECTs";
            }

            else if (Compensating == 0 && proposal.GetGeneral().LevelId == 1)
            {
                if (String.IsNullOrWhiteSpace(CompensatingReason)) {
                    s = "Section B Tentative Programme of Study : Year " + GetYear().YearNo + " Unit " + GetUnit().Code + " NC Reason";
                }
                
            }
            return s;
        }
    }

    [MetadataType(typeof(ExternalReviewMetadata))]
    public partial class ExternalReview
    {
        public List<Comment> GetComments()
        {
            GaptDbContext db = new GaptDbContext();
            var tcs = db.Thread_Comment.Where(m => m.ThreadId == ThreadId).ToList();
            List<Comment> comments = new List<Comment>();
            foreach (Thread_Comment tc in tcs)
            {
                var comment = db.Comments.SingleOrDefault(m => m.Id == tc.CommentId);
                comments.Add(comment);
            }
            return comments;
        }

        public List<Reviewer> GetReviewers()
        {
            GaptDbContext db = new GaptDbContext();
            //string query = "Select * from dbo.Reviewer rev Join dbo.ExternalReview_Reviewer err on err.ExternalReviewId =" + Id + " Order by rev.Name";
            //var reviewers = db.Database.SqlQuery<Reviewer>(query).ToList();
            //return reviewers;
            var reviewerIds = db.Database.SqlQuery<int>("Select ReviewerId From dbo.ExternalReview_Reviewer Where ExternalReviewId = " + Id).ToList();
            List<Reviewer> reviewers = new List<Reviewer>();

            foreach (int id in reviewerIds)
            {
                var reviewer = db.Reviewers.SingleOrDefault(m => m.Id == id);
                reviewers.Add(reviewer);
            }
            var sorted = reviewers.OrderBy(m => m.Name).ToList();
            return sorted;
        }

        public string GetErrorMessage()
        {
            var reviewers = GetReviewers();
            string s = null;
            if (reviewers.Count() < 3)
            {
                s = "Section C : Minimum 3 reviewers required";
            }
            else
            {
                foreach (Reviewer reviewer in reviewers)
                {
                    if (reviewer.GetErrorMessage() != null)
                    {
                        s = reviewer.GetErrorMessage();
                        break;
                    }
                }
            }
            return s;
        }

    }

    [MetadataType(typeof(ReviewerMetadata))]
    public partial class Reviewer
    {
        public string GetErrorMessage()
        {
            string s = null;
            if (String.IsNullOrWhiteSpace(Name))
            {
                s = "Section C : An external reviewer has no name";
            }
            else if (String.IsNullOrWhiteSpace(Affiliation))
            {
                s = "Section C : " + Name + " has no affiliation";
            }
            else if (String.IsNullOrWhiteSpace(Address))
            {
                s = "Section C : " + Name + " has no address";
            }
            else if (String.IsNullOrWhiteSpace(Email))
            {
                s = "Section C : " + Name + " has no email";
            }
            else
            {
                try
                {
                    int tel = Convert.ToInt32(Telephone);
                    if (tel < 0) {
                        s = "Section C : " + Name + " invalid telephone number";
                    } else if (String.IsNullOrWhiteSpace(Telephone)) {
                        s = "Section C : " + Name + " has no telephone number";
                    }
                }
                catch {
                    s = "Section C : " + Name + " invalid telephone number";
                }
            }
            return s;
        }
    }

    [MetadataType(typeof(IncomeExpenditureMetadata))]
    public partial class IncomeExpenditure
    {
        public List<StatementIE> GetStatements()
        {
            GaptDbContext db = new GaptDbContext();
            var statementsIds = db.Database.SqlQuery<int>("Select StatementId From dbo.IncomeExpenditure_StatementIE Where IncomeExpenditureId = " + Id).ToList();
            List<StatementIE> statements = new List<StatementIE>();

            foreach (int id in statementsIds)
            {
                var statement = db.StatementIEs.SingleOrDefault(m => m.Id == id);
                statements.Add(statement);
            }
            return statements;
        }

        public string GetErrorMessage()
        {
            var statements = GetStatements();
            string s = null;
            if (statements.Count() == 0)
            {
                s = "Section D : No statements added";
            }
            else
            {
                foreach (StatementIE statement in statements)
                {
                    if (statement.GetErrorMessage() != null)
                    {
                        s = statement.GetErrorMessage();
                        break;
                    }
                }
            }
            return s;
        }

    }

    [MetadataType(typeof(StatementIEMetadata))]
    public partial class StatementIE
    {
        public string GetErrorMessage()
        {
            string s = null;
            if (String.IsNullOrWhiteSpace(Upload))
            {
                s = "Section D : Invalid uploaded statement";
            }
            return s;
        }
    }

    [MetadataType(typeof(StatementServMetadata))]
    public partial class StatementServ
    {
    }

    [MetadataType(typeof(PvcApprovalMetadata))]
    public partial class PvcApproval
    {
    }
}