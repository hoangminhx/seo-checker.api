namespace SEOChecker.Application.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<int>> SearchAsync(string keyword, string target, int range, CancellationToken cancellationToken);
    }
}
