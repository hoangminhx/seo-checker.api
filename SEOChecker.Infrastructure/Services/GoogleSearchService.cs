using Microsoft.Extensions.Options;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;
using System.Text.RegularExpressions;

namespace SEOChecker.Infrastructure.Services
{
    public class GoogleSearchService : ISearchService
    {
        private readonly SearchEnginesSettingModel searchEnginesSettings;

        public GoogleSearchService(IOptions<SearchEnginesSettingModel> options) 
        {
            searchEnginesSettings = options.Value;
        }

        public async Task<IEnumerable<int>> SearchAsync(string keyword, string target, int range, CancellationToken cancellationToken)
        {
            using (HttpClient client = new HttpClient())
            {
                List<int> indexes = new List<int>();

                const int PAGE_SIZE = 100;
                int pageCount = (range % PAGE_SIZE) > 0 ? ((range / PAGE_SIZE) + 1) : (range / PAGE_SIZE);

                for (int page = 0; page < pageCount; page++)
                {
                    string requestUri = $"{searchEnginesSettings.Google}/search?q={Uri.EscapeDataString(keyword)}&num={PAGE_SIZE}&start={page * PAGE_SIZE}";

                    HttpResponseMessage response = await client.GetAsync(requestUri, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception("Failed to fetch");

                    string htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    Regex regex = new Regex("<div class=\"Gx5Zad xpd EtOod pkphOe\">(.*?)</div>", RegexOptions.Singleline);

                    var matches = regex.Matches(htmlContent);

                    for (int i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];
                        string snippet = match.Groups[1].Value;
                        if (snippet.Contains(target, StringComparison.OrdinalIgnoreCase))
                            indexes.Add(page * PAGE_SIZE + i + 1);
                    }
                }

                return indexes;
            }
        }
    }
}
