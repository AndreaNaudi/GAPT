using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Proposal> AllProposals { get; set; }
    }
}