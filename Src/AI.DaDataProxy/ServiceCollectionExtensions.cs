using Microsoft.Extensions.DependencyInjection;
using AI.DaDataProxy.DaData;

namespace AI.DaDataProxy
{
    /// <summary>
    /// Provides extension methods for IServiceCollection to register project-specific services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Project Name specific services to the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddDaDataProxyServices(this IServiceCollection services)
        {
            services.AddScoped<DaDataHandler>();

            return services;
        }
    }
}