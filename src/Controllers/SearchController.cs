using DancingGoat.Customizations.Search;

using Kentico.Xperience.Lucene.Core.Indexing;
using Kentico.Xperience.Lucene.Core.Search;

using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.Util;

using Microsoft.AspNetCore.Mvc;

namespace DancingGoat.Controllers;

public class SearchController(
    ILuceneSearchService luceneSearchService,
    ILuceneIndexManager luceneIndexManager) : Controller
{
    private const int PHRASE_SLOP = 3;
    private const int MAX_RESULTS = 20;


    [HttpGet]
    public IActionResult Index(string searchText)
    {
        var index = luceneIndexManager.GetRequiredIndex("Articles");
        var query = GetQuery(searchText);
        var combinedQuery = new BooleanQuery
        {
            { query, Occur.MUST }
        };

        var results = luceneSearchService.UseSearcher(index, searcher =>
        {
            var topDocs = searcher.Search(combinedQuery, MAX_RESULTS);

            return topDocs.ScoreDocs.Select(scoreDoc =>
            {
                var doc = searcher.Doc(scoreDoc.Doc);

                return new ArticleSearchModel
                {
                    Title = doc.Get(nameof(ArticleSearchModel.Title)) ?? string.Empty,
                    Url = doc.Get(nameof(ArticleSearchModel.Url)) ?? string.Empty,
                    Summary = doc.Get(nameof(ArticleSearchModel.Summary)) ?? string.Empty,
                    Score = Math.Round(scoreDoc.Score * 100, 2)

                };
            })
            .ToList();
        });

        return View(new ArticleSearchResults { SearchText = searchText, Results = results });
    }


    private static BooleanQuery GetQuery(string searchText)
    {
        string queryText = searchText.Trim();
        var query = new BooleanQuery();

        if (!string.IsNullOrWhiteSpace(queryText))
        {
            var (slop, term) = queryText switch
            {
                ['"', .., '"'] => (0, queryText.Trim('"')),
                _ => (PHRASE_SLOP, queryText)
            };

            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var queryBuilder = new QueryBuilder(analyzer);
            var titleQuery = queryBuilder.CreatePhraseQuery(nameof(ArticleSearchModel.Title), term, slop);
            query = AddToTermQuery(query, titleQuery, 5);

            var summaryQuery = queryBuilder.CreatePhraseQuery(nameof(ArticleSearchModel.Summary), term, slop);
            query = AddToTermQuery(query, summaryQuery, 1);

            if (slop > 0)
            {
                var titleShould = queryBuilder.CreateBooleanQuery(nameof(ArticleSearchModel.Title), term, Occur.SHOULD);
                query = AddToTermQuery(query, titleShould, 0.5f);

                var summaryShould = queryBuilder.CreateBooleanQuery(nameof(ArticleSearchModel.Summary), term, Occur.SHOULD);
                query = AddToTermQuery(query, summaryShould, 0.1f);
            }
        }

        return query;
    }


    private static BooleanQuery AddToTermQuery(BooleanQuery query, Query textQueryPart, float boost)
    {
        if (textQueryPart is null)
        {
            return query;
        }

        textQueryPart.Boost = boost;
        query.Add(textQueryPart, Occur.SHOULD);

        return query;
    }
}
