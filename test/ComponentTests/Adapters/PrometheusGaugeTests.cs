// <copyright file="PrometheusGaugeTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusGauge = Prometheus.Gauge;
using PrometheusGaugeConfiguration = Prometheus.GaugeConfiguration;
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
    /// Tests for the prometheus gauge adapter.
    /// </summary>
    [TestClass]
    public class PrometheusGaugeTests
    {
        private static readonly Random RNG = new ();

        private static readonly IPrometheusMetricFactoryAdapter PromMetricFactory = new PrometheusMetricFactoryAdapter();

        private static PrometheusGauge gauge;

        private static PrometheusGaugeAdapter gaugeAdapter;

        /// <summary>
        /// Initializes a new prometheus gauge and adapter.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            gauge = CreatePrometheusGauge(RandomMetricName());
            gaugeAdapter = new PrometheusGaugeAdapter(gauge);
        }

        /// <summary>
        /// Tests that we are able to extract the correct value from gauge.
        /// </summary>
        [TestMethod]
        public void TestValue()
        {
            gaugeAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that the adapters call to Set sets the gauge to the given value.
        /// </summary>
        [TestMethod]
        public void TestSet()
        {
            var val = RandomDouble();
            gaugeAdapter.Set(val);
            gaugeAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that the adapters call to Inc increments the gauge by the default value of 1.
        /// </summary>
        [TestMethod]
        public void TestInc()
        {
            gaugeAdapter.Inc();
            gaugeAdapter.Value.Should().Be(1);
        }

        /// <summary>
        /// Tests that the adapters call to Inc increments the gauge by a provided value to increment by.
        /// </summary>
        [TestMethod]
        public void TestIncBy()
        {
            var val = RandomDouble();
            gaugeAdapter.Inc(val);
            gaugeAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that the adapters call to IncTo increments the gauge to the provided value.
        /// </summary>
        [TestMethod]
        public void TestIncTo()
        {
            var val = RandomDouble();
            gaugeAdapter.IncTo(val);
            gaugeAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that the adapters call to Dec decrements the gauge by the default value of 1.
        /// </summary>
        [TestMethod]
        public void TestDec()
        {
            gaugeAdapter.Dec();
            gaugeAdapter.Value.Should().Be(-1);
        }

        /// <summary>
        /// Tests that the adapters call to Dec decrements the gauge by a provided value to decrement by.
        /// </summary>
        [TestMethod]
        public void TestDecBy()
        {
            var val = RandomDouble();
            gaugeAdapter.Dec(val);
            gaugeAdapter.Value.Should().Be(-val);
        }

        /// <summary>
        /// Tests that the adapters call to DecTo leaves the gauge where it's at when it's set to a smaller value.
        /// </summary>
        [TestMethod]
        public void TestDecToWhenSmaller()
        {
            var val = RandomDouble();
            gaugeAdapter.IncTo(val);
            gaugeAdapter.DecTo(val + 1);
            gaugeAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that the adapters call to DecTo leaves the gauge at 0 when it's already 0.
        /// </summary>
        [TestMethod]
        public void TestDecToWhenBigger()
        {
            var val = RandomDouble();
            gaugeAdapter.IncTo(val + 1);
            gaugeAdapter.DecTo(val);
            gaugeAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that new timer returns a correct duration.
        /// </summary>
        [TestMethod]
        public void TestNewTimer()
        {
            PrometheusITimer timer;
            using (timer = gaugeAdapter.NewTimer())
            {
                Thread.Sleep(10);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(10));

            // sanity check it goes up if we add time
            using (timer = gaugeAdapter.NewTimer())
            {
                Thread.Sleep(20);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(20));
        }

        /// <summary>
        /// Tests that track in progress returns a correct value by mocking a concurrent operation with the same gauge.
        /// </summary>
        [TestMethod]
        public void TestTrackInProgress()
        {
            gaugeAdapter.Value.Should().Be(0);

            using (gaugeAdapter.TrackInProgress())
            {
                gaugeAdapter.Value.Should().Be(1);

                using (gaugeAdapter.TrackInProgress())
                {
                    gaugeAdapter.Value.Should().Be(2);
                }

                gaugeAdapter.Value.Should().Be(1);
            }

            gaugeAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that with labels sets the label values.
        /// </summary>
        [TestMethod]
        public void TestWithLabels()
        {
            IPrometheusGaugeAdapter labelledGauge = gaugeAdapter.WithLabels("foo", "bar", "baz");
            labelledGauge.Should().NotBeNull();
            labelledGauge.Should().NotBeSameAs(gaugeAdapter);

            Action action = () => labelledGauge.WithLabels("foo");
            action.Should().Throw<InvalidOperationException>();
        }

        /// <summary>
        /// Tests that with labels chained with a call uses the correct gauge.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsSet()
        {
            gaugeAdapter.WithLabels("foo", "bar", "baz").Value.Should().Be(gauge.WithLabels("foo", "bar", "baz").Value);
            gaugeAdapter.WithLabels("foo", "bar", "baz").Set(RNG.NextDouble());
            gaugeAdapter.WithLabels("foo", "bar", "baz").Value.Should().Be(gauge.WithLabels("foo", "bar", "baz").Value);
        }

        /************************************ HELPER METHODS *************************************/

        private static string RandomMetricName() => $"a{Guid.NewGuid().ToString().Replace("-", string.Empty)}";

        private static PrometheusGauge CreatePrometheusGauge(string name) => PromMetricFactory.CreateGauge(name, string.Empty, new PrometheusGaugeConfiguration() { LabelNames = new string[] { "foo", "bar", "baz" } });

        private static double RandomDouble() => RNG.NextDouble();
    }
}
