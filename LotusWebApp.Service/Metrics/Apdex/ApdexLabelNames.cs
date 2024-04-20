namespace LotusWebApp.Metrics;

/// <summary>
/// Названия меток для метрик <a href="https://en.wikipedia.org/wiki/Apdex">Apdex</a>
/// </summary>
public static class ApdexLabelNames
{
	/// <summary>
	/// HTTP-метод + шаблон роута
	/// </summary>
	public const string Route = "route";

	/// <summary>
	/// Все метки
	/// </summary>
	public static readonly string[] All = {Route};
}