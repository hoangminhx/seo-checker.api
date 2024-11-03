namespace SEOChecker.Application.DTOs.SEO
{
    public record SEOResponseDto
    {
        public IEnumerable<SearchEngineResultDto> Result { get; set; } = new List<SearchEngineResultDto>();
    }
}
