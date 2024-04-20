namespace LotusWebApp.Metrics
{
	/// <summary>
	/// Названия меток для метрик HTTP-запросов
	/// </summary>
	public static class HttpRequestLabelNames
	{
		/// <summary>
		/// Код ответа
		/// </summary>
		public const string StatusCode = "status_code";

		/// <summary>
		/// HTTP-метод + шаблон роута
		/// </summary>
		public const string Route = "route";

		/// <summary>
		/// Все метки
		/// </summary>
		public static readonly string[] All = {StatusCode, Route};
	}
}