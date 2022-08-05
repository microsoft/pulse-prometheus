// <copyright file="IPrometheusMetricFactoryAdapter.cs" company="Microsoft Corporation">
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
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.Interfaces
{
    /// <summary>
    /// Mockable interface to Prometheus.IMetricFactory.
    /// </summary>
    public interface IPrometheusMetricFactoryAdapter
    {
        /// <summary>
        /// Creates a prometheus counter.
        /// </summary>
        /// <param name="name"><see cref="string"/>Metric name.</param>
        /// <param name="help"><see cref="string"/>Metric description.</param>
        /// <param name="configuration"><see cref="PrometheusCounterConfiguration"/>.</param>
        /// <returns><see cref="PrometheusCounter"/>.</returns>
        public PrometheusCounter CreateCounter(string name, string help, PrometheusCounterConfiguration configuration = null);

        /// <summary>
        /// Creates a prometheus gauge.
        /// </summary>
        /// <param name="name"><see cref="string"/>Metric name.</param>
        /// <param name="help"><see cref="string"/>Metric description.</param>
        /// <param name="configuration"><see cref="PrometheusGaugeConfiguration"/>.</param>
        /// <returns><see cref="PrometheusGauge"/>.</returns>
        public PrometheusGauge CreateGauge(string name, string help, PrometheusGaugeConfiguration configuration = null);

        /// <summary>
        /// Creates a prometheus histogram.
        /// </summary>
        /// <param name="name"><see cref="string"/>Metric name.</param>
        /// <param name="help"><see cref="string"/>Metric description.</param>
        /// <param name="configuration"><see cref="PrometheusHistogramConfiguration"/>.</param>
        /// <returns><see cref="PrometheusHistogram"/>.</returns>
        public PrometheusHistogram CreateHistogram(string name, string help, PrometheusHistogramConfiguration configuration = null);

        /// <summary>
        /// Creates a prometheus summary.
        /// </summary>
        /// <param name="name"><see cref="string"/>Metric name.</param>
        /// <param name="help"><see cref="string"/>Metric description.</param>
        /// <param name="configuration"><see cref="PrometheusSummaryConfiguration"/>.</param>
        /// <returns><see cref="PrometheusSummary"/>.</returns>
        public PrometheusSummary CreateSummary(string name, string help, PrometheusSummaryConfiguration configuration = null);
    }
}
