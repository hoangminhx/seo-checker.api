using Microsoft.Extensions.DependencyInjection;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.Constants;

namespace SEOChecker.Infrastructure.Services
{
    public class SearchServiceFactory : ISearchServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SearchServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public ISearchService GetSearchService(string searchEngine)
        {
            switch (searchEngine)
            {
                case SearchEngines.Google:
                    return serviceProvider.GetRequiredService<GoogleSearchService>();
                case SearchEngines.Bing:
                    return serviceProvider.GetRequiredService<BingSearchService>();
                default:
                    throw new Exception("Invalid search engine.");
            }
        }
    }
}
