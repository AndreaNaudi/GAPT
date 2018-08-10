using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GAPT.Models;

namespace GAPT.ViewModels
{
    public class ViewRepliesViewModel
    {
        public IEnumerable<Comment> Replies { get; set; }
        public Comment Comment { get; set; }
        public int Pid { get; set; }
        public bool Submitted { get; set; }
        public bool HasApproval { get; set; }
        public string Section { get; set; }
    }
}