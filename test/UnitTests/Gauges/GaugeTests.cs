// <copyright file="GaugeTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.UnitTests.Gauges
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Gauges;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for gauge.
    /// </summary>
    [TestClass]
    public class GaugeTests
    {
        private static readonly Random RNG = new ();

        private static readonly Mock<IPrometheusGaugeAdapter> MockPrometheusGauge = new ();

        private static readonly Mock<IPrometheusGaugeAdapter> MockLabelledPrometheusGauge = new ();

        private static IGauge gauge;

        /// <summary>
        /// Initializes and sets up a mock prometheus gauge.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockPrometheusGauge.Reset();
            MockLabelledPrometheusGauge.Reset();
            MockPrometheusGauge
                .Setup(x => x.WithLabels(It.IsAny<string[]>()))
                .Returns(MockLabelledPrometheusGauge.Object);
            gauge = CreateGauge();
        }

        /// <summary>
        /// Tests that we implement the IGauge interface.
        /// </summary>
        [TestMethod]
        public void TestImplementsInterface() => CreateGauge();

        /// <summary>
        /// Tests that we are able to extract the correct value from gauge.
        /// </summary>
        [TestMethod]
        public void TestValueReturnsPrometheusValue()
        {
            MockPrometheusGauge.SetupGet(x => x.Value).Returns(10);

            gauge.Value.Should().Be(10);
            MockPrometheusGauge.Verify(x => x.Value);
        }

        /// <summary>
        /// Tests that our gauge's call to Set calls prometheus's Set with the supplied value.
        /// </summary>
        [TestMethod]
        public void TestCallsSetWithSuppliedValue()
        {
            var val = RandomDouble();
            gauge.Set(val);

            MockPrometheusGauge.Verify(x => x.Set(val), Times.Once);
        }

        /// <summary>
        /// Tests that our gauge's call to Increment calls prometheus's Inc with the
        /// default 1.0 value.
        /// </summary>
        [TestMethod]
        public void TestCallsIncWithDefaultValue()
        {
            gauge.Increment();

            MockPrometheusGauge.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests that our gauges's call to Increment with a value to increment by calls
        /// prometheus's Inc with the provided value to increment by.
        /// </summary>
        [TestMethod]
        public void TestCallsIncWithSuppliedValue()
        {
            var val = RandomDouble();
            gauge.Increment(val);
            MockPrometheusGauge.Verify(x => x.Inc(val), Times.Once);
        }

        /// <summary>
        /// Tests Increment with one user supplied value.
        /// This is testing that supplying a single user label to WithLabels and calling a
        /// supported method afterward works.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsIncrement()
        {
            string labelValue = "foo";

            gauge.WithLabels(labelValue).Increment();

            MockPrometheusGauge.Verify(x => x.Inc(1.0), Times.Never);
            MockPrometheusGauge.Verify(x => x.WithLabels(labelValue), Times.Once);
            MockLabelledPrometheusGauge.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests Increment with multiple user supplied values.
        /// This is testing that supplying multiple user label values to WithLabels and calling a
        /// supported method afterward works.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsIncrementMultipleUserValues()
        {
            gauge.WithLabels("foo", "bar").Increment();

            MockPrometheusGauge.Verify(x => x.Inc(1.0), Times.Never);
            MockPrometheusGauge.Verify(x => x.WithLabels("foo", "bar"), Times.Once);
            MockLabelledPrometheusGauge.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests that our gauges's call to IncrementTo calls prometheus's IncTo with the given
        /// value.
        /// </summary>
        [TestMethod]
        public void TestCallsIncTo()
        {
            gauge.IncrementTo(500);

            MockPrometheusGauge.Verify(x => x.IncTo(500), Times.Once);
        }

        /// <summary>
        /// Tests that our gauge's call to Decrement calls prometheus's Dec with the
        /// default 1.0 value.
        /// </summary>
        [TestMethod]
        public void TestCallsDecWithDefaultValue()
        {
            gauge.Decrement();

            MockPrometheusGauge.Verify(x => x.Dec(1.0), Times.Once);
        }

        /// <summary>
        /// Tests that our gauges's call to Decrement with a value to decrement by calls
        /// prometheus's Dec with the provided value to decrement by.
        /// </summary>
        [TestMethod]
        public void TestCallsDecWithSuppliedValue()
        {
            var val = RandomDouble();
            gauge.Decrement(val);
            MockPrometheusGauge.Verify(x => x.Dec(val), Times.Once);
        }

        /// <summary>
        /// Tests that our gauges's call to DecrementTo calls prometheus's DecTo with the given
        /// value.
        /// </summary>
        [TestMethod]
        public void TestCallsDecTo()
        {
            gauge.DecrementTo(500);

            MockPrometheusGauge.Verify(x => x.DecTo(500), Times.Once);
        }

        /// <summary>
        /// Tests NewTimer implements the ITimer interface, sets up the gauge, and calls the timer.
        /// </summary>
        [TestMethod]
        public void TestNewTimerReturnsITimerAndSetsUpLabels()
        {
            ITimer t = gauge.NewTimer();

            MockPrometheusGauge.Verify(x => x.NewTimer(), Times.Once);
        }

        /// <summary>
        /// Tests TrackInProgress implements the IDisposable interface and sets up the gauge with labels.
        /// </summary>
        [TestMethod]
        public void TestTrackInProgressReturnsIDisposableAndSetsUpLabels()
        {
            IDisposable d = gauge.TrackInProgress();

            MockPrometheusGauge.Verify(x => x.TrackInProgress(), Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static IGauge CreateGauge() => new PulsePrometheusGauge(MockPrometheusGauge.Object);

        private static double RandomDouble() => RNG.NextDouble();
    }
}
