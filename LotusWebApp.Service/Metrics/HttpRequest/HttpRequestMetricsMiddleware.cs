using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace LotusWebApp.Metrics
{
	/// <summary>
	/// Мидлварь, собирающая метрики по запросам к сервису.
	/// </summary>
	internal class HttpRequestMetricsMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IApdexMetrics _apdex;

		public HttpRequestMetricsMiddleware(RequestDelegate next, HttpRequestMetricsOptions options, IApdexMetrics apdex)
		{
			_next = next;
			_apdex = apdex;

			// See https://prometheus.io/docs/practices/naming/

			_requestsDurationHistogram = Prometheus.Metrics.CreateHistogram(
				"http_request_duration_seconds",
				"The duration of HTTP requests processed by the service.",
				new HistogramConfiguration
				{
					LabelNames = HttpRequestLabelNames.All,
					Buckets = options.Buckets
				}
			);

			_activeRequestsGauge = Prometheus.Metrics.CreateGauge(
				"http_requests_active",
				"The number of requests currently in progress."
			);

			_requestsTotalCounter = Prometheus.Metrics.CreateCounter(
				"http_requests_total",
				"The count of HTTP requests that have been processed by the service.",
				HttpRequestLabelNames.All // Размечаем счётчик всеми доступными метками
			);

			_ignoredPaths = options.IgnoredPaths.Select(p => new PathString(p)).ToArray();
		}

		/// <summary>
		/// Количество текущих запросов
		/// </summary>
		private readonly Gauge _activeRequestsGauge;

		/// <summary>
		/// Общее количество запросов
		/// </summary>
		private readonly Counter _requestsTotalCounter;

		/// <summary>
		/// Гистограмма длительности запросов
		/// </summary>
		private readonly Histogram _requestsDurationHistogram;

		/// <summary>
		/// Запросы, на э/п, начинающиеся на эти строки, не включаем в метрики.
		/// </summary>
		private readonly PathString[] _ignoredPaths;

		public async Task Invoke(HttpContext context)
		{
			if (_ignoredPaths.Any(p =>
				context.Request.Path.StartsWithSegments(p, StringComparison.InvariantCultureIgnoreCase)))
			{
				await _next(context);
				return;
			}

			using (_activeRequestsGauge.TrackInProgress())
			{
				var sw = Stopwatch.StartNew();

				try
				{
					await _next(context);
				}
				finally
				{
					// Собираем метки после выполнения остальных обработчиков, чтобы
					// получить актуальный код ответа.
					var route = context.GetRouteTemplateWithMethod();
					var statusCode = context.Response.StatusCode;
					var labels = GetLabels(statusCode, route);
					var apdexFeature = context.Features.Get<IApdexMetricFeature>();

					sw.Stop();
					_requestsDurationHistogram.WithLabels(labels).Observe(sw.Elapsed.TotalSeconds);
					_requestsTotalCounter.WithLabels(labels).Inc();

					// Собираем Apdex метрики
					if (apdexFeature != null)
					{
						_apdex.Update(apdexFeature.SatisfiedResponseTime,
							route,
							sw.Elapsed,
							statusCode,
							apdexFeature.AdditionalSuccessCodes);
					}
				}
			}
		}

		private static IReadOnlyDictionary<string, string> GetLabels(int statusCode, string route)
		{
			var result = new Dictionary<string, string>();
			for (var i = 0; i < HttpRequestLabelNames.All.Length; i++)
			{
				var labelName = HttpRequestLabelNames.All[i];

				result[labelName] = labelName switch
				{
					HttpRequestLabelNames.StatusCode => statusCode.ToString(CultureInfo.InvariantCulture),
					HttpRequestLabelNames.Route => route,
					_ => "{unknown}"
				};
			}

			return result;
		}
	}
}