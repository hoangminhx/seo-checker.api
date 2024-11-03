namespace SEOChecker.Domain.AppSettingsModels
{
    public record AppSettingsModel
    {
        public int CacheDurationInSecond { get; set; } = 1800;
    }
}
