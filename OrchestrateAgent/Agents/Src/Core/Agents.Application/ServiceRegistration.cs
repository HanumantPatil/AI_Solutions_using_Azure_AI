using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Agents.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
