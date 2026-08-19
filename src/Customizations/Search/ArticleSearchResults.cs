namespace DancingGoat.Customizations.Search;

public class ArticleSearchResults
{
    public string SearchText { get; set; } = string.Empty;


    public IEnumerable<ArticleSearchModel> Results { get; set; } = Enumerable.Empty<ArticleSearchModel>();
}
