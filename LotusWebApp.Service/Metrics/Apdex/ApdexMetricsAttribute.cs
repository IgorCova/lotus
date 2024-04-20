using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LotusWebApp.Metrics;

/// <summary>
/// Включает сбор метрик Apdex для помеченного эндпоинта.
/// </summary>
public class ApdexMetricAttribute : ActionFilterAttribute
{
	private readonly TimeSpan _satisfiedResponseTime;
	private readonly int[]? _additionalSuccessCodes;

	/// <summary>
	/// .ctor
	/// </summary>
	/// <param name="durationValue">Удовлетворительное время ответа</param>
	/// <param name="durationUnit">Единица измерения времени</param>
	/// <param name="additionalSuccessCodes">Список дополнительных успешных statusCode (если не хватате 200-ых статусов)</param>
	public ApdexMetricAttribute(int durationValue, DurationUnit durationUnit, int[] additionalSuccessCodes)
	{
		_satisfiedResponseTime = TimeSpan.FromMilliseconds(durationValue * (int)durationUnit);
		_additionalSuccessCodes = additionalSuccessCodes;
	}

	/// <summary>
	/// .ctor
	/// </summary>
	/// <param name="durationValue">Удовлетворительное время ответа</param>
	/// <param name="durationUnit">Единица измерения времени</param>
	public ApdexMetricAttribute(int durationValue, DurationUnit durationUnit)
	{
		_satisfiedResponseTime = TimeSpan.FromMilliseconds(durationValue * (int)durationUnit);
	}

	/// <inheritdoc />
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var feature = new ApdexMetricFeature(_satisfiedResponseTime);
		if (_additionalSuccessCodes != null)
		{
			feature.AdditionalSuccessCodes = _additionalSuccessCodes;
		}
		context.HttpContext.Features.Set((IApdexMetricFeature)feature);
		base.OnActionExecuting(context);
	}
}