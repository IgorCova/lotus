using System;

namespace LotusWebApp.Metrics
{
	/// <summary>
	/// Содержит удовлетворительное время ответа для данного запроса.
	/// Для сбора метрик <a href="https://en.wikipedia.org/wiki/Apdex">Apdex</a>
	/// </summary>
	internal interface IApdexMetricFeature
	{
		/// <summary>
		/// Удовлетворительная продолжительность ответа
		/// </summary>
		TimeSpan SatisfiedResponseTime { get; }

		/// <summary>
		/// Дополнительный список успешных statusCode
		/// </summary>
		int[]? AdditionalSuccessCodes { get; }
	}

	/// <summary>
	/// Опции сбора <a href="https://en.wikipedia.org/wiki/Apdex">Apdex</a> метрик
	/// </summary>
	/// <param name="SatisfiedResponseTime">Удовлетворительная продолжительность ответа</param>
	internal sealed record ApdexMetricFeature(TimeSpan SatisfiedResponseTime) : IApdexMetricFeature
	{
		/// <summary>
		/// Список дополнительных успешных statusCode
		/// </summary>
		public int[]? AdditionalSuccessCodes { get; set; }
	}
}