namespace SEOChecker.API.Configs
{
    public static class CorsConfig
    {
        public static IServiceCollection AddCorsWithConfig(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            CorsConfigModel cfg = configurationManager.GetSection("App:CorsConfig").Get<CorsConfigModel>() ?? new CorsConfigModel();
            return services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins(cfg.AllowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }

    public record CorsConfigModel
    {
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    }
}
