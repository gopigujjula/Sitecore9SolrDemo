using SC9SolrDemo.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SolrNetExtension;
using Sitecore.ContentSearch.SolrProvider.SolrNetIntegration;
using System.Linq;
using System.Web.Mvc;

namespace SC9SolrDemo.Controllers
{
    public class SuggesterController : Controller
    {
        public ActionResult Index()
        {
            var searchText = Request.QueryString["text"] ?? string.Empty;

            if (string.IsNullOrEmpty(searchText))
                return View();

            SolrSuggestQuery query = searchText;
           
            var queryOptions = new SuggestHandlerQueryOptions
            {
                Parameters = new SuggestParameters
                {
                    Count = 3,
                    Build = true
                }
            };

            var indexName = $"sitecore_{Sitecore.Context.Database.Name}_index";
            var index = ContentSearchManager.GetIndex(indexName);

            SuggesterResult suggesterResult = new SuggesterResult();

            using (var context = index.CreateSearchContext())
            {
                var results = context.Suggest(query, queryOptions);
                
                var suggestions = results.Suggestions["mySuggester"].Suggestions.Select(a => a.Term);
                
                suggesterResult.SearchText = searchText;
                suggesterResult.Suggestions = suggestions.ToList();
            }

            return View(suggesterResult);
        }
    }
}