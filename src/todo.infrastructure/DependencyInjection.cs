using System;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services;
        }
    }
}
