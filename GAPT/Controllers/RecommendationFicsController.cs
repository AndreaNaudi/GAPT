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
    public class RecommendationFicsController : Controller
    {
        private GaptDbContext _context;

        public RecommendationFicsController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Details(int id)
        {
            var recommendation = _context.RecommendationFics.SingleOrDefault(m => m.Id == id);
            if (recommendation == null)
            {
                return HttpNotFound();
            }

            //get proposal
            var ar = _context.Approval_Recommendation.SingleOrDefault(m => m.RecommendationId == recommendation.Id);
            var proposal = _context.Proposals.SingleOrDefault(m => m.ApprovalId == ar.ApprovalId);
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (!proposal.IsViewable(User.Identity.GetUserId()))
            {
                return HttpNotFound();
            }

            if (recommendation.SignedBy == null)
            {
                

                if (User.IsInRole(RoleName.Dean))
                {
                    var faculty = _context.Ref_Faculty.SingleOrDefault(m => m.Id == recommendation.FacultyId);
                    if (faculty.Dean == User.Identity.GetUserId())
                    {
                        //return Form if user is the correct dean

                        //set datepickers start to today
                        recommendation.HeldDateA = DateTime.Today.ToString();
                        recommendation.HeldDateB = DateTime.Today.ToString();
                        var viewModel = new RecommendationFicsFormViewModel
                        {
                            Proposal = proposal,
                            Recommendation = recommendation
                        };
                        return View("Form", viewModel);
                    }
                    else {
                        return View("NoDecision", proposal.Id);
                    }

                    
                }
                else {
                    return View("NoDecision", proposal.Id);
                }

                
            }
            else
            {
                ApplicationDbContext adb = new ApplicationDbContext();
                var signedBy = adb.Users.SingleOrDefault(m => m.Id == recommendation.SignedBy);
                if (recommendation.Selection == true)
                {
                    //return accept view
                    var viewModel = new RecommendationFicsFormViewModel
                    {
                        Proposal = proposal,
                        Recommendation = recommendation,
                        SignedBy = signedBy.Email
                    };
                    return View("Accepted", viewModel);
                }
                else
                {
                    //return reject view
                    var viewModel = new RecommendationFicsFormViewModel
                    {
                        Proposal = proposal,
                        Recommendation = recommendation,
                        SignedBy = signedBy.Email
                    };
                    return View("Rejected", viewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Dean)]
        public ActionResult Save(RecommendationFicsFormViewModel vm)
        {
            var proposal = vm.Proposal;
            proposal = _context.Proposals.SingleOrDefault(m => m.Id == proposal.Id);
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            var recommendation = vm.Recommendation;

            var faculty = _context.Ref_Faculty.SingleOrDefault(m => m.Id == recommendation.FacultyId);
            if (faculty.Dean != User.Identity.GetUserId())
            {
                return HttpNotFound();
            }

            if (recommendation.Selection == true)
            {
                //if a is chosen, set helddateB to a default date
                recommendation.HeldDateB = "01/01/0001";
            }
            else {
                //if b is chosen, set helddateA to a default date
                recommendation.HeldDateA = "01/01/0001";
            }
            if (!ModelState.IsValid)
            {
                return View("Form", recommendation);
            }

            var recommendationInDb = _context.RecommendationFics.SingleOrDefault(m => m.Id == recommendation.Id);
            recommendationInDb.HeldDateA = recommendation.HeldDateA;
            recommendationInDb.HeldDateB = recommendation.HeldDateB;
            recommendationInDb.SignedBy = User.Identity.GetUserId();
            recommendationInDb.SignedDate = DateTime.Now;
            recommendationInDb.Selection = recommendation.Selection;

            _context.SaveChanges();
            return RedirectToAction("Approval", "Proposal", new { id = proposal.Id });

        }
    }
}