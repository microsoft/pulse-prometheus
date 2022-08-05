// <copyright file="FactoryTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusMetricConfiguration = Prometheus.MetricConfiguration;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.UnitTests.Factories
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Factories;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for metric factory.
    /// </summary>
    [TestClass]
    public class FactoryTests
    {
        private static readonly Mock<IPrometheusMetricFactoryAdapter> MockMetricsFactory = new ();

        private static readonly Dictionary<string, string> UserStaticLabels = new () { { "user", "labels" }, { "baz", "bot" } };

        private static readonly string[] OneUserLabelName = new string[] { "taz" };

        private static readonly string[] MultipleUserLabelNames = new string[] { "taz", "spaz" };

        private static readonly double[] Buckets = new double[] { 1.0, 2.0, 3.0 };

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
        /// Tests that we create a counter correctly when providing no labels.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithNoLabels()
        {
            var counter = metricFactory.CreateCounter("foo", "bar");
            PrometheusCounterConfiguration expectedCounterConfiguration = CreateCounterConfiguration(null, null, false);

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing static labels.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithStaticLabels()
        {
            var counter = metricFactory.CreateCounterWithStaticLabels("foo", "bar", UserStaticLabels);
            PrometheusCounterConfiguration expectedCounterConfiguration = CreateCounterConfiguration(UserStaticLabels, null, false);

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing one user label.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithOneLabelNames()
        {
            var counter = metricFactory.CreateCounter("foo", "bar", "taz");
            PrometheusCounterConfiguration expectedCounterConfiguration = CreateCounterConfiguration(null, OneUserLabelName, false);

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing multiple user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithMultipleLabelNames()
        {
            var counter = metricFactory.CreateCounter("foo", "bar", "taz", "spaz");
            PrometheusCounterConfiguration expectedCounterConfiguration = CreateCounterConfiguration(null, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a counter correctly when providing static labels and user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateCounterWithStaticLabelsAndLabelNames()
        {
            var counter = metricFactory.CreateCounterWithStaticLabels("foo", "bar", UserStaticLabels, "taz", "spaz");
            PrometheusCounterConfiguration expectedCounterConfiguration = CreateCounterConfiguration(UserStaticLabels, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateCounter("foo", "bar", It.Is<PrometheusCounterConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedCounterConfiguration))), Times.Once);
        }

        /********************************** GAUGE FACTORY TESTS **********************************/

        /// <summary>
        /// Tests that we create a gauge correctly when providing no labels.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithNoLabels()
        {
            var gauge = metricFactory.CreateGauge("foo", "bar");
            PrometheusGaugeConfiguration expectedGaugeConfiguration = CreateGaugeConfiguration(null, null, false);

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing static labels.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithStaticLabels()
        {
            var gauge = metricFactory.CreateGaugeWithStaticLabels("foo", "bar", UserStaticLabels);
            PrometheusGaugeConfiguration expectedGaugeConfiguration = CreateGaugeConfiguration(UserStaticLabels, null, false);

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing one user label.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithOneLabelName()
        {
            var gauge = metricFactory.CreateGauge("foo", "bar", "taz");
            PrometheusGaugeConfiguration expectedGaugeConfiguration = CreateGaugeConfiguration(null, OneUserLabelName, false);

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing multiple user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithMultipleLabelNames()
        {
            var gauge = metricFactory.CreateGauge("foo", "bar", "taz", "spaz");
            PrometheusGaugeConfiguration expectedGaugeConfiguration = CreateGaugeConfiguration(null, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a gauge correctly when providing static labels and user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateGaugeWithStaticLabelsAndLabelNames()
        {
            var gauge = metricFactory.CreateGaugeWithStaticLabels("foo", "bar", UserStaticLabels, "taz", "spaz");
            PrometheusGaugeConfiguration expectedGaugeConfiguration = CreateGaugeConfiguration(UserStaticLabels, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateGauge("foo", "bar", It.Is<PrometheusGaugeConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedGaugeConfiguration))), Times.Once);
        }

        /******************************** HISTOGRAM FACTORY TESTS ********************************/

        /// <summary>
        /// Tests that we create a histogram correctly when providing no labels.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithNoLabels()
        {
            var histogram = metricFactory.CreateHistogram("foo", "bar", Buckets);
            PrometheusHistogramConfiguration expectedHistogramConfiguration = CreateHistogramConfiguration(Buckets, null, null, false);

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedHistogramConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing static labels.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithStaticLabels()
        {
            var histogram = metricFactory.CreateHistogramWithStaticLabels("foo", "bar", Buckets, UserStaticLabels);
            PrometheusHistogramConfiguration expectedHistogramConfiguration = CreateHistogramConfiguration(Buckets, UserStaticLabels, null, false);

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedHistogramConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing one user label.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithOneLabelName()
        {
            var histogram = metricFactory.CreateHistogram("foo", "bar", Buckets, "taz");
            PrometheusHistogramConfiguration expectedHistogramConfiguration = CreateHistogramConfiguration(Buckets, null, OneUserLabelName, false);

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedHistogramConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a histogram correctly when providing multiple user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithMultipleUserLabelNames()
        {
            var histogram = metricFactory.CreateHistogram("foo", "bar", Buckets, "taz", "spaz");
            PrometheusHistogramConfiguration expectedHistogramConfiguration = CreateHistogramConfiguration(Buckets, null, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedHistogramConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a hitsogram correctly when providing static labels and user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateHistogramWithStaticLabelsAndLabelNames()
        {
            var histogram = metricFactory.CreateHistogramWithStaticLabels("foo", "bar", Buckets, UserStaticLabels, "taz", "spaz");
            PrometheusHistogramConfiguration expectedHistogramConfiguration = CreateHistogramConfiguration(Buckets, UserStaticLabels, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateHistogram("foo", "bar", It.Is<PrometheusHistogramConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedHistogramConfiguration))), Times.Once);
        }

        /********************************* SUMMARY FACTORY TESTS *********************************/

        /// <summary>
        /// Tests that we create a summary correctly when providing no labels.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithNoLabels()
        {
            var summary = metricFactory.CreateSummary("foo", "bar");
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(null, null, null, null, null, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing static labels.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithStaticLabels()
        {
            var summary = metricFactory.CreateSummaryWithStaticLabels("foo", "bar", UserStaticLabels);
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(null, null, null, UserStaticLabels, null, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing one user label.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithOneLabelName()
        {
            var summary = metricFactory.CreateSummary("foo", "bar", null, null, null, "taz");
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(null, null, null, null, OneUserLabelName, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing multiple user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithMultipleLabelNames()
        {
            var summary = metricFactory.CreateSummary("foo", "bar", null, null, null, "taz", "spaz");
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(null, null, null, null, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when providing static labels and user labels.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithStaticLabelsAndLabelNames()
        {
            var summary = metricFactory.CreateSummaryWithStaticLabels("foo", "bar", UserStaticLabels, null, null, null, "taz", "spaz");
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(null, null, null, UserStaticLabels, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /// <summary>
        /// Tests that we create a summary correctly when using no defaults.
        /// </summary>
        [TestMethod]
        public void TestCreateSummaryWithNoDefaults()
        {
            var summary = metricFactory.CreateSummaryWithStaticLabels("foo", "bar", UserStaticLabels, TimeSpan.FromMinutes(12), 5, 1500, "taz", "spaz");
            PrometheusSummaryConfiguration expectedSummaryConfiguration = CreateSummaryConfiguration(TimeSpan.FromMinutes(12), 5, 1500, UserStaticLabels, MultipleUserLabelNames, false);

            MockMetricsFactory.Verify(x => x.CreateSummary("foo", "bar", It.Is<PrometheusSummaryConfiguration>(arg => MatchesExpectedMetricConfiguration(arg, expectedSummaryConfiguration))), Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static IMetricFactory CreateMetricFactory() => new PulseMetricFactory(MockMetricsFactory.Object);

        private static PrometheusCounterConfiguration CreateCounterConfiguration(Dictionary<string, string> staticLabels, string[] labelNames, bool suppressInitialValue)
        {
            return new PrometheusCounterConfiguration()
            {
                StaticLabels = staticLabels,
                LabelNames = labelNames,
                SuppressInitialValue = suppressInitialValue,
            };
        }

        private static PrometheusGaugeConfiguration CreateGaugeConfiguration(Dictionary<string, string> staticLabels, string[] labelNames, bool suppressInitialValue)
        {
            return new PrometheusGaugeConfiguration()
            {
                StaticLabels = staticLabels,
                LabelNames = labelNames,
                SuppressInitialValue = suppressInitialValue,
            };
        }

        private static PrometheusHistogramConfiguration CreateHistogramConfiguration(double[] buckets, Dictionary<string, string> staticLabels, string[] labelNames, bool suppressInitialValue)
        {
            return new PrometheusHistogramConfiguration()
            {
                Buckets = buckets,
                StaticLabels = staticLabels,
                LabelNames = labelNames,
                SuppressInitialValue = suppressInitialValue,
            };
        }

        private static PrometheusSummaryConfiguration CreateSummaryConfiguration(TimeSpan? maxAge, int? ageBuckets, int? bufferSize, Dictionary<string, string> staticLabels, string[] labelNames, bool suppressInitialValue)
        {
            return new PrometheusSummaryConfiguration()
            {
                Objectives = null,
                MaxAge = maxAge ?? PrometheusSummary.DefMaxAge,
                AgeBuckets = ageBuckets ?? PrometheusSummary.DefAgeBuckets,
                BufferSize = bufferSize ?? PrometheusSummary.DefBufCap,
                StaticLabels = staticLabels,
                LabelNames = labelNames,
                SuppressInitialValue = suppressInitialValue,
            };
        }

        private static bool MatchesExpectedMetricConfiguration(PrometheusMetricConfiguration actual, PrometheusMetricConfiguration expected)
        {
            try
            {
                actual.Should().BeEquivalentTo(expected);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
