using Microsoft.Extensions.DependencyInjection;
using SEOChecker.Application.Interfaces;
using SEOChecker.Infrastructure.Services;

namespace SEOChecker.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<ISearchService, GoogleSearchService>();
            services.AddTransient<ISearchService, BingSearchService>();
            services.AddTransient<GoogleSearchService>();
            services.AddTransient<BingSearchService>();
            services.AddTransient<ISearchServiceFactory, SearchServiceFactory>();
            return services;
        }
    }
}
