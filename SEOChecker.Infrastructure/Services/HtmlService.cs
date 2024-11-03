using SEOChecker.Application.Interfaces;

namespace SEOChecker.Infrastructure.Services
{
    internal class HtmlService : IHtmlService
    {
        private readonly HttpClient httpClient;

        private readonly string[] UserAgents = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
        };

        public HtmlService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgents[0]);
        }

        public async Task<string> ReadHTMLAsStringAsync(string requestUri, CancellationToken cancellationToken)
        {
            Random random = new Random();

            string userAgent = UserAgents[random.Next(UserAgents.Length)];
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

            int delay = random.Next(1000, 3000);
            Thread.Sleep(delay);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch");

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
