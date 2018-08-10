using GAPT.Models;
using GAPT.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    [Authorize(Roles = RoleName.Lecturer + ", " + RoleName.Apqru + ", " + RoleName.Dean + ", " + RoleName.HoD)]
    public class YearController : Controller
    {
        private GaptDbContext _context;

        public YearController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public ActionResult New(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (!proposal.IsEditable(User.Identity.GetUserId()))
            {
                return HttpNotFound();
            }
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
            Year year = new Year();
            int? maxYearNo = _context.Years.Where(m => m.TentativePsId == pr.TentativePsId).Max(u => (int?)u.YearNo);
            int? yearNo; ;
            if (maxYearNo == null)
            {
                yearNo = 1;
            }
            else {
                yearNo = maxYearNo + 1;
            }
            
            year.YearNo = yearNo;

            var allUnits = _context.Database.SqlQuery<Ref_Unit>("Select * from Ref_Unit where DepartmentId In(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = " + proposal.GeneralId + ")").ToList();
            foreach (Ref_Unit unit in allUnits.ToList())
            {
                // do not show units of servicing departments which already accepted an approval
                var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == unit.DepartmentId);
                var stm = dept.GetServStatement(proposal.Id);
                if (stm != null)
                {
                    if (stm.Selection == true)
                    {
                        allUnits.Remove(unit);
                    }
                }
            }

            ApplicationDbContext adb = new ApplicationDbContext();
            string umStaff = "Select distinct * from dbo.AspNetUsers Where Id in(Select Id from dbo.AspNetUsers users Inner Join AspNetUserRoles roles On users.Id = roles.UserId and roles.RoleId in ('2', '4', '5'))";
            var lecturers = adb.Database.SqlQuery<ApplicationUser>(umStaff).ToList();

            var viewModel = new YearFormViewModel
            {
                Proposal = proposal,
                Lecturers = lecturers,
                Year = year,
                AllUnits = allUnits
            };
            return View("Form", viewModel);
        }
        
        public ActionResult Edit(int id)
        {
            var year = _context.Years.SingleOrDefault(m => m.Id == id);
            if (year == null)
            {
                return HttpNotFound();
            }
            var tentative = _context.TentativePs.SingleOrDefault(m => m.Id == year.TentativePsId);
            var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.TentativePsId == tentative.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.ProgrammeRationaleId == pr.Id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (!proposal.IsEditable(User.Identity.GetUserId()))
            {
                return HttpNotFound();
            }
            string selectedQuery = "SELECT * FROM Year_Unit WHERE YearId = " + year.Id;
            var selectedUnits = _context.Database.SqlQuery<Year_Unit>(selectedQuery).ToList();
            string unselectedQuery = "SELECT * FROM Ref_Unit WHERE Id not in (SELECT UnitId FROM Year_Unit WHERE YearId = "+year.Id+") AND DepartmentId IN(Select distinct DepartmentId From dbo.Department_General WHERE GeneralId = "+ proposal.GeneralId+ ")";
            var unselectedUnits = _context.Database.SqlQuery<Ref_Unit>(unselectedQuery).ToList();

            //do not show units for which their department (servicing) has accepted an approval 
            foreach (Ref_Unit unit in unselectedUnits.ToList())
            {
                var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == unit.DepartmentId);
                var stm = dept.GetServStatement(proposal.Id);
                if (stm != null)
                {
                    if (stm.Selection == true)
                    {
                        unselectedUnits.Remove(unit);
                    }
                }
            }
            foreach (Year_Unit yu in selectedUnits.ToList()) {
                var unit = _context.Ref_Unit.SingleOrDefault(m => m.Id == yu.UnitId);
                yu.Ref_Unit = unit;
                var dept = _context.Ref_Department.SingleOrDefault(m => m.Id == unit.DepartmentId);
                unit.Ref_Department = dept;
            }

            List<int> AvailableYearNos = new List<int>();
            var takenYearNos = _context.Database.SqlQuery<int>("Select YearNo From dbo.Year Where TentativePsId = " + pr.TentativePsId);
            var maxYearNo = _context.Database.SqlQuery<int>("Select Max(YearNo) From dbo.Year Where TentativePsId = " + pr.TentativePsId).First() + 5;
            for (int i = 1; i <= maxYearNo; i++) {
                if (!takenYearNos.Contains(i)) {
                    AvailableYearNos.Add(i);
                }
            }

            ApplicationDbContext adb = new ApplicationDbContext();
            string umStaff = "Select distinct * from dbo.AspNetUsers Where Id in(Select Id from dbo.AspNetUsers users Inner Join AspNetUserRoles roles On users.Id = roles.UserId and roles.RoleId in ('2', '4', '5'))";
            var lecturers = adb.Database.SqlQuery<ApplicationUser>(umStaff).ToList();

            var viewModel = new YearFormViewModel
            {
                Year = year,
                Proposal = proposal,
                SelectedUnits = selectedUnits,
                UnselectedUnits = unselectedUnits,
                AvailableYearNos = AvailableYearNos,
                Lecturers = lecturers
            };
            return View("Form", viewModel);
            


        }

        // GET: Year
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(YearFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (!proposal.IsEditable(User.Identity.GetUserId()))
            {
                return HttpNotFound();
            }
            var year = vm.Year;
            if (!ModelState.IsValid)
            {
                return View("Form", year);
            }

            if (year.Id == 0)
            {
                _context.Years.Add(year);
                var pr = _context.ProgrammeRationales.SingleOrDefault(m => m.Id == proposal.ProgrammeRationaleId);
                year.TentativeP = pr.TentativeP;
                year.TentativePsId = pr.TentativePsId;
                
            }
            else
            {
                var yearInDb = _context.Years.SingleOrDefault(m => m.Id == year.Id);
                yearInDb.TotalEcts = year.TotalEcts;

                if (year.YearNo != null) {
                    var maxYearNo = _context.Database.SqlQuery<int>("Select Max(YearNo) From dbo.Year Where TentativePsId = " + yearInDb.TentativePsId).First() + 5;
                    if (year.YearNo <= maxYearNo)
                    {
                        // check that no year exists with the same number
                        var takenYearNos = _context.Database.SqlQuery<int>("Select YearNo From dbo.Year Where TentativePsId = " + yearInDb.TentativePsId).ToList();
                        
                        if (!takenYearNos.Contains((int)year.YearNo))
                        {
                            yearInDb.YearNo = year.YearNo;
                        }
                        else
                        {
                            return HttpNotFound();
                        }
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }

                // remove exsting Year_Units where TentativePsId == tentative.Id
                var toRemove = _context.Year_Unit.Where(m => m.YearId == year.Id).ToList();
                foreach (Year_Unit yu in toRemove.ToList())
                {
                    var unit = _context.Ref_Unit.SingleOrDefault(m => m.Id == yu.UnitId);
                    var dept = unit.Ref_Department;
                    var stm = dept.GetServStatement(proposal.Id);
                    if (stm != null) {
                        if (stm.Selection == true) {
                            toRemove.Remove(yu);
                        }
                        
                    }

                }

                _context.Year_Unit.RemoveRange(toRemove);

                //string queryYu = "DELETE FROM dbo.Year_Unit WHERE YearId = " + year.Id;
                //_context.Database.ExecuteSqlCommand(queryYu);

                
            }
            foreach (string name in Request.Form.AllKeys)
            {

                try
                {
                    int unitId = Convert.ToInt32(name);
                    Year_Unit yu = new Year_Unit();
                    yu.YearId = year.Id;
                    yu.Year = _context.Years.SingleOrDefault(m => m.Id == year.Id);
                    yu.UnitId = unitId;
                    yu.Coe = Convert.ToInt32(Request["coe_" + name]);
                    if (Request["lecturer_" + name] != "null")
                    {
                        yu.Lecturer = Request["lecturer_" + name];
                    }
                    else {
                        yu.Lecturer = null;
                    }
                    if (Request["credits_" + name] != "")
                    {
                        yu.Ects = Convert.ToInt32(Request["credits_" + name]);
                    }
                    //if (Request["lecturer_" + name] != "")
                    //{
                    //    yu.Lecturer = Request["lecturer_" + name];
                    //}
                    yu.Period = Convert.ToInt32(Request["period_" + name]);
                    if (proposal.GetGeneral().LevelId == 1)
                    {
                        // if UG, check if compensating was selected
                        if (Request["comp_" + name] == "on")
                        {
                            yu.Compensating = 1;
                            yu.CompensatingReason = null;
                        }
                        else
                        {
                            yu.Compensating = 0;
                            if (Request["reason_" + name] != "")
                            {
                                yu.CompensatingReason = Request["reason_" + name];
                            }
                        }
                    }
                    else
                    {
                        yu.Compensating = 0;
                        yu.CompensatingReason = "Compensated passes are not applicable for PG courses.";
                    }
                    _context.Year_Unit.Add(yu);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            _context.SaveChanges();

            var jump = Request["jump"];
            switch (jump)
            {
                case "0":
                    {
                        // Save pressed
                        return RedirectToAction("Index", "TentativePs", new { id = proposal.Id });
                    }
                case "1":
                    {
                        // New unit pressed
                        return RedirectToAction("New", "Unit", new { yearRedirect = year.Id });
                    }
                default:
                    {
                        return RedirectToAction("Index", "TentativePs", new { id = proposal.Id });
                    }
            }

            

        }
    }
}