using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LotusWebApp.Data;

/// <summary>
/// Интерфейс для регистрации контекста
/// </summary>
public interface IContextRegistration
{
    void RegisterContext(IServiceCollection collection, IConfiguration configuration);
}