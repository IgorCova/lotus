using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Prometheus;

namespace LotusWebApp.Metrics;

/// <summary>
/// Метрики <a href="https://en.wikipedia.org/wiki/Apdex">Apdex</a>
/// </summary>
internal interface IApdexMetrics
{
	/// <summary>
	/// Обновляет метрики Apdex
	/// </summary>
	/// <param name="satisfiedResponseTime">Удовлетворительная продолжительность ответа</param>
	/// <param name="route">Роут запроса для которого собираются метрики</param>
	/// <param name="duration">Продолжительность запроса</param>
	/// <param name="statusCode">код ответа</param>
	/// <param name="additionalSuccessCodes">Список дополнительных успешных statusCode</param>
	void Update(TimeSpan satisfiedResponseTime, string route, TimeSpan duration, int statusCode, int[]? additionalSuccessCodes = null);
}

/// <summary>
/// Метрики <a href="https://en.wikipedia.org/wiki/Apdex">Apdex</a>
/// </summary>
internal class ApdexMetrics : IApdexMetrics
{
	/// <summary>
	/// Счетчик запросов с временем выполнения не превыщащим T
	/// </summary>
	private readonly Counter _apdexSatisfiedCounter;

	/// <summary>
	/// Счетчик запросов с временем выполнения между T и 4*T(включительно)
	/// </summary>
	private readonly Counter _apdexToleratingCounter;

	/// <summary>
	/// Cчетчик общего кол-ва запросов
	/// </summary>
	private readonly Counter _apdexTotalCounter;

	/// <summary>
	/// .ctor
	/// </summary>
	public ApdexMetrics()
	{
		// See https://prometheus.io/docs/practices/naming/

		_apdexSatisfiedCounter = Prometheus.Metrics.CreateCounter(
			"apdex_satisfied",
			"The responses number satisfied by x < T threshold time condition",
			new CounterConfiguration
			{
				LabelNames = ApdexLabelNames.All
			}
		);

		_apdexToleratingCounter = Prometheus.Metrics.CreateCounter(
			"apdex_tolerating",
			"The responses number tolerating by T < x <= 4 * T threshold time condition",
			new CounterConfiguration
			{
				LabelNames = ApdexLabelNames.All
			}
		);

		_apdexTotalCounter = Prometheus.Metrics.CreateCounter(
			"apdex_total",
			"The total responses number",
			new CounterConfiguration
			{
				LabelNames = ApdexLabelNames.All
			}
		);
	}

	/// <summary>
	/// Обновляет метрики Apdex
	/// </summary>
	/// <param name="satisfiedResponseTime">Удовлетворительная продолжительность ответа</param>
	/// <param name="route">Роут запроса для которого собираются метрики</param>
	/// <param name="duration">Продолжительность запроса</param>
	/// <param name="statusCode">код ответа</param>
	/// <param name="additionalSuccessCodes">Список дополнительных успешных statusCode</param>
	public void Update(TimeSpan satisfiedResponseTime, string route, TimeSpan duration, int statusCode, int[]? additionalSuccessCodes = null)
	{
		var labels = new Dictionary<string, string>
		{
			{ ApdexLabelNames.Route, route },
		};
		_apdexTotalCounter.WithLabels(labels).Inc();

		var t = satisfiedResponseTime;
		var t4 = satisfiedResponseTime * 4;

		// Для ошибок и долгих запросов ничего не делаем
		if (statusCode > 299 && (additionalSuccessCodes == null || !additionalSuccessCodes.Contains(statusCode))
		    || duration > t4) return;

		if (duration <= t)
		{
			_apdexSatisfiedCounter.WithLabels(labels).Inc();
			_apdexToleratingCounter.WithLabels(labels).Publish(); // для появления метрик с нулевым значением
		}
		if (duration > t && duration <= t4)
		{
			_apdexSatisfiedCounter.WithLabels(labels).Publish(); // для появления метрик с нулевым значением
			_apdexToleratingCounter.WithLabels(labels).Inc();
		}
	}
}