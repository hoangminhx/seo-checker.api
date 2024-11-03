using Microsoft.Extensions.Options;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;
using System.Text.RegularExpressions;

namespace SEOChecker.Infrastructure.Services
{
    public class BingSearchService : ISearchService
    {
        private readonly SearchEnginesSettingModel searchEnginesSettings;
        private readonly IHtmlService htmlService;

        public BingSearchService(IOptions<SearchEnginesSettingModel> options, IHtmlService htmlService)
        {
            searchEnginesSettings = options.Value;
            this.htmlService = htmlService;
        }

        public async Task<IEnumerable<int>> SearchAsync(string keyword, string target, int range, CancellationToken cancellationToken)
        {
            List<int> indexes = new();

            const int PAGE_SIZE = 20;
            int pageCount = (range % PAGE_SIZE) > 0 ? ((range / PAGE_SIZE) + 1) : (range / PAGE_SIZE);

            for (int page = 0; page < pageCount; page++)
            {
                string requestUri = $"{searchEnginesSettings.Bing}/search?q={Uri.EscapeDataString(keyword)}&count={PAGE_SIZE}&first={page * PAGE_SIZE}&_={Guid.NewGuid()}";
                string htmlContent = await htmlService.ReadHTMLAsStringAsync(requestUri, cancellationToken);

                Regex regex = new Regex("<li class=\"b_algo(.*?)</li>", RegexOptions.Singleline);

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
