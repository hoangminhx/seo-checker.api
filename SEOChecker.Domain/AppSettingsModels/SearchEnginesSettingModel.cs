namespace SEOChecker.Domain.AppSettingsModels
{
    public record SearchEnginesSettingModel
    {
        public string Google { get; set; } = "https://www.google.com";
        public string Bing { get; set; } = "https://www.bing.com";
    }
}
