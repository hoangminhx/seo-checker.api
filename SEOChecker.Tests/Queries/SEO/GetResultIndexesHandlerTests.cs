using Microsoft.Extensions.Options;
using Moq;
using SEOChecker.Application.DTOs.SEO;
using SEOChecker.Application.Interfaces;
using SEOChecker.Application.Queries.SEO.GetResultIndexes;
using SEOChecker.Domain.AppSettingsModels;
using SEOChecker.Domain.Constants;

namespace SEOChecker.Tests.Queries.SEO
{
    public class GetResultIndexesHandlerTests
    {
        private readonly Mock<IMemoryCacheService> mockMemoryCacheService;
        private readonly Mock<ISearchServiceFactory> mockSearchServiceFactory;
        private readonly Mock<IOptions<AppSettingsModel>> mockOptions;
        private readonly GetResultIndexesHandler handler;

        public GetResultIndexesHandlerTests()
        {
            mockMemoryCacheService = new Mock<IMemoryCacheService>();
            mockSearchServiceFactory = new Mock<ISearchServiceFactory>();

            mockOptions = new Mock<IOptions<AppSettingsModel>>();
            mockOptions.Setup(o => o.Value).Returns(new AppSettingsModel
            {
                CacheDurationInSecond = 60
            });

            handler = new GetResultIndexesHandler(mockMemoryCacheService.Object, mockSearchServiceFactory.Object, mockOptions.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedResult_WhenCacheIsHit()
        {
            //arrange
            GetResultIndexesQuery query = new(new SEORequestDto
            {
                SearchEngines = SearchEngines.Google,
                Keyword = "e-settlements",
                Target = "sympli.com.au",
                Range = 10
            });

            SearchEngineResultDto cachedResult = new()
            {
                SearchEngine = SearchEngines.Google,
                Indexes = new List<int> { 1, 2, 3 }
            };

            SearchEngineResultDto? cachedValue = cachedResult;
            mockMemoryCacheService
                .Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedValue))
                .Returns(true);

            //act
            var result = await handler.Handle(query, CancellationToken.None);

            //assert
            Assert.NotNull(result);
            Assert.Single(result.Result);
            Assert.Equal(SearchEngines.Google, result.Result.First().SearchEngine);
        }

        [Fact]
        public async Task Handle_ShouldCallSearchService_WhenCacheIsMissed()
        {
            //arrange
            GetResultIndexesQuery query = new(new SEORequestDto
            {
                SearchEngines = SearchEngines.Google,
                Keyword = "test",
                Target = "example.com",
                Range = 10
            });

            SearchEngineResultDto? cachedValue = null;
            mockMemoryCacheService.Setup(x => x.TryGetValue(It.IsAny<string>(), out cachedValue)).Returns(false);

            Mock<ISearchService> searchServiceMock = new();
            searchServiceMock
                .Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            mockSearchServiceFactory
                .Setup(x => x.GetSearchService(SearchEngines.Google))
                .Returns(searchServiceMock.Object);

            //act
            var result = await handler.Handle(query, CancellationToken.None);

            //assert
            Assert.NotNull(result);
            Assert.Single(result.Result);
            Assert.Equal(SearchEngines.Google, result.Result.First().SearchEngine);
            Assert.Equal(new List<int> { 1, 2, 3 }, result.Result.First().Indexes);
        }
    }
}
