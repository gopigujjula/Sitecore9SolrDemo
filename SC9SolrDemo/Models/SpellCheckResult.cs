using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SC9SolrDemo.Models
{
    public class SpellCheckResult
    {
        public string SearchText { get; set; }
        public List<string> Suggestions { get; set; }
    }
}