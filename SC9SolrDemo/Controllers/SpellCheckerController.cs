using SC9SolrDemo.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using SolrNet.Commands.Parameters;
using System.Collections.Generic;
using System.Web.Mvc;
using Sitecore.ContentSearch.SolrNetExtension;
using SolrNet;

namespace SC9SolrDemo.Controllers
{
    public class SpellCheckerController : Controller
    {
        public ActionResult Index()
        {
            var searchText = Request.QueryString["text"] ?? string.Empty;

            if (string.IsNullOrEmpty(searchText))
                return View();

            //var queryOptions = new QueryOptions
            //{                
            //    SpellCheck = new SpellCheckingParameters()
            //    {
            //        Query = searchText
            //    }
            //};

            var queryOptions = new SpellCheckHandlerQueryOptions()
            {
                SpellCheck = new SpellCheckingParameters()
                {
                    Query = searchText
                }
            };

            var indexName = $"sitecore_{Sitecore.Context.Database.Name}_index";
            var index = ContentSearchManager.GetIndex(indexName);

            SpellCheckResult spellCheckResult = new SpellCheckResult();

            using (var context = index.CreateSearchContext())
            {
                //var results = context.Query<SearchResultItem>($"_name:{searchText}", 
                //    queryOptions);

                var results = context.GetSpellCheck(new SolrQuery($"_name:{searchText}"), queryOptions);

                if (results?.SpellChecking == null || results.SpellChecking.Count < 1)
                {
                    return View();
                }

                var suggestions = new List<string>();
                foreach (var term in results.SpellChecking)
                {
                    foreach (var suggestion in term.Suggestions)
                    {
                        suggestions.Add(suggestion);
                    }
                }

                spellCheckResult.SearchText = searchText;
                spellCheckResult.Suggestions = suggestions;
            }
            return View(spellCheckResult);
        }
    }
}