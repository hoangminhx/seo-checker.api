using Microsoft.Extensions.Options;
using Moq;
using SEOChecker.Application.Interfaces;
using SEOChecker.Domain.AppSettingsModels;
using SEOChecker.Infrastructure.Services;

namespace SEOChecker.Tests.Services
{
    public class BingSearchServiceTests
    {
        private readonly BingSearchService service;
        private readonly Mock<IOptions<SearchEnginesSettingModel>> mockOptions;
        private readonly Mock<IHtmlService> mockHtmlService;

        private readonly string fakeHtmlContent = @"<ol id=""b_results"" class=""""><li class=""b_algo"" data-id="""" iid=""""><div class=""b_tpcn""><a class=""tilk"" aria-label=""sympli.com.au"" href=""https://www.sympli.com.au/"" h=""ID=SERP,5128.1"">
</a><div class=""tptxt""><div class=""tptt"">sympli.com.au</div><div class=""tpmeta""><div class=""b_attribution"" tabindex=""0""><cite>https://www.sympli.com.au</cite></span></span></div></div></div></a></div><h2><a href=""https://www.sympli.com.au/"" h=""ID=SERP,5128.2"">Making <strong>e-Settlements</strong> Simple | Sympli</a></h2></div></li><li class=""b_algo"" data-id="""" iid=""""><div class=""b_tpcn""><a class=""tilk"" aria-label=""oracle.com"" href=""https://www.oracle.com/us/products/applications/peoplesoft-enterprise/srm/052355.html"" h=""ID=SERP,5146.1"">
</a><div class=""tpic""><a class=""tilk"" aria-label=""oracle.com"" href=""https://www.oracle.com/us/products/applications/peoplesoft-enterprise/srm/052355.html"" h=""ID=SERP,5146.1""><div class=""tptxt""><div class=""tpmeta""><div class=""b_attribution"" u=""1|5095|4998758290556487|BMOzn3ExN2XhLFOG9_ezd4n1-SrKlD5A"" tabindex=""0""><cite>https://www.oracle.com › us › products › applications › ...</cite></div></div></div></a></div><h2><a href=""https://www.oracle.com/us/products/applications/peoplesoft-enterprise/srm/052355.html"" h=""ID=SERP,5146.2"">PeopleSoft <strong>Enterprise eSettlements</strong> - <strong>Oracle</strong></a></h2></div></li><li class=""b_algo"" data-id="""" iid=""""><div class=""b_tpcn""><a class=""tilk"" aria-label=""oracle.com"" href=""https://www.oracle.com/a/ocom/docs/applications/peoplesoft/peoplesoft-esettlements.pdf"" h=""ID=SERP,5162.1"">
</a><div class=""tpic""><a class=""tilk"" aria-label=""oracle.com"" href=""https://www.oracle.com/a/ocom/docs/applications/peoplesoft/peoplesoft-esettlements.pdf"" h=""ID=SERP,5162.1""><div class=""tptxt""><div class=""tpmeta""><div class=""b_attribution"" tabindex=""0""><cite>https://www.oracle.com › ... › peoplesoft-esettlements.pdf</cite>&nbsp;· PDF tệp</div></div></div></a></div><h2><a href=""https://www.oracle.com/a/ocom/docs/applications/peoplesoft/peoplesoft-esettlements.pdf"" h=""ID=SERP,5162.2"">PeopleSoft Enterprise <strong>eSettlements</strong> - <strong>Oracle</strong></a></h2></div></li></ol>";

        public BingSearchServiceTests()
        {
            mockOptions = new Mock<IOptions<SearchEnginesSettingModel>>();
            mockOptions.Setup(o => o.Value).Returns(new SearchEnginesSettingModel { Google = "http://fake-bing.com" });
            mockHtmlService = new Mock<IHtmlService>();

            service = new BingSearchService(mockOptions.Object, mockHtmlService.Object);
        }

        [Theory]
        [InlineData("e-settlements", "sympli.com.au", 20, new int[] { 1 })]
        [InlineData("e-settlements", "cannot contain this text", 20, new int[0])]
        public async Task BingSearchService_SearchAsync_ShouldReturnIndexes(string keyword, string target, int range, int[] indexes)
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
