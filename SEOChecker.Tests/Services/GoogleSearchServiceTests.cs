using Microsoft.Extensions.Options;
using Moq;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;
using SEOChecker.Infrastructure.Services;

namespace SEOChecker.Tests.Services
{
    public class GoogleSearchServiceTests
    {
        private readonly GoogleSearchService service;
        private readonly Mock<IOptions<SearchEnginesSettingModel>> mockOptions;
        private readonly Mock<IHtmlService> mockHtmlService;

        private readonly string fakeHtmlContent = @"<div id=""main""><div><div class=""Gx5Zad xpd EtOod pkphOe""><div class=""egMi0 kCrYT""><a href=""/url?q=https://esettlementgroup.com/&amp;"" data-ved=""2ahUKEwi3m5zxvsCJAxUMr1YBHSTJJq4QFnoECAIQAg""><div class=""DnJfK"">
</div></a></div></div></div><div><div class=""Gx5Zad xpd EtOod pkphOe""><div class=""egMi0 kCrYT""><a href=""/url?q=https://www.sympli.com.au/&amp;sa=U&amp;"" data-ved=""2ahUKEwi3m5zxvsCJAxUMr1YBHSTJJq4QFnoECBUQAg""><div class=""DnJfK"">
</div></a></div></div></div><div><div class=""Gx5Zad xpd EtOod pkphOe""><div class=""egMi0 kCrYT""><a href=""/url?q=https://ng.linkedin.com/company/esettlement&amp;"" data-ved=""2ahUKEwi3m5zxvsCJAxUMr1YBHSTJJq4QFnoECGEQAg""><div class=""DnJfK"">
</div></a></div></div></div><div><div class=""Gx5Zad xpd EtOod pkphOe""><div class=""egMi0 kCrYT""><a href=""/url?q=https://www.sympli.com.au/&amp;sa=U&amp;"" data-ved=""2ahUKEwi3m5zxvsCJAxUMr1YBHSTJJq4QFnoECBUQAg""><div class=""DnJfK"">
</div></a></div></div></div><div><div class=""Gx5Zad xpd EtOod pkphOe""><div class=""egMi0 kCrYT""><a href=""/url?q=https://www.sympli.com.au/&amp;sa=U&amp;"" data-ved=""2ahUKEwi3m5zxvsCJAxUMr1YBHSTJJq4QFnoECBUQAg""><div class=""DnJfK"">
</div></a></div></div></div></div>";

        public GoogleSearchServiceTests()
        {
            mockOptions = new Mock<IOptions<SearchEnginesSettingModel>>();
            mockOptions.Setup(o => o.Value).Returns(new SearchEnginesSettingModel { Google = "http://fake-google.com" });
            mockHtmlService = new Mock<IHtmlService>();

            service = new GoogleSearchService(mockOptions.Object, mockHtmlService.Object);
        }

        [Theory]
        [InlineData("e-settlements", "sympli.com.au", 100, new int[]{2, 4, 5})]
        [InlineData("e-settlements", "cannot contain this text", 100, new int[0])]
        public async Task GoogleSearchService_SearchAsync_ShouldReturnIndexes(string keyword, string target, int range, int[] indexes)
        {
            //arange
            mockHtmlService.Setup(c => c.ReadHTMLAsStringAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(fakeHtmlContent);

            //act
            var result = await service.SearchAsync(keyword, target, range, CancellationToken.None);

            //assert
            Assert.NotNull(result);
            Assert.True(indexes.Count() == result.Count() && !indexes.Except(result).Any() && !result.Except(indexes).Any());
        }
    }
}
