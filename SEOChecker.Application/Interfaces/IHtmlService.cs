namespace SEOChecker.Application.Interfaces
{
    public interface IHtmlService
    {
        Task<string> ReadHTMLAsStringAsync(string requestUri, CancellationToken cancellationToken);
    }
}
