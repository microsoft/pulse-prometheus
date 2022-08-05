// <copyright file="PrometheusMetricFactoryAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounter = Prometheus.Counter;
using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusGauge = Prometheus.Gauge;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
using PrometheusHistogram = Prometheus.Histogram;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusMetrics = Prometheus.Metrics;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.Adapters
{
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Mockable adapter around Prometheus.MetricFactory.
    /// </summary>
    public class PrometheusMetricFactoryAdapter : IPrometheusMetricFactoryAdapter
    {
        /// <inheritdoc/>
        public PrometheusCounter CreateCounter(string name, string help, PrometheusCounterConfiguration configuration = null)
            => PrometheusMetrics.CreateCounter(name, help, configuration);

        /// <inheritdoc/>
        public PrometheusGauge CreateGauge(string name, string help, PrometheusGaugeConfiguration configuration = null)
            => PrometheusMetrics.CreateGauge(name, help, configuration);

        /// <inheritdoc/>
        public PrometheusHistogram CreateHistogram(string name, string help, PrometheusHistogramConfiguration configuration = null)
            => PrometheusMetrics.CreateHistogram(name, help, configuration);

        /// <inheritdoc/>
        public PrometheusSummary CreateSummary(string name, string help, PrometheusSummaryConfiguration configuration = null)
            => PrometheusMetrics.CreateSummary(name, help, configuration);
    }
}