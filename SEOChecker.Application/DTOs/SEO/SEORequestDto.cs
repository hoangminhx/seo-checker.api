namespace SEOChecker.Application.DTOs.SEO
{
    public record SEORequestDto
    {
        public string Keyword { get; set; } = "";
        public string Target { get; set; } = "";
        public string SearchEngines { get; set; } = "";
        public int Range { get; set; } = 100;
    }
}
