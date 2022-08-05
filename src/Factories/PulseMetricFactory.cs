// <copyright file="PulseMetricFactory.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.Factories
{
    using System;
    using System.Collections.Generic;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Counters;
    using Pulse.Prometheus.Gauges;
    using Pulse.Prometheus.Histograms;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Summaries;

    /// <summary>
    /// Factory for creating metric instances.
    /// </summary>
    public class PulseMetricFactory : IMetricFactory
    {
        private static readonly bool SuppressInitialValue = false;

        private readonly IPrometheusMetricFactoryAdapter prometheusFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulseMetricFactory"/> class.
        /// </summary>
        /// <param name="metricFactory"><see cref="IPrometheusMetricFactoryAdapter"/>.</param>
        public PulseMetricFactory(IPrometheusMetricFactoryAdapter metricFactory)
        {
            prometheusFactory = metricFactory;
        }

        /// <inheritdoc/>
        public ICounter CreateCounter(string name, string desc, params string[] labelNames)
        {
            return CreateCounterWithStaticLabels(name, desc, null, labelNames);
        }

        /// <inheritdoc/>
        public ICounter CreateCounterWithStaticLabels(string name, string desc, Dictionary<string, string> staticLabels, params string[] labelNames)
        {
            var config = new PrometheusCounterConfiguration()
            {
                StaticLabels = staticLabels,
                LabelNames = GetLabelNames(labelNames),
                SuppressInitialValue = SuppressInitialValue,
            };

            return new PulsePrometheusCounter(new PrometheusCounterAdapter(prometheusFactory.CreateCounter(name, desc, config)));
        }

        /// <inheritdoc/>
        public IGauge CreateGauge(string name, string desc, params string[] labelNames)
        {
            return CreateGaugeWithStaticLabels(name, desc, null, labelNames);
        }

        /// <inheritdoc/>
        public IGauge CreateGaugeWithStaticLabels(string name, string desc, Dictionary<string, string> staticLabels, params string[] labelNames)
        {
            var config = new PrometheusGaugeConfiguration()
            {
                StaticLabels = staticLabels,
                LabelNames = GetLabelNames(labelNames),
                SuppressInitialValue = SuppressInitialValue,
            };

            return new PulsePrometheusGauge(new PrometheusGaugeAdapter(prometheusFactory.CreateGauge(name, desc, config)));
        }

        /// <inheritdoc/>
        public IHistogram CreateHistogram(string name, string desc, double[] buckets, params string[] labelNames)
        {
            return CreateHistogramWithStaticLabels(name, desc, buckets, null, labelNames);
        }

        /// <inheritdoc/>
        public IHistogram CreateHistogramWithStaticLabels(string name, string desc, double[] buckets, Dictionary<string, string> staticLabels, params string[] labelNames)
        {
            var config = new PrometheusHistogramConfiguration()
            {
                Buckets = buckets,
                StaticLabels = staticLabels,
                LabelNames = GetLabelNames(labelNames),
                SuppressInitialValue = SuppressInitialValue,
            };

            return new PulsePrometheusHistogram(new PrometheusHistogramAdapter(prometheusFactory.CreateHistogram(name, desc, config)));
        }

        /// <inheritdoc/>
        public ISummary CreateSummary(string name, string desc, TimeSpan? maxAge = null, int? ageBuckets = null, int? bufferSize = null, params string[] labelNames)
        {
            return CreateSummaryWithStaticLabels(name, desc, null, maxAge, ageBuckets, bufferSize, labelNames);
        }

        /// <inheritdoc/>
        public ISummary CreateSummaryWithStaticLabels(string name, string desc, Dictionary<string, string> staticLabels, TimeSpan? maxAge = null, int? ageBuckets = null, int? bufferSize = null, params string[] labelNames)
        {
            var config = new PrometheusSummaryConfiguration()
            {
                Objectives = null,
                MaxAge = maxAge ?? PrometheusSummary.DefMaxAge,
                AgeBuckets = ageBuckets ?? PrometheusSummary.DefAgeBuckets,
                BufferSize = bufferSize ?? PrometheusSummary.DefBufCap,
                StaticLabels = staticLabels,
                LabelNames = GetLabelNames(labelNames),
                SuppressInitialValue = SuppressInitialValue,
            };

            return new PulsePrometheusSummary(new PrometheusSummaryAdapter(prometheusFactory.CreateSummary(name, desc, config)));
        }

        private static string[] GetLabelNames(string[] labelNames) => labelNames.Length == 0 ? null : labelNames;
    }
}
