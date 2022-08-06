// <copyright file="PulseMetricFactory.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusQuantileEpsilonPair = Prometheus.QuantileEpsilonPair;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Pulse.Configurations;
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
        public ICounter CreateCounter(string name, string desc, CounterConfiguration config = null)
        {
            if (config == null)
            {
                return new PulsePrometheusCounter(new PrometheusCounterAdapter(prometheusFactory.CreateCounter(name, desc)));
            }

            var prometheusCounterConfig = new PrometheusCounterConfiguration()
            {
                LabelNames = config.MutableLabelNames,
                StaticLabels = config.ImmutableLabels,
                SuppressInitialValue = !config.PublishOnCreation,
            };

            return new PulsePrometheusCounter(new PrometheusCounterAdapter(prometheusFactory.CreateCounter(name, desc, prometheusCounterConfig)));
        }

        /// <inheritdoc/>
        public IGauge CreateGauge(string name, string desc, GaugeConfiguration config = null)
        {
            if (config == null)
            {
                return new PulsePrometheusGauge(new PrometheusGaugeAdapter(prometheusFactory.CreateGauge(name, desc)));
            }

            var prometheusGaugeConfig = new PrometheusGaugeConfiguration()
            {
                LabelNames = config.MutableLabelNames,
                StaticLabels = config.ImmutableLabels,
                SuppressInitialValue = !config.PublishOnCreation,
            };

            return new PulsePrometheusGauge(new PrometheusGaugeAdapter(prometheusFactory.CreateGauge(name, desc, prometheusGaugeConfig)));
        }

        /// <inheritdoc/>
        public IHistogram CreateHistogram(string name, string desc, HistogramConfiguration config = null)
        {
            if (config == null)
            {
                return new PulsePrometheusHistogram(new PrometheusHistogramAdapter(prometheusFactory.CreateHistogram(name, desc)));
            }

            var prometheusHistogramConfig = new PrometheusHistogramConfiguration()
            {
                Buckets = config.Buckets ?? PulsePrometheusHistogram.DefaultBuckets,
                LabelNames = config.MutableLabelNames,
                StaticLabels = config.ImmutableLabels,
                SuppressInitialValue = !config.PublishOnCreation,
            };

            return new PulsePrometheusHistogram(new PrometheusHistogramAdapter(prometheusFactory.CreateHistogram(name, desc, prometheusHistogramConfig)));
        }

        /// <inheritdoc/>
        public ISummary CreateSummary(string name, string desc, SummaryConfiguration config = null)
        {
            if (config == null)
            {
                return new PulsePrometheusSummary(new PrometheusSummaryAdapter(prometheusFactory.CreateSummary(name, desc)));
            }

            var prometheusSummaryConfig = new PrometheusSummaryConfiguration();

            if (config.AgeBuckets != null)
            {
                prometheusSummaryConfig.AgeBuckets = Convert.ToInt32(config.AgeBuckets);
            }

            if (config.BufferSize != null)
            {
                prometheusSummaryConfig.BufferSize = Convert.ToInt32(config.BufferSize);
            }

            prometheusSummaryConfig.LabelNames = config.MutableLabelNames;
            prometheusSummaryConfig.MaxAge = config.MaxAge ?? PrometheusSummary.DefMaxAge;
            prometheusSummaryConfig.Objectives = ConvertObjectivesToQuantileEpsilonPairs(config.Objectives);
            prometheusSummaryConfig.StaticLabels = config.ImmutableLabels;
            prometheusSummaryConfig.SuppressInitialValue = !config.PublishOnCreation;

            return new PulsePrometheusSummary(new PrometheusSummaryAdapter(prometheusFactory.CreateSummary(name, desc, prometheusSummaryConfig)));
        }

        private static IReadOnlyList<PrometheusQuantileEpsilonPair> ConvertObjectivesToQuantileEpsilonPairs(IReadOnlyList<(double quantile, double epsilon)> objectives)
        {
            if (objectives == null)
            {
                return null;
            }

            IList<PrometheusQuantileEpsilonPair> prometheusObjectives = new List<PrometheusQuantileEpsilonPair>();

            for (int i = 0; i < objectives.Count; i++)
            {
                prometheusObjectives.Add(new PrometheusQuantileEpsilonPair(objectives[i].quantile, objectives[i].epsilon));
            }

            return ImmutableList.CreateRange(prometheusObjectives);
        }
    }
}
