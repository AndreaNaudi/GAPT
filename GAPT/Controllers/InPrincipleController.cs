using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GAPT.Controllers
{
    public class InPrincipleController : Controller
    {
        private GaptDbContext _context;

        public InPrincipleController()
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
            if (proposal == null || !proposal.Submitted || !proposal.HasFacultyApproval())
            {
                return HttpNotFound();
            }
            if (proposal.InPrincipalId == null)
            {
                InPrincipal a = new InPrincipal();
                proposal.InPrincipal = a;
                _context.InPrincipals.Add(a);


                var pvc = new PvcApproval();
                pvc.Upload = "_";
                _context.PvcApprovals.Add(pvc);

                var ipp = new InPrincipal_Pvc();
                ipp.PvcApproval = pvc;
                ipp.PvcApprovalId = pvc.Id;
                ipp.InPrincipal = a;
                ipp.InPrincipalId = a.Id;

                _context.InPrincipal_Pvc.Add(ipp);
                _context.SaveChanges();
                return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });
            }
            else
            {
                return RedirectToAction("InPrinciple", "Proposal", new { id = proposal.Id });
            }
        }

        [Authorize(Roles = RoleName.Apqru)]
        public ActionResult ResetApprovals(int id)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == id);
            if (proposal == null || !proposal.Submitted || (bool)proposal.InPrincipalApproved)
            {
                return HttpNotFound();
            }
            var pvc = proposal.GetPvcApproval();
            
            var senate = proposal.GetSenateDecision();
            var council = proposal.GetCouncilDecision();

            //remove data from linking and object tables
            
            if (pvc != null)
            {
                var pvcInDb = _context.PvcApprovals.SingleOrDefault(m => m.Id == pvc.Id);
                pvcInDb.Selection = null;
                pvcInDb.SignedBy = null;
                pvcInDb.SignedDate = null;
                pvcInDb.CouncilRef = null;
                pvcInDb.Upload = "_";

            }

            if (senate != null) {
                var senateInDb = _context.SenateDecisions.SingleOrDefault(m => m.Id == senate.Id);
                senateInDb.Selection = false;
                senateInDb.SignedBy = null;
                senateInDb.SignedDate = null;
            }

            if (council != null)
            {
                var councilInDb = _context.CouncilDecisions.SingleOrDefault(m => m.Id == council.Id);
                councilInDb.Selection = false;
                councilInDb.SignedBy = null;
                councilInDb.SignedDate = null;
            }

            _context.SaveChanges();
            return RedirectToAction("ResetApprovals", "Approval", new { id = proposal.Id});
        }
    }
}