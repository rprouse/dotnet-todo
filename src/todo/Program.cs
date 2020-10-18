using Alteridem.Todo.Application;
using Alteridem.Todo.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo
{
    class Program
    {
        static void Main()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var app = new TodoApplication(serviceCollection);
            app.Run();
        }

        static private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddApplication()
                .AddInfrastructure();
        }
    }
}
