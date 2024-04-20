using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Prometheus;

namespace LotusWebApp.Metrics;

internal static class CollectorExtensions
{
	public static TChild WithLabels<TChild>(this ICollector<TChild> collector, IReadOnlyDictionary<string, string> labels) where TChild : ChildBase
	{
		var labelValues = new string[collector.LabelNames.Length];
		for (var i = 0; i < labelValues.Length; i++)
		{
			labelValues[i] = labels[collector.LabelNames[i]];
		}

		return collector.WithLabels(labelValues);
	}

	public static string GetRouteTemplateWithMethod(this HttpContext context)
	{
		var method = context.Request.Method.ToUpperInvariant();
		var epf = context.Features.Get<IEndpointFeature>();
		var routePattern = (epf?.Endpoint as RouteEndpoint)?.RoutePattern;
		if (routePattern == null)
		{
			// Если не нашли роута соответствующего запросу, значит такой запрос не обрабатывается API.
			// Чтобы не замусоривать метрики не указываем в метке адрес запроса.
			return "{unknown}";
		}

		var sb = new StringBuilder();
		sb.Append(method).Append(' ');

		foreach (var segment in routePattern.PathSegments)
		{
			sb.Append('/');

			foreach (var segmentPart in segment.Parts)
			{
				switch (segmentPart.PartKind)
				{
					case RoutePatternPartKind.Literal:
						sb.Append(((RoutePatternLiteralPart) segmentPart).Content);
						break;
					case RoutePatternPartKind.Parameter:
						var pp = (RoutePatternParameterPart) segmentPart;
						switch (pp.ParameterKind)
						{
							case RoutePatternParameterKind.Standard:
								// Если это параметр version, то подставляем в шаблон указанную в запросе версию.
								if (pp.Name == "version")
								{
									// Получаем данные о версии API для текущего запроса.
									var versioningFeature = context.Features.Get<IApiVersioningFeature>();
									// Приводим формат версии API к одному виду. См. https://github.com/microsoft/aspnet-api-versioning/wiki/Version-Format
									sb.Append(versioningFeature?.RequestedApiVersion?.ToString("VV"));
									break;
								}
								sb.Append('{').Append(pp.Name).Append('}');
								break;
							case RoutePatternParameterKind.Optional:
								sb.Append('{').Append(pp.Name).Append('?').Append('}');
								break;
							case RoutePatternParameterKind.CatchAll:
								sb.Append("{*}");
								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
						break;
					case RoutePatternPartKind.Separator:
						sb.Append(((RoutePatternSeparatorPart) segmentPart).Content);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		return sb.ToString();
	}
}