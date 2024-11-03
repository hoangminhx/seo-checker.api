using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SEOChecker.Application.DTOs.SEO;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;

namespace SEOChecker.Application.Queries.SEO.GetResultIndexes
{
    public class GetResultIndexesHandler : IRequestHandler<GetResultIndexesQuery, SEOResponseDto>
    {
        private readonly IMemoryCache memoryCache;
        private readonly ISearchServiceFactory searchServiceFactory;
        private readonly AppSettingsModel appSettings;

        public GetResultIndexesHandler(IMemoryCache memoryCache, ISearchServiceFactory searchServiceFactory, IOptions<AppSettingsModel> options)
        {
            this.memoryCache = memoryCache;
            this.searchServiceFactory = searchServiceFactory;
            appSettings = options.Value;
        }

        public async Task<SEOResponseDto> Handle(GetResultIndexesQuery request, CancellationToken cancellationToken)
        {
            SEOResponseDto response = new SEOResponseDto();

            string[] searchEngines = request.request.SearchEngines.Split(',');

            foreach (string searchEngine in searchEngines)
            {
                SearchEngineResultDto? searchEngineResult = TryGetResultFromCache(searchEngine, request.request);
                if (searchEngineResult == null)
                {
                    ISearchService searchService = searchServiceFactory.GetSearchService(searchEngine);
                    IEnumerable<int> res = await searchService.SearchAsync(request.request.Keyword, request.request.Target, request.request.Range, cancellationToken);

                    searchEngineResult = new SearchEngineResultDto
                    { 
                        SearchEngine = searchEngine,
                        Indexes = res
                    };
                }

                response.Result = response.Result.Append(searchEngineResult);
            }

            SetResponseToCache(request.request, response);

            return response;
        }

        private SearchEngineResultDto? TryGetResultFromCache(string searchEngine, SEORequestDto request)
        {
            string key = $"{searchEngine}_{request.Keyword}_{request.Target}_{request.Range.ToString()}";

            SearchEngineResultDto? result;
            memoryCache.TryGetValue(key, out result);

            return result;
        }

        private void SetResponseToCache(SEORequestDto request, SEOResponseDto response)
        {
            string[] searchEngines = request.SearchEngines.Split(",");

            foreach (string searchEngine in searchEngines)
            {
                string key = $"{searchEngine}_{request.Keyword}_{request.Target}_{request.Range.ToString()}";

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(appSettings.CacheDurationInSecond) // Item expires after x seconds
                };

                memoryCache.Set(key, response.Result.FirstOrDefault(c => c.SearchEngine == searchEngine), cacheEntryOptions);
            }
        }
    }
}
