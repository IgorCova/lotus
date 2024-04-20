namespace LotusWebApp.Metrics
{
	/// <summary>
	/// Опции сбора метрик http-запросов
	/// </summary>
	public class HttpRequestMetricsOptions
	{
		/// <summary>
		/// Бакеты для гистограммы длительности запросов по умолчанию
		/// </summary>
		private static readonly double[] DefaultBuckets =
			{.001, .005, .01, .025, .05, .1, .25, .5, 1, 2.5, 5, 10, 15, 30};

		/// <summary>
		/// Игнорируемые шаблоны адресов по умолчанию
		/// </summary>
		private static readonly string[] DefaultIgnoredPaths =
			{"/internal", "/swagger", "/favicon.ico"};

		/// <summary>
		/// Бакеты для гистограммы длительности запросов
		/// См. https://prometheus.io/docs/practices/histograms/#errors-of-quantile-estimation
		/// </summary>
		public double[] Buckets { get; set; }

		/// <summary>
		/// Игнорируемые шаблоны адресов
		/// </summary>
		public string[] IgnoredPaths { get; set; }

		/// <summary>
		/// .ctor
		/// </summary>
		public HttpRequestMetricsOptions()
		{
			Buckets = DefaultBuckets;
			IgnoredPaths = DefaultIgnoredPaths;
		}
	}
}