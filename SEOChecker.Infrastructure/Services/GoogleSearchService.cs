using Microsoft.Extensions.Options;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;
using System.Text.RegularExpressions;

namespace SEOChecker.Infrastructure.Services
{
    public class GoogleSearchService : ISearchService
    {
        private readonly SearchEnginesSettingModel searchEnginesSettings;
        private readonly IHtmlService htmlService;

        public GoogleSearchService(IOptions<SearchEnginesSettingModel> options, IHtmlService htmlService) 
        {
            searchEnginesSettings = options.Value;
            this.htmlService = htmlService;
        }

        public async Task<IEnumerable<int>> SearchAsync(string keyword, string target, int range, CancellationToken cancellationToken)
        {
            List<int> indexes = new();

            const int PAGE_SIZE = 100;
            int pageCount = (range % PAGE_SIZE) > 0 ? ((range / PAGE_SIZE) + 1) : (range / PAGE_SIZE);

            for (int page = 0; page < pageCount; page++)
            {
                string requestUri = $"{searchEnginesSettings.Google}/search?q={Uri.EscapeDataString(keyword)}&num={PAGE_SIZE}&start={page * PAGE_SIZE}";
                string htmlContent = await htmlService.ReadHTMLAsStringAsync(requestUri, cancellationToken);

                Regex regex = new("<div class=\"Gx5Zad xpd EtOod pkphOe\">(.*?)</div>", RegexOptions.Singleline);

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
