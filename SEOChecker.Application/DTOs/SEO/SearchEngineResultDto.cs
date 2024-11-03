namespace SEOChecker.Application.DTOs.SEO
{
    public record SearchEngineResultDto
    {
        public string? SearchEngine { get; set; }
        public IEnumerable<int>? Indexes { get; set; }
    }
}
