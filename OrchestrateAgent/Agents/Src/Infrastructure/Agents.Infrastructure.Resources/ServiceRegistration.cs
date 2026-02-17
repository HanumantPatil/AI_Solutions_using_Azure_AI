using Agents.Application.Interfaces;
using Agents.Infrastructure.Resources.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.Infrastructure.Resources
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddResourcesInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ITranslator, Translator>();

            return services;
        }
    }
}
