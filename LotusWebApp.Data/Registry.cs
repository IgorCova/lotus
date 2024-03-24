using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;

namespace LotusWebApp.Data;

public static class DbContextRegistration
{
    /// <summary>
    /// Регистрация DB контекст
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="typeOfRegistration"></param>
    /// <param name="configuration"></param>
    public static void RegisterContext(this IServiceCollection serviceCollection, Type typeOfRegistration,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetAssembly(typeOfRegistration);

        if (assembly == null)
            return;

        foreach (var context in AppDomain.CurrentDomain.GetAssemblies()
                     .Where(x => x.FullName != null)
                     .SelectMany(s => s.GetTypes())
                     .Where(p => typeOfRegistration.IsAssignableFrom(p) && p.IsClass))
        {
            var contextInstance = context.CreateInstance();
            var methodOfRegistration = contextInstance.GetType().GetMethod("RegisterContext",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder,
                new[] { typeof(IServiceCollection), typeof(IConfiguration) }, null);

            methodOfRegistration?.Invoke(contextInstance,
                new object[] { serviceCollection, configuration });
        }
    }

    /// <summary>
    /// Регистрация всех репозиториев
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void RegisterAllRepository(this IServiceCollection serviceCollection)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var context in assemblies
                     .Where(x => x.FullName != null && x.FullName.Contains("Lotus"))
                     .SelectMany(s => s.GetTypes())
                     .Where(p => p.FullName != null && typeof(IRegisterRepository).IsAssignableFrom(p) && p.IsClass
                                 && !p.FullName.Contains("RepositoryBase")))
        {
            var interfaceType = assemblies
                .Where(x => x.FullName != null && x.FullName.Contains("Lotus"))
                .SelectMany(s => s.GetTypes()).FirstOrDefault(p =>
                    p.FullName != null && p.IsAssignableFrom(context) && p.IsClass &&
                    !p.FullName.Contains("RepositoryBase"))
                ?.GetInterfaces().FirstOrDefault(x => x.FullName?.Contains(context.Name) ?? false);

            if (interfaceType != null)
            {
                serviceCollection.AddScoped(interfaceType, context);
                Console.WriteLine($"Added repository {context.Name} implementation of {interfaceType.Name}");
            }
            else
            {
                serviceCollection.AddScoped(context);
                Console.WriteLine($"Added repository {context.Name} without implementation interface");
            }
        }
    }

    public static void RegisterDataProviderService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IDataProviderService, DataProviderService>();
    }
}