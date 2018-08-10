using GAPT.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GAPT.ViewModels;

namespace GAPT.Controllers
{
    public class CommentController : Controller
    {
        private GaptDbContext _context;

        public CommentController()
        {
            _context = new GaptDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewReplies(int cid, int pid, string section)
        {
            var proposal = _context.Proposals.SingleOrDefault(m => m.Id == pid);
            if (proposal == null)
            {
                return HttpNotFound();
            }
            if (section != "#" || section != "General" || section != "Rationale" || section != "Demand" || section != "ProgrammeStudy" || section != "ExternalReview" || section != "Approval" || section != "TentativePs") {

            }
            string email = null;
            if (User.IsInRole(RoleName.ER))
            {
                email = User.Identity.GetUserName();
            }
            if (!proposal.IsViewable(User.Identity.GetUserId(), email))
            {
                return HttpNotFound();
            }

            var comment = _context.Comments.SingleOrDefault(m => m.Id == cid);
            if (comment == null) {
                return HttpNotFound();
            }
            var replies = new List<Comment>();
            replies = _context.Comments.Where(m => m.ReplyTo == cid).OrderBy(m => m.Date).ToList();
           
            var viewModel = new ViewRepliesViewModel
            {
                Comment = comment,
                Replies = replies,
                Pid = pid,
                Submitted = proposal.Submitted,
                HasApproval = proposal.HasFacultyApproval(),
                Section = section
            };
            return View(viewModel);

        }

        [HttpPost]
        public ActionResult Edit(int cid)
        {
            var comment = _context.Comments.SingleOrDefault(m => m.Id == cid);
            if (comment == null || comment.UserId != User.Identity.GetUserName())
            {
                return HttpNotFound();
            }
            foreach (string name in Request.Form.AllKeys)
            {

                try
                {
                    int commentId = Convert.ToInt32(name);
                    if (cid != commentId) {
                        return HttpNotFound();
                    }
                    if (Request[name] == "")
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    comment.Description = Request[name];
                    comment.Date = DateTime.Now;
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
       
        public ActionResult Remove(int cid, bool rtc)
        {
            //rtc removes thread_comment. Must be false for replies
            var comment = _context.Comments.SingleOrDefault(m => m.Id == cid);
            Proposal proposal = null;
            if (comment == null || comment.UserId != User.Identity.GetUserName())
            {
                return HttpNotFound();
            }
            
            var replies = _context.Comments.Where(m => m.ReplyTo == cid).ToList();
            if (replies.Count() > 0) {
                foreach (Comment reply in replies) {
                    Remove(reply.Id, false);
                }
                //_context.Comments.RemoveRange(replies);
            }
            if (rtc == true)
            {
                var tc = _context.Thread_Comment.SingleOrDefault(m => m.CommentId == cid);
                if (tc != null)
                {
                    var general = _context.Generals.SingleOrDefault(m => m.ThreadId == tc.ThreadId);
                    proposal = general.GetProposal();
                    _context.Thread_Comment.Remove(tc);
                }

            }
            
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());


        }

        [HttpPost]
        public ActionResult Save()
        {
            foreach (string name in Request.Form.AllKeys)
            {

                try
                {
                    int threadId = Convert.ToInt32(name);
                    Thread t = _context.Threads.Single(m => m.Id == threadId);
                    Comment c = new Comment();
                    if (Request[name] == "") {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    c.Description = Request[name];
                    c.UserId = User.Identity.GetUserName();
                    c.Date = DateTime.Now;
                    c.ReplyTo = null;
                    if (User.IsInRole(RoleName.Pvc) || User.IsInRole(RoleName.PvcChairman))
                    {
                        c.Type = "PVC";
                    }
                    else if (User.IsInRole(RoleName.Apqru))
                    {
                        c.Type = "APQRU";
                    }
                    else if (User.IsInRole(RoleName.ER))
                    {
                        c.Type = "ER";
                    }
                    else
                    {
                        c.Type = "UM";
                    }
                    _context.Comments.Add(c);
                    Thread_Comment tc = new Thread_Comment();
                    tc.Thread = t;
                    tc.Comment = c;
                    _context.Thread_Comment.Add(tc);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult Reply()
        {
            foreach (string name in Request.Form.AllKeys)
            {

                try
                {
                    int commentId = Convert.ToInt32(name);
                    Comment c = _context.Comments.SingleOrDefault(m => m.Id == commentId);
                    Comment rep = new Comment();
                    if (Request[name] == "")
                    {
                        return Redirect(Request.UrlReferrer.ToString());
                    }
                    rep.Description = Request[name];
                    rep.UserId = User.Identity.GetUserName();
                    rep.Date = DateTime.Now;
                    rep.ReplyTo = commentId;
                    if (User.IsInRole(RoleName.Pvc) || User.IsInRole(RoleName.PvcChairman))
                    {
                        rep.Type = "PVC";
                    }
                    else if (User.IsInRole(RoleName.Apqru))
                    {
                        rep.Type = "APQRU";
                    }
                    else if (User.IsInRole(RoleName.ER))
                    {
                        rep.Type = "ER";
                    }
                    else
                    {
                        rep.Type = "UM";
                    }
                    _context.Comments.Add(rep);
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}