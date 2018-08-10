using GAPT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GAPT.ViewModels
{
    public class ProposalsIndexViewModel
    {
        public IEnumerable<Proposal> Proposals { get; set; }
        public string  Heading { get; set; }
        public int Count { get; set; }
    }
}