namespace SEOChecker.Application.Interfaces
{
    public interface ISearchServiceFactory
    {
        ISearchService GetSearchService(string searchEngine);
    }
}
