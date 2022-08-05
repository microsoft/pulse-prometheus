// <copyright file="PrometheusHistogramTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusHistogram = Prometheus.Histogram;
using PrometheusHistogramConfiguration = Prometheus.HistogramConfiguration;
using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.ComponentTests.Adapters
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for the prometheus histogram adapter.
    /// </summary>
    [TestClass]
    public class PrometheusHistogramTests
    {
        private static readonly Random RNG = new ();

        private static readonly IPrometheusMetricFactoryAdapter PromMetricFactory = new PrometheusMetricFactoryAdapter();

        private static PrometheusHistogram histogram;

        private static PrometheusHistogramAdapter histogramAdapter;

        /// <summary>
        /// Initializes a new prometheus histogram and adapter.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            histogram = CreatePrometheusHistogram(RandomMetricName());
            histogramAdapter = new PrometheusHistogramAdapter(histogram);
            VerifySumAndCount();
        }

        /// <summary>
        /// Test histogram's individual observe increments sum and count correctly when called once.
        /// </summary>
        [TestMethod]
        public void TestObserveOneEvent()
        {
            var val = RandomDouble();
            histogramAdapter.Observe(val);
            VerifySumAndCount(val, 1);
        }

        /// <summary>
        /// Test histogram's individual observe increments sum and count correctly when called more than once.
        /// </summary>
        [TestMethod]
        public void TestObserveMultipleEvents()
        {
            var val = RandomDouble();
            histogramAdapter.Observe(val);
            histogramAdapter.Observe(val);
            VerifySumAndCount(val * 2, 2);
        }

        /// <summary>
        /// Test histogram's batch observe increments sum and count correctly when called.
        /// </summary>
        [TestMethod]
        public void TestBatchObserveEvents()
        {
            var val = RandomDouble();
            var count = Convert.ToInt64(RandomDouble());
            histogramAdapter.Observe(val, count);
            VerifySumAndCount(val * count, count);
        }

        /// <summary>
        /// Tests that new timer returns a correct duration.
        /// </summary>
        [TestMethod]
        public void TestNewTimer()
        {
            PrometheusITimer timer;
            using (timer = histogramAdapter.NewTimer())
            {
                Thread.Sleep(10);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(10));

            // sanity check it goes up if we add time
            using (timer = histogramAdapter.NewTimer())
            {
                Thread.Sleep(20);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(20));
        }

        /// <summary>
        /// Tests that we correctly create buckets exponentially.
        /// </summary>
        [TestMethod]
        public void ExponentialBucketsCreatesExpectedBuckets()
        {
            var expected = new[]
            {
                0.0078125,
                0.01171875,
                0.017578125,
                0.0263671875,
                0.03955078125,
                0.059326171875,
                0.0889892578125,
                0.13348388671875,
            };

            var actual = PrometheusHistogramAdapter.ExponentialBuckets(0.0078125, 1.5, 8);

            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Tests that we correctly create buckets linearly.
        /// </summary>
        [TestMethod]
        public void LinearBucketsCreatesExpectedBuckets()
        {
            var expected = new[]
            {
                0.025, 0.050, 0.075, 0.1,
                0.125, 0.150, 0.175, 0.2,
                0.225, 0.250, 0.275, 0.3,
            };

            var actual = PrometheusHistogramAdapter.LinearBuckets(0.025, 0.025, 12);

            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Tests that we correctly create buckets in powers of ten.
        /// </summary>
        [TestMethod]
        public void PowersOfTenDividedBucketsCreatesExpectedBuckets()
        {
            var expected = new[]
            {
                0.025, 0.050, 0.075, 0.1,
                0.25, 0.5, 0.75, 1,
                2.5, 5.0, 7.5, 10,
                25, 50, 75, 100,
            };

            var actual = PrometheusHistogramAdapter.PowersOfTenDividedBuckets(-2, 2, 4);

            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Tests that with labels sets the label values.
        /// </summary>
        [TestMethod]
        public void TestWithLabels()
        {
            IPrometheusHistogramAdapter labelledHistogram = histogramAdapter.WithLabels("foo", "bar", "baz");
            labelledHistogram.Should().NotBeNull();
            labelledHistogram.Should().NotBeSameAs(histogramAdapter);

            Action action = () => labelledHistogram.WithLabels("foo");
            action.Should().Throw<InvalidOperationException>();
        }

        /// <summary>
        /// Tests that with labels chained with a call uses the correct histogram.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsObserve()
        {
            histogramAdapter.WithLabels("foo", "bar", "baz").Sum.Should().Be(histogram.WithLabels("foo", "bar", "baz").Sum);
            histogramAdapter.WithLabels("foo", "bar", "baz").Count.Should().Be(histogram.WithLabels("foo", "bar", "baz").Count);
            histogramAdapter.WithLabels("foo", "bar", "baz").Observe(RNG.NextDouble());
            histogramAdapter.WithLabels("foo", "bar", "baz").Sum.Should().Be(histogram.WithLabels("foo", "bar", "baz").Sum);
            histogramAdapter.WithLabels("foo", "bar", "baz").Count.Should().Be(histogram.WithLabels("foo", "bar", "baz").Count);
        }

        /************************************ HELPER METHODS *************************************/

        private static string RandomMetricName() => $"a{Guid.NewGuid().ToString().Replace("-", string.Empty)}";

        private static PrometheusHistogram CreatePrometheusHistogram(string name) => PromMetricFactory.CreateHistogram(name, string.Empty, new PrometheusHistogramConfiguration() { Buckets = new double[] { 1.0, 2.0, 3.0 }, LabelNames = new string[] { "foo", "bar", "baz" } });

        private static double RandomDouble() => RNG.NextDouble();

        private static void VerifySumAndCount(double sum = 0, long count = 0)
        {
            histogramAdapter.Sum.Should().Be(sum);
            histogramAdapter.Count.Should().Be(count);
        }
    }
}
