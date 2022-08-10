// <copyright file="MetricServerMiddlewareExtensions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusMetricServerMiddlewareExtensions = Prometheus.MetricServerMiddlewareExtensions;

namespace Pulse.Prometheus.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Pulse.Interfaces;

    /// <summary>
    /// Set of extensions for adding metrics endpoints.
    /// </summary>
    public static class MetricServerMiddlewareExtensions
    {
        /// <summary>
        /// Starts a Prometheus metrics exporter using endpoint routing.
        /// The default URL is /metrics, which is a Prometheus convention.
        /// Use methods on the <see cref="IMetricFactory"/> interface to create your metrics.
        /// </summary>
        /// <param name="endpoints"><see cref="IEndpointRouteBuilder"/>.</param>
        /// <param name="pattern"><see cref="string"/> endpoint pattern.</param>
        /// <returns><see cref="IEndpointConventionBuilder"/>.</returns>
        public static IEndpointConventionBuilder MapMetrics(this IEndpointRouteBuilder endpoints, string pattern = "/metrics")
        {
            return PrometheusMetricServerMiddlewareExtensions.MapMetrics(endpoints, pattern);
        }
    }
}
