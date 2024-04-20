using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LotusWebApp.Metrics
{
	/// <summary>
	/// Регистрация мидлвари
	/// </summary>
	public static class ApplicationBuilderExtensions
	{
		/// <summary>
		/// Добавляет в пайплайн мидлварь для сбора метрик входящих http-запросов
		/// </summary>
		/// <exception cref="ArgumentNullException">Если <paramref name="app"/> <c>null</c></exception>
		/// <exception cref="ArgumentException">Если в опциях не указаны бакеты</exception>
		public static IApplicationBuilder UseHttpRequestMetrics(
			this IApplicationBuilder app,
			Action<HttpRequestMetricsOptions>? configureOptions = null)
		{
			if (app == null)
			{
				throw new ArgumentNullException(nameof(app));
			}

			var options = app.ApplicationServices.GetService<IOptions<HttpRequestMetricsOptions>>()?.Value ??
			              new HttpRequestMetricsOptions();

			configureOptions?.Invoke(options);

			if (options.Buckets == null || options.Buckets.Length == 0)
			{
				throw new ArgumentException("Необходимо указать бакеты для гистограммы");
			}

			app.UseMiddleware<HttpRequestMetricsMiddleware>(options, new ApdexMetrics());

			return app;
		}
	}
}