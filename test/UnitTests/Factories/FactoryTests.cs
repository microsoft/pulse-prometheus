// <copyright file="FactoryTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusMetricConfiguration = Prometheus.MetricConfiguration;
using PrometheusQuantileEpsilonPair = Prometheus.QuantileEpsilonPair;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.UnitTests.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Configurations;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Factories;
    using Pulse.Prometheus.Histograms;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for metric factory.
    /// </summary>
    [TestClass]
    public class FactoryTests
    {
        private static readonly Mock<IPrometheusMetricFactoryAdapter> MockMetricsFactory = new ();

        private static IMetricFactory metricFactory;

        /// <summary>
        /// Initializes and sets up a mock prometheus metric factory and mock prometheus
        /// environment provider.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockMetricsFactory.Reset();
            metricFactory = CreateMetricFactory();
        }

        /// <summary>
        /// Tests that we implement the IMetricFactory interface.
        /// </summary>
        [TestMethod]
        public void TestImplementsInterface() => CreateMetricFactory();

        /********************************* COUNTER FACTORY TESTS *********************************/

        /// <summary>
        /// Tests that we create a counter correctly when providing no configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithDefaultConfiguration()
        {
            var counter = metricFactory.CreateCounter("foo", "bar");

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", null), Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing immutable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithImmutableLabelsConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };

            CounterConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
            };

            var counter = metricFactory.CreateCounter("foo", "bar", config);
            PrometheusCounterConfiguration expectedCounterConfiguration =
                CreatePrometheusCounterConfiguration(immutableLabels, null, true);

            MockMetricsFactory.Verify(
                x => x.CreateCounter(
                    "foo",
                    "bar",
                    It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing variable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithVariableLabelsConfiguration()
        {
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };

            CounterConfiguration config = new ()
            {
                MutableLabelNames = mutableLabelNames,
            };

            var counter = metricFactory.CreateCounter("foo", "bar", config);
            PrometheusCounterConfiguration expectedCounterConfiguration =
                CreatePrometheusCounterConfiguration(null, mutableLabelNames, true);

            MockMetricsFactory.Verify(
                x => x.CreateCounter(
                    "foo",
                    "bar",
                    It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing publish on create configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithPublishOnCreationConfiguration()
        {
            var publishOnCreation = false;

            CounterConfiguration config = new ()
            {
                PublishOnCreation = publishOnCreation,
            };

            var counter = metricFactory.CreateCounter("foo", "bar", config);
            PrometheusCounterConfiguration expectedCounterConfiguration =
                CreatePrometheusCounterConfiguration(null, null, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateCounter(
                    "foo",
                    "bar",
                    It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing a full configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithFullCustomConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };
            var publishOnCreation = false;

            CounterConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
                MutableLabelNames = mutableLabelNames,
                PublishOnCreation = publishOnCreation,
            };

            var counter = metricFactory.CreateCounter("foo", "bar", config);
            PrometheusCounterConfiguration expectedCounterConfiguration =
                CreatePrometheusCounterConfiguration(immutableLabels, mutableLabelNames, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateCounter(
                    "foo",
                    "bar",
                    It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))),
                Times.Once);
        }

        /********************************** GAUGE FACTORY TESTS **********************************/

        /// <summary>
        /// Tests that we create a gauge correctly when providing no configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithDefaultConfiguration()
        {
            var gauge = metricFactory.CreateGauge("foo", "bar");

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", null), Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing immutable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithImmutableLabelsConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };

            GaugeConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
            };

            var gauge = metricFactory.CreateGauge("foo", "bar", config);
            PrometheusGaugeConfiguration expectedGaugeConfiguration =
                CreatePrometheusGaugeConfiguration(immutableLabels, null, true);

            MockMetricsFactory.Verify(
                x => x.CreateGauge(
                    "foo",
                    "bar",
                    It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing variable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithVariableLabelsConfiguration()
        {
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };

            GaugeConfiguration config = new ()
            {
                MutableLabelNames = mutableLabelNames,
            };

            var gauge = metricFactory.CreateGauge("foo", "bar", config);
            PrometheusGaugeConfiguration expectedGaugeConfiguration =
                CreatePrometheusGaugeConfiguration(null, mutableLabelNames, true);

            MockMetricsFactory.Verify(
                x => x.CreateGauge(
                    "foo",
                    "bar",
                    It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing publish on create configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithPublishOnCreationConfiguration()
        {
            var publishOnCreation = false;

            GaugeConfiguration config = new ()
            {
                PublishOnCreation = publishOnCreation,
            };

            var gauge = metricFactory.CreateGauge("foo", "bar", config);
            PrometheusGaugeConfiguration expectedGaugeConfiguration =
                CreatePrometheusGaugeConfiguration(null, null, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateGauge(
                    "foo",
                    "bar",
                    It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing a configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithFullCustomConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };
            var publishOnCreation = false;

            GaugeConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
                MutableLabelNames = mutableLabelNames,
                PublishOnCreation = publishOnCreation,
            };

            var gauge = metricFactory.CreateGauge("foo", "bar", config);
            PrometheusGaugeConfiguration expectedGaugeConfiguration =
                CreatePrometheusGaugeConfiguration(immutableLabels, mutableLabelNames, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateGauge(
                    "foo",
                    "bar",
                    It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))),
                Times.Once);
        }

        /********************************** HISTOGRAM FACTORY TESTS **********************************/

        /// <summary>
        /// Tests that we create a histogram correctly when providing no configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithDefaultConfiguration()
        {
            var histogram = metricFactory.CreateHistogram("foo", "bar");

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", null), Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing buckets configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithBucketsConfiguration()
        {
            var buckets = new double[] { 1.0, 2.0 };

            HistogramConfiguration config = new ()
            {
                Buckets = buckets,
            };

            var histogram = metricFactory.CreateHistogram("foo", "bar", config);
            PrometheusHistogramConfiguration expectedHistogramConfiguration =
                CreatePrometheusHistogramConfiguration(buckets, null, null, true);

            MockMetricsFactory.Verify(
                x => x.CreateHistogram(
                    "foo",
                    "bar",
                    It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedHistogramConfiguration(arg, expectedHistogramConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing immutable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithImmutableLabelsConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };

            HistogramConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
            };

            var gauge = metricFactory.CreateHistogram("foo", "bar", config);
            PrometheusHistogramConfiguration expectedHistogramConfiguration =
                CreatePrometheusHistogramConfiguration(PulsePrometheusHistogram.DefaultBuckets, immutableLabels, null, true);

            MockMetricsFactory.Verify(
                x => x.CreateHistogram(
                    "foo",
                    "bar",
                    It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedHistogramConfiguration(arg, expectedHistogramConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing variable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithVariableLabelsConfiguration()
        {
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };

            HistogramConfiguration config = new ()
            {
                MutableLabelNames = mutableLabelNames,
            };

            var histogram = metricFactory.CreateHistogram("foo", "bar", config);
            PrometheusHistogramConfiguration expectedHistogramConfiguration =
                CreatePrometheusHistogramConfiguration(PulsePrometheusHistogram.DefaultBuckets, null, mutableLabelNames, true);

            MockMetricsFactory.Verify(
                x => x.CreateHistogram(
                    "foo",
                    "bar",
                    It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedHistogramConfiguration(arg, expectedHistogramConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing publish on create configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithPublishOnCreationConfiguration()
        {
            var publishOnCreation = false;

            HistogramConfiguration config = new ()
            {
                PublishOnCreation = publishOnCreation,
            };

            var histogram = metricFactory.CreateHistogram("foo", "bar", config);
            PrometheusHistogramConfiguration expectedHistogramConfiguration =
                CreatePrometheusHistogramConfiguration(PulsePrometheusHistogram.DefaultBuckets, null, null, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateHistogram(
                    "foo",
                    "bar",
                    It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedHistogramConfiguration(arg, expectedHistogramConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing a configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithFullCustomConfiguration()
        {
            var buckets = new double[] { 1.0, 2.0 };
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };
            var publishOnCreation = false;

            HistogramConfiguration config = new ()
            {
                Buckets = buckets,
                ImmutableLabels = immutableLabels,
                MutableLabelNames = mutableLabelNames,
                PublishOnCreation = publishOnCreation,
            };

            var histogram = metricFactory.CreateHistogram("foo", "bar", config);
            PrometheusHistogramConfiguration expectedHistogramConfiguration =
                CreatePrometheusHistogramConfiguration(buckets, immutableLabels, mutableLabelNames, publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateHistogram(
                    "foo",
                    "bar",
                    It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedHistogramConfiguration(arg, expectedHistogramConfiguration))),
                Times.Once);
        }

        /********************************** SUMMARY FACTORY TESTS **********************************/

        /// <summary>
        /// Tests that we create a summary correctly when providing no configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithDefaultConfiguration()
        {
            var summary = metricFactory.CreateSummary("foo", "bar");

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", null), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing age buckets configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithAgeBucketsConfiguration()
        {
            uint ageBuckets = 3;

            SummaryConfiguration config = new ()
            {
                AgeBuckets = ageBuckets,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    PrometheusSummary.DefMaxAge,
                    Convert.ToInt32(ageBuckets),
                    PrometheusSummary.DefBufCap,
                    null,
                    null,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing buffer size configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithBufferSizeConfiguration()
        {
            uint bufferSize = 1000;

            SummaryConfiguration config = new ()
            {
                BufferSize = bufferSize,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    PrometheusSummary.DefMaxAge,
                    PrometheusSummary.DefAgeBuckets,
                    Convert.ToInt32(bufferSize),
                    null,
                    null,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing max age configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithMaxAgeConfiguration()
        {
            var maxAge = TimeSpan.FromMinutes(60);

            SummaryConfiguration config = new ()
            {
                MaxAge = maxAge,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    maxAge,
                    PrometheusSummary.DefAgeBuckets,
                    PrometheusSummary.DefBufCap,
                    null,
                    null,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing objectives configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithObjectivesConfiguration()
        {
            IReadOnlyList<(double, double)> objectives = new[] { (1.0, 2.0), (2.0, 3.0) };

            SummaryConfiguration config = new ()
            {
                Objectives = objectives,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    objectives,
                    PrometheusSummary.DefMaxAge,
                    PrometheusSummary.DefAgeBuckets,
                    PrometheusSummary.DefBufCap,
                    null,
                    null,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing immutable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithImmutableLabelsConfiguration()
        {
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };

            SummaryConfiguration config = new ()
            {
                ImmutableLabels = immutableLabels,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    PrometheusSummary.DefMaxAge,
                    PrometheusSummary.DefAgeBuckets,
                    PrometheusSummary.DefBufCap,
                    immutableLabels,
                    null,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing variable labels configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithVariableLabelsConfiguration()
        {
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };

            SummaryConfiguration config = new ()
            {
                MutableLabelNames = mutableLabelNames,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    PrometheusSummary.DefMaxAge,
                    PrometheusSummary.DefAgeBuckets,
                    PrometheusSummary.DefBufCap,
                    null,
                    mutableLabelNames,
                    true);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing publish on creation configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithPublishOnCreationConfiguration()
        {
            var publishOnCreation = false;

            SummaryConfiguration config = new ()
            {
                PublishOnCreation = publishOnCreation,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    null,
                    PrometheusSummary.DefMaxAge,
                    PrometheusSummary.DefAgeBuckets,
                    PrometheusSummary.DefBufCap,
                    null,
                    null,
                    publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing a configuration.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithFullCustomConfiguration()
        {
            uint ageBuckets = 3;
            uint bufferSize = 1000;
            var maxAge = TimeSpan.FromMinutes(60);
            IReadOnlyList<(double, double)> objectives = new[] { (1.0, 2.0), (2.0, 3.0) };
            var immutableLabels = new Dictionary<string, string>() { { "foo", "bar" }, { "baz", "spaz" } };
            var mutableLabelNames = new string[] { "poo", "shoe", "phew" };
            var publishOnCreation = false;

            SummaryConfiguration config = new ()
            {
                AgeBuckets = ageBuckets,
                BufferSize = bufferSize,
                ImmutableLabels = immutableLabels,
                MaxAge = maxAge,
                PublishOnCreation = publishOnCreation,
                Objectives = objectives,
                MutableLabelNames = mutableLabelNames,
            };

            var summary = metricFactory.CreateSummary("foo", "bar", config);
            PrometheusSummaryConfiguration expectedSummaryConfiguration =
                CreatePrometheusSummaryConfiguration(
                    objectives,
                    maxAge,
                    Convert.ToInt32(ageBuckets),
                    Convert.ToInt32(bufferSize),
                    immutableLabels,
                    mutableLabelNames,
                    publishOnCreation);

            MockMetricsFactory.Verify(
                x => x.CreateSummary(
                    "foo",
                    "bar",
                    It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedSummaryConfiguration(arg, expectedSummaryConfiguration))),
                Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static IMetricFactory CreateMetricFactory() => new PulseMetricFactory(MockMetricsFactory.Object);

        private static PrometheusCounterConfiguration CreatePrometheusCounterConfiguration(Dictionary<string, string> immutableLabels, string[] mutableLabelNames, bool publishOnCreation)
        {
            return new PrometheusCounterConfiguration()
            {
                StaticLabels = immutableLabels,
                LabelNames = mutableLabelNames,
                SuppressInitialValue = !publishOnCreation,
            };
        }

        private static PrometheusGaugeConfiguration CreatePrometheusGaugeConfiguration(Dictionary<string, string> immutableLabels, string[] mutableLabelNames, bool publishOnCreation)
        {
            return new PrometheusGaugeConfiguration()
            {
                StaticLabels = immutableLabels,
                LabelNames = mutableLabelNames,
                SuppressInitialValue = !publishOnCreation,
            };
        }

        private static PrometheusHistogramConfiguration CreatePrometheusHistogramConfiguration(double[] buckets, Dictionary<string, string> immutableLabels, string[] mutableLabelNames, bool publishOnCreation)
        {
            return new PrometheusHistogramConfiguration()
            {
                Buckets = buckets,
                StaticLabels = immutableLabels,
                LabelNames = mutableLabelNames,
                SuppressInitialValue = !publishOnCreation,
            };
        }

        private static PrometheusSummaryConfiguration CreatePrometheusSummaryConfiguration(IReadOnlyList<(double quantile, double epsilon)> objectives, TimeSpan maxAge, int ageBuckets, int bufferSize, Dictionary<string, string> immutableLabels, string[] mutableLabelNames, bool publishOnCreation)
        {
            return new PrometheusSummaryConfiguration()
            {
                Objectives = ConvertObjectivesToQuantileEpsilonPairs(objectives),
                MaxAge = maxAge,
                AgeBuckets = ageBuckets,
                BufferSize = bufferSize,
                StaticLabels = immutableLabels,
                LabelNames = mutableLabelNames,
                SuppressInitialValue = !publishOnCreation,
            };
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

        private static bool MatchesExpectedMetricConfiguration(PrometheusMetricConfiguration actual, PrometheusMetricConfiguration expected)
        {
            bool configsMatch = true;

            if ((actual.LabelNames == null && expected.LabelNames != null) ||
                (actual.LabelNames != null && expected.LabelNames == null) ||
                (actual.StaticLabels == null && expected.StaticLabels != null) ||
                (actual.StaticLabels != null && expected.StaticLabels == null))
            {
                return false;
            }
            else if (actual.LabelNames != null && expected.LabelNames != null)
            {
                configsMatch = configsMatch && actual.LabelNames.SequenceEqual(expected.LabelNames);
            }
            else if (actual.StaticLabels != null && expected.StaticLabels != null)
            {
                configsMatch = configsMatch && actual.StaticLabels.Count == expected.StaticLabels.Count && !actual.StaticLabels.Except(expected.StaticLabels).Any();
            }

            return configsMatch && actual.SuppressInitialValue == expected.SuppressInitialValue;
        }

        private static bool MatchesExpectedHistogramConfiguration(PrometheusHistogramConfiguration actual, PrometheusHistogramConfiguration expected)
        {
            bool configsMatch = true;

            if ((actual.Buckets != null && expected.Buckets == null) ||
                (actual.Buckets == null && expected.Buckets != null))
            {
                return false;
            }
            else if (actual.Buckets == null && expected.Buckets == null)
            {
                configsMatch = configsMatch && actual.Buckets.SequenceEqual(expected.Buckets);
            }

            return configsMatch && MatchesExpectedMetricConfiguration(actual, expected);
        }

        private static bool MatchesExpectedSummaryConfiguration(PrometheusSummaryConfiguration actual, PrometheusSummaryConfiguration expected)
        {
            bool configsMatch = true;

            if ((actual.Objectives != null && expected.Objectives == null) ||
                (actual.Objectives == null && expected.Objectives != null))
            {
                return false;
            }
            else if (actual.Objectives != null && expected.Objectives != null)
            {
                configsMatch = configsMatch && actual.Objectives.SequenceEqual(expected.Objectives);
            }

            return
                configsMatch &&
                MatchesExpectedMetricConfiguration(actual, expected) &&
                actual.AgeBuckets == expected.AgeBuckets &&
                actual.MaxAge.CompareTo(expected.MaxAge) == 0 &&
                actual.BufferSize == expected.BufferSize;
        }
    }
}
