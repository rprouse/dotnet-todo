using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Alteridem.Todo.Domain.Interfaces;
using Alteridem.Todo.Infrastructure.Persistence;
using ColoredConsole;
using Microsoft.Extensions.DependencyInjection;

namespace Alteridem.Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string configFile)
    {
        services.AddTransient<ITaskFile, TaskFile>();
        services.AddSingleton<ITaskConfiguration, TaskConfiguration>((_) =>
        {
            try
            {
                if (File.Exists(configFile))
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        IgnoreNullValues = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    string json = File.ReadAllText(configFile);
                    return JsonSerializer.Deserialize<TaskConfiguration>(json, options);
                }
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLine("Error reading configuration file. Using default configuration".Red());
                ColorConsole.WriteLine(ex.Message.Red());
                Console.WriteLine();
            }
            return new TaskConfiguration();
        });
        return services;
    }
}
