using GAPT.Models;
using GAPT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class ApprovalController : Controller
    {
        private GaptDbContext _context;

        public ApprovalController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Jump(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted)
            {
                return HttpNotFound();
            }
            if (proposal.ApprovalId == null)
            {
                Approval a = new Approval();
                proposal.Approval = a;
                Thread t = new Thread();
                _context.Threads.Add(t);
                a.Thread = t;
                _context.Approvals.Add(a);
                _context.SaveChanges();
                return RedirectToAction("Approval", "Proposal", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("Approval", "Proposal", new { id = proposal.Id });
            }
        }

        [Authorize(Roles = RoleName.Apqru)]
        public ActionResult ResetApprovals(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted || (proposal.HasFacultyApproval() && (bool)proposal.InPrincipalApproved))
            {
                return HttpNotFound();
            }
            var prevApprovalId = proposal.ApprovalId;
            

            //remove data from linking and object tables
            var aes = _context.Approval_Endorsement.Where(m => m.ApprovalId == prevApprovalId);
            foreach (Approval_Endorsement ae in aes) {
                var endorsement = _context.EndorsementCollabs.SingleOrDefault(m => m.Id == ae.EndorsementId);
                if (endorsement != null) {
                    endorsement.Selection = null;
                    endorsement.HeldDate = "_";
                    endorsement.SignedBy = null;
                    endorsement.SignedDate = null;
                }
                
            }

            var ass = _context.Approval_Statement.Where(m => m.ApprovalId == prevApprovalId);
            foreach (Approval_Statement a in ass)
            {
                var statement = _context.StatementServs.SingleOrDefault(m => m.Id == a.StatementId);
                if (statement != null && statement.Selection == false)
                {
                    statement.Selection = null;
                    statement.Reservations = "_";
                    statement.SignedDate = null;
                    statement.SignedBy = null;
                }

            }

            var ars = _context.Approval_Recommendation.Where(m => m.ApprovalId == prevApprovalId);
            foreach (Approval_Recommendation ar in ars)
            {
                var rec = _context.RecommendationFics.SingleOrDefault(m => m.Id == ar.RecommendationId);
                if (rec != null)
                {
                    rec.Selection = null;
                    rec.HeldDateA = "_";
                    rec.HeldDateB = "_";
                    rec.SignedBy = null;
                    rec.SignedDate = null;
                }

            }
            proposal.Submitted = false;
            proposal.RequiresModification = true;
            _context.SaveChanges();
            return RedirectToAction("Index", "Proposal");
        }

    }
}