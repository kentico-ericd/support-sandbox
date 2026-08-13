using CMS.ContentEngine;
using CMS.Websites;

using DancingGoat.Models;

using Kentico.Content.Web.Mvc;
using Kentico.Xperience.Lucene.Core.Indexing;

using Lucene.Net.Documents;

namespace DancingGoat.Customizations.Search;

public class ArticleIndexingStrategy(IContentRetriever contentRetriever) : DefaultLuceneIndexingStrategy
{
    public override async Task<Document?> MapToLuceneDocumentOrNull(IIndexEventItemModel item)
    {
        var document = new Document();
        string url = string.Empty;
        string title = string.Empty;

        if (item is IndexEventWebPageItemModel webpageItem &&
            string.Equals(item.ContentTypeName, ArticlePage.CONTENT_TYPE_NAME, StringComparison.OrdinalIgnoreCase))
        {
            var parameters = new RetrievePagesParameters
            {
                ChannelName = webpageItem.WebsiteChannelName,
                LanguageName = webpageItem.LanguageName,
                IsForPreview = false
            };
            var result = await contentRetriever.RetrievePagesByGuids<ArticlePage>([webpageItem.ItemGuid], parameters);
            var page = result.FirstOrDefault();
            if (page is null)
            {
                return null;
            }

            title = page.ArticleTitle;
            url = page.GetUrl().AbsoluteUrl;
        }

        document.Add(new TextField(nameof(ArticleSearchResult.Title), title, Field.Store.YES));
        document.Add(new TextField(nameof(ArticleSearchResult.Url), url, Field.Store.YES));

        return document;
    }
}
