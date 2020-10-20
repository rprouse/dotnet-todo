using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<ITaskFile, TaskFile>();
            services.AddSingleton<ITaskConfiguration, TaskConfiguration>();
            return services;
        }
    }
}
