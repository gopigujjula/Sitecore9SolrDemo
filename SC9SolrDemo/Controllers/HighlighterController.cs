using SC9SolrDemo.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SC9SolrDemo.Controllers
{
    public class HighlighterController: Controller
    {
        public ActionResult Index()
        {
            string searchField = "description_t";
            
            string searchValue = Request.QueryString["text"] ?? string.Empty;

            if (string.IsNullOrEmpty(searchValue))
                return View();


            var queryOptions = new QueryOptions
            {
                Highlight = new HighlightingParameters
                {
                    Fields = new[] { searchField },
                    BeforeTerm = "<em style='color:red'>",
                    AfterTerm = "</em>",
                    Fragsize =10000
                }               
            };

            var indexName = string.Format("sitecore_{0}_index", Sitecore.Context.Database.Name);
            var index = ContentSearchManager.GetIndex(indexName);

            List<HighlightResult> highlightResults = new List<HighlightResult>();

            using (var context = index.CreateSearchContext())
            {
                var results = context.Query<SearchResultItem>(
                    new SolrQueryByField(searchField, searchValue), queryOptions);

                foreach (var result in results)
                {
                    var highlights = results.Highlights[result.Fields["_uniqueid"].ToString()];

                    if (highlights.Any())
                    {
                        foreach(var highlightResult in highlights)
                        {
                            highlightResults.Add(
                                new HighlightResult
                                {
                                    Name = result.Name,
                                    Value = string.Join(",", highlightResult.Value)
                                });
                        }
                    }
                }
            }
            
            return View(highlightResults);
        }
    }
}