using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;
using Microsoft.AspNet.Identity;

namespace GAPT.Controllers
{
    [Authorize(Roles = RoleName.Lecturer + ", " + RoleName.Apqru + ", " + RoleName.Dean + ", " + RoleName.HoD)]
    public class GeneralController : Controller
    {
        private GaptDbContext _context;

        public GeneralController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: General
        public ActionResult Index()
        {
            return View();
        }

        [Route("General/New")]
        public ActionResult New() {
            var viewModel = GetFormViewModel(null);
            viewModel.General = new General();
            return View("Form", viewModel);
        }

        private GeneralFormViewModel GetFormViewModel(int? pid) {
            ApplicationDbContext db = new ApplicationDbContext();
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == pid);
            General general = null;
            if (pid != null) {
                general = proposal.General;
            }
            
            var levels = _context.Ref_Level.ToList();
            var faculties = _context.Ref_Faculty.ToList();
            var deliveries = _context.Ref_Delivery.ToList();

            var departments = _context.Ref_Department.ToList();
            List<SelectListItem> listSelectListInitDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListServDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListCollabDepts = new List<SelectListItem>();
            List<SelectListItem> listSelectListProposers = new List<SelectListItem>();
            List<SelectListItem> listSelectListShared = new List<SelectListItem>();

            // lecturers, deans, hods
            //string umStaff = "Select * from AspNetUsers users Inner Join AspNetUserRoles roles On users.Id = roles.UserId and roles.RoleId in ('2', '4', '5')";
            string umStaff = "Select Distinct * From AspNetUsers Where Id in(Select users.Id from AspNetUsers users Inner Join AspNetUserRoles roles On(users.Id = roles.UserId and roles.RoleId in ('2', '4', '5')))";
            var users = db.Database.SqlQuery<ApplicationUser>(umStaff).ToList();

            if (pid == null)
            {
                foreach (ApplicationUser user in users)
                {
                    SelectListItem selectList = new SelectListItem()
                    {
                        Text = user.Email,
                        Value = user.Id,
                    };
                    listSelectListProposers.Add(selectList);
                }
            }
            else {
                var sharedIds = _context.Database.SqlQuery<string>("Select UserId From Shared_General Where GeneralId = " + general.Id).ToList();

                string umStaffLeft = "Select Distinct * From AspNetUsers Where Id in(Select users.Id from AspNetUsers users Inner Join AspNetUserRoles roles On users.Id = roles.UserId and roles.RoleId in ('2', '4', '5') and users.Id Not In('" + string.Join("','", sharedIds.ToArray()) + "'))";

                var usersLeft = db.Database.SqlQuery<ApplicationUser>(umStaffLeft).ToList();
                foreach (ApplicationUser user in usersLeft)
                {
                    SelectListItem selectList = new SelectListItem()
                    {
                        Text = user.Email,
                        Value = user.Id,
                    };
                    listSelectListProposers.Add(selectList);
                }
            }
            

            if (pid != null) {
                var proposersIds = _context.Database.SqlQuery<string>("Select UserId From Proposer_General Where GeneralId = " + general.Id).ToList();

                string umStaffLeft = "Select Distinct * From AspNetUsers Where Id in(Select users.Id from AspNetUsers users Inner Join AspNetUserRoles roles On users.Id = roles.UserId and roles.RoleId in ('2', '4', '5') and users.Id Not In('" + string.Join("','", proposersIds.ToArray()) + "'))";

                var usersLeft = db.Database.SqlQuery<ApplicationUser>(umStaffLeft).ToList();
                foreach (ApplicationUser user in usersLeft)
                {
                    SelectListItem selectList = new SelectListItem()
                    {
                        Text = user.Email,
                        Value = user.Id,
                    };
                    listSelectListShared.Add(selectList);
                }
            }
            

            foreach (Ref_Department dept in departments)
            {
                var deptInDb = _context.Ref_Department.SingleOrDefault(m => m.Id == dept.Id);
                SelectListItem selectList = new SelectListItem()
                {
                    Text = dept.Name,
                    Value = dept.Id.ToString(),
                };

                if (pid != null)
                {
                    var dg = _context.Department_General.SingleOrDefault(m => m.DepartmentId == dept.Id && m.GeneralId == general.Id);
                    if (dg != null)
                    {
                        if (dg.Type == 1)
                        {
                            listSelectListInitDepts.Add(selectList);
                        }
                    }
                    else
                    {
                        listSelectListInitDepts.Add(selectList);
                    }
                }
                else {
                    listSelectListInitDepts.Add(selectList);
                }

                if (pid != null)
                {
                    var dg = _context.Department_General.SingleOrDefault(m => m.DepartmentId == dept.Id && m.GeneralId == general.Id);
                    if (dg != null)
                    {
                        if (dg.Type == 2)
                        {
                            listSelectListCollabDepts.Add(selectList);
                        }
                    }
                    else
                    {
                        listSelectListCollabDepts.Add(selectList);
                    }
                }
                else
                {
                    listSelectListCollabDepts.Add(selectList);
                }

                if (pid != null)
                {
                    var stm = dept.GetServStatement((int)pid);
                    if (stm != null)
                    {
                        if (stm.Selection == true)
                        {
                            continue;
                        }
                        else
                        {
                            listSelectListServDepts.Add(selectList);
                        }
                    }
                    else {
                        var dg = _context.Department_General.SingleOrDefault(m => m.DepartmentId == dept.Id && m.GeneralId == general.Id);
                        if (dg != null)
                        {
                            if (dg.Type == 3)
                            {
                                listSelectListServDepts.Add(selectList);
                            }
                        }
                        else
                        {
                            listSelectListServDepts.Add(selectList);
                        }
                        
                    }
                    
                }
                else {
                    listSelectListServDepts.Add(selectList);
                }
                
            }

            var viewModel = new GeneralFormViewModel
            {
                Levels = levels,
                Faculties = faculties,
                Deliveries = deliveries,
                InitDepts = listSelectListInitDepts,
                ServDepts = listSelectListServDepts,
                CollabDepts = listSelectListCollabDepts,
                Proposers = listSelectListProposers,
                Shared = listSelectListShared,
                SelectedTypes = new List<string>()
            };
            return viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(GeneralFormViewModel vm)
        {

            var general = vm.General;
            List<Department_General> removed = new List<Department_General>();
            List<Department_General> added = new List<Department_General>(); 
            if (!ModelState.IsValid)
            {
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        return Content(error.ErrorMessage);
                //    }
                //}
                return View("Form", general);
            }

            if (general.Id == 0)
            {
                Thread t = new Thread();
                _context.Threads.Add(t);
                general.Thread = t;
                _context.Generals.Add(general);
                var proposal = new Proposal();
                proposal.GeneralId = general.Id;
                proposal.CreatedOn = DateTime.Now;
                proposal.CreatedBy = User.Identity.GetUserId();
                proposal.IsInEdit = true;
                proposal.UserEditing = User.Identity.GetUserId();
                _context.Proposals.Add(proposal);
            }
            else
            {
                var proposal = _context.Proposals.SingleOrDefault(m => m.GeneralId == general.Id);
                if (proposal == null) {
                    return HttpNotFound();
                }
                if (!proposal.IsEditable(User.Identity.GetUserId()))
                {
                    return HttpNotFound();
                }
                if (proposal.Submitted) {
                    return Content("Proposal already submitted");
                }
                var changedLevel = false;

                var generalInDb = _context.Generals.SingleOrDefault(m => m.Id == general.Id);
                if (generalInDb.LevelId != general.LevelId) {
                    changedLevel = true;
                }
                
                
                generalInDb.Title = general.Title;
                generalInDb.LevelId = general.LevelId;
                generalInDb.Ref_Level = _context.Ref_Level.SingleOrDefault(m => m.Id == general.LevelId);
                generalInDb.AreasStudy = general.AreasStudy;
                generalInDb.FacultyId = general.FacultyId;
                generalInDb.Ref_Faculty = general.Ref_Faculty;
                generalInDb.DeliveryId = general.DeliveryId;
                generalInDb.Ref_Delivery = _context.Ref_Delivery.SingleOrDefault(m => m.Id == general.DeliveryId);
                generalInDb.DurationSemesters = general.DurationSemesters;
                generalInDb.FirstDateIntake = general.FirstDateIntake;
                generalInDb.ExpectedStudents = general.ExpectedStudents;
                if (general.MaxStudents == null)
                {
                    generalInDb.CappingReason = null;
                }
                else {
                    generalInDb.CappingReason = general.CappingReason;
                }
                generalInDb.MaxStudents = general.MaxStudents;

                //remove departments, to be added again later
                var toRemove = _context.Department_General.Where(m => m.GeneralId == general.Id).ToList();

                //remove proposers and shared, to be added again later
                var proposersToRemove = _context.Proposer_General.Where(m => m.GeneralId == general.Id).ToList();
                var sharedToRemove = _context.Shared_General.Where(m => m.GeneralId == general.Id).ToList();
                var typesToRemove = _context.Type_General.Where(m => m.GeneralId == general.Id).ToList();

                foreach (Department_General dg in toRemove.ToList()) {
                    if (dg.Type == 3)
                    {
                        //servicing
                        var dept = dg.Ref_Department;
                        //does not remove if approval accepted
                        var stm = dept.GetServStatement(proposal.Id);
                        if (stm != null)
                        {
                            if (stm.Selection == true) {
                                toRemove.Remove(dg);
                            }
                        }
                    }
                    
                }
                removed = toRemove;
                _context.Department_General.RemoveRange(toRemove);
                _context.Proposer_General.RemoveRange(proposersToRemove);
                _context.Shared_General.RemoveRange(sharedToRemove);
                _context.Type_General.RemoveRange(typesToRemove);

                _context.SaveChanges();

                //if level changed -> change existing year_units
                if (changedLevel) {
                    if (general.LevelId == 2)
                    {
                        if (proposal.ProgrammeRationale != null)
                        {
                            if (proposal.ProgrammeRationale.TentativeP != null)
                            {
                                var tentative = proposal.ProgrammeRationale.TentativeP;
                                var years = tentative.GetYears();
                                foreach (Year y in years)
                                {
                                    var yus = y.GetYearUnits();
                                    foreach (Year_Unit yu in yus)
                                    {
                                        var yuInDb = _context.Year_Unit.Single(m => m.Id == yu.Id);
                                        yuInDb.Compensating = 0;
                                        yuInDb.CompensatingReason = "Compensated passes are not applicable for PG courses.";
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        //switch reasons to null if they were PG default
                        if (proposal.ProgrammeRationale != null)
                        {
                            if (proposal.ProgrammeRationale.TentativeP != null)
                            {
                                var tentative = proposal.ProgrammeRationale.TentativeP;
                                var years = tentative.GetYears();
                                foreach (Year y in years)
                                {
                                    var yus = y.GetYearUnits();
                                    foreach (Year_Unit yu in yus)
                                    {
                                        var yuInDb = _context.Year_Unit.Single(m => m.Id == yu.Id);
                                        if (yuInDb.Compensating == 0 && yuInDb.CompensatingReason == "Compensated passes are not applicable for PG courses.")
                                        {
                                            yuInDb.CompensatingReason = null;

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
                
            _context.SaveChanges();
            
            if (vm.SelectedInitDepts != null)
            {
                foreach (string s in vm.SelectedInitDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 1);
                    var existing = _context.Department_General.Where(m => m.DepartmentId == department.Id && m.GeneralId == general.Id).ToList();
                    if (existing.Count == 0) {
                        _context.Department_General.Add(dg);
                        added.Add(dg);
                    }
                    
                    
                }
            }
            _context.SaveChanges();
            if (vm.SelectedCollabDepts != null)
            {
                foreach (string s in vm.SelectedCollabDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 2);
                    var existing = _context.Department_General.Where(m => m.DepartmentId == department.Id && m.GeneralId == general.Id).ToList();
                    if (existing.Count == 0)
                    {
                        _context.Department_General.Add(dg);
                        added.Add(dg);
                        
                    }
                }
            }
            _context.SaveChanges();
            if (vm.SelectedServDepts != null)
            {
                foreach (string s in vm.SelectedServDepts)
                {
                    var department = _context.Ref_Department.SingleOrDefault(m => m.Id.ToString() == s);
                    Department_General dg = new Department_General(department.Id, general.Id, 3);
                    var existing = _context.Department_General.Where(m => m.DepartmentId == department.Id && m.GeneralId == general.Id).ToList();
                    if (existing.Count == 0)
                    {
                        _context.Department_General.Add(dg);
                        added.Add(dg);
                    }
                }
            }

            if (vm.SelectedTypes != null)
            {
                foreach (string s in vm.SelectedTypes)
                {
                    var type = _context.Ref_Type.SingleOrDefault(m => m.Id.ToString() == s);
                    Type_General tg = new Type_General(type.Id, general.Id);
                    _context.Type_General.Add(tg);
                }
            }

            List<int> removedDepIds = new List<int>();
            foreach (Department_General dg in removed) {
                var f = false;
                foreach (Department_General adg in added) {
                    if ((dg.DepartmentId == adg.DepartmentId) && (dg.GeneralId == adg.GeneralId)) {
                        f = true;
                    }
                }
                if (f == true)
                {
                    continue;
                }
                else {
                    removedDepIds.Add(dg.DepartmentId);
                    //return Content("removed " + dg.DepartmentId);
                }
            }

            if (vm.SelectedProposers != null)
            {
                ApplicationDbContext adb = new ApplicationDbContext();
                foreach (string s in vm.SelectedProposers)
                {
                    var user = adb.Database.SqlQuery<ApplicationUser>("Select * from dbo.AspNetUsers Where Id = '" + s + "'").First();
                    Proposer_General pg = new Proposer_General(user.Id, general.Id);
                    _context.Proposer_General.Add(pg);
                }
            }

            if (vm.SelectedShared != null)
            {
                ApplicationDbContext adb = new ApplicationDbContext();
                foreach (string s in vm.SelectedShared)
                {
                    var user = adb.Database.SqlQuery<ApplicationUser>("Select * from dbo.AspNetUsers Where Id = '" + s + "'").First();
                    Shared_General sg = new Shared_General(user.Id, general.Id);
                    _context.Shared_General.Add(sg);
                }
            }

            _context.SaveChanges();
            var jump = Request["jump"];
            var prop = general.GetProposal();
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == prop.ProgrammeRationaleId);
            if (pr != null) {
                if (pr.TentativePsId != null) {
                    var tentativePs = pr.TentativeP;
                    var years = tentativePs.GetYears();
                    foreach (Year year in years)
                    {
                        var year_units = year.GetYearUnits();
                        foreach (Year_Unit yu in year_units)
                        {
                            if (removedDepIds.Contains(yu.GetUnit().DepartmentId))
                            {
                                var yuInDb = _context.Year_Unit.SingleOrDefault(m => m.Id == yu.Id);
                                _context.Year_Unit.Remove(yuInDb);
                            }
                        }
                    }
                }
            }
            
            
            _context.Database.ExecuteSqlCommand("Update dbo.GeneralHistory Set EditedBy = '" + User.Identity.GetUserId()+ "' Where GeneralId = " + general.Id + " and EditedBy is null");
            _context.SaveChanges();
            switch (jump) {
                case "0": {
                        // Save pressed -> return form
                        return RedirectToAction("Edit", "General", new { id = general.Id });
                    }
                case "1":
                    {
                        // Next pressed -> return next page
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = prop.Id });
                    }
                case "2":
                    {
                        // Save and exit
                        return RedirectToAction("CloseEdit", "Proposal", new { id = prop.Id });
                    }
                case "A":
                    {
                        // A pressed -> go to Section A
                        return RedirectToAction("Edit", "General", new { id = general.Id });
                    }
                case "B":
                    {
                        // B pressed -> go to Section B
                        return RedirectToAction("Jump", "ProgrammeRationale", new { id = prop.Id });
                    }
                case "C":
                    {
                        // C pressed -> go to Section C
                        return RedirectToAction("Jump", "ExternalReview", new { id = prop.Id });
                    }
                case "D":
                    {
                        // D pressed -> go to Section D
                        return RedirectToAction("Jump", "IncomeExpenditure", new { id = prop.Id });
                    }
                default:
                    {
                        return RedirectToAction("Index", "Proposal");
                    }
            }
            
        }
        
        public ActionResult Edit(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(c => c.Id == id);
            if (proposal == null || proposal.Submitted) {
                return HttpNotFound();
            }

            if (!proposal.IsEditable(User.Identity.GetUserId()))
            {
                return HttpNotFound();
            }

            var general = _context.Generals.SingleOrDefault(c => c.Id == proposal.GeneralId);

            if (general == null)
            {
                return HttpNotFound();
            }

            proposal.IsInEdit = true;
            proposal.UserEditing = User.Identity.GetUserId();
            _context.SaveChanges();

            // I -> Initiating
            string queryI = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 1 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedInit = _context.Database.SqlQuery<Ref_Department>(queryI).ToList();

            // C -> Collaborating
            string queryC = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 2 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedCollab = _context.Database.SqlQuery<Ref_Department>(queryC).ToList();

            // S -> Servicing
            string queryS = "SELECT * FROM dbo.Ref_Department JOIN dbo.Department_General ON dbo.Ref_Department.Id = dbo.Department_General.DepartmentId WHERE dbo.Department_General.Type = 3 and dbo.Department_General.GeneralId = " + general.Id + "; ";
            var selectedServ = _context.Database.SqlQuery<Ref_Department>(queryS).ToList();

            ApplicationDbContext adb = new ApplicationDbContext();
            // proposers
            List<string> selectedProposers = new List<string>();
            var pgs = _context.Proposer_General.Where(m => m.GeneralId == general.Id);
            foreach (Proposer_General pg in pgs)
            {
                var userEmail = adb.Database.SqlQuery<string>("Select Email From dbo.AspNetUsers Where Id = '" + pg.UserId + "'").First();
                selectedProposers.Add(userEmail);
            }

            // shared
            List<string> selectedShared = new List<string>();
            var sgs = _context.Shared_General.Where(m => m.GeneralId == general.Id);
            foreach (Shared_General sg in sgs)
            {
                var userEmail = adb.Database.SqlQuery<string>("Select Email From dbo.AspNetUsers Where Id = '" + sg.UserId + "'").First();
                selectedShared.Add(userEmail);
            }

            //types
            List<string> selectedTypes = new List<string>();
            var tgs = _context.Type_General.Where(m => m.GeneralId == general.Id);
            foreach (Type_General tg in tgs)
            {
                selectedTypes.Add(tg.TypeId.ToString());
            }

            List<string> selectedI = new List<string>();
            foreach (Ref_Department dept in selectedInit)
            {
                string s = dept.Name;
                selectedI.Add(s);
            }

            List<string> selectedC = new List<string>();
            foreach (Ref_Department dept in selectedCollab)
            {
                string s = dept.Name;
                selectedC.Add(s);
            }

            List<string> selectedS = new List<string>();
            foreach (Ref_Department dept in selectedServ)
            {
                string s = dept.Name;
                selectedS.Add(s);
            }

            var viewModel = GetFormViewModel(proposal.Id);
            viewModel.General = general;
            //viewModel.InitDepts = listSelectListDepts;
            viewModel.SelectedInitDepts = selectedI;
            //viewModel.CollabDepts = listSelectListDepts;
            viewModel.SelectedCollabDepts = selectedC;
            //viewModel.ServDepts = listSelectListDepts;
            viewModel.SelectedServDepts = selectedS;
            viewModel.SelectedProposers = selectedProposers;
            viewModel.SelectedShared = selectedShared;
            viewModel.SelectedTypes = selectedTypes;
            return View("Form", viewModel);
        }

    }
}