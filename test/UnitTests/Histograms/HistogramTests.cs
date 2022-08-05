// <copyright file="HistogramTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.UnitTests.Histograms
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Histograms;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for histogram.
    /// </summary>
    [TestClass]
    public class HistogramTests
    {
        private static readonly Random RNG = new ();

        private static readonly Mock<IPrometheusHistogramAdapter> MockPrometheusHistogram = new ();

        private static readonly Mock<IPrometheusHistogramAdapter> MockLabelledPrometheusHistogram = new ();

        private static readonly Mock<PulsePrometheusHistogram> MockHistogram = new ();

        private static IHistogram histogram;

        /// <summary>
        /// Initializes and sets up a mock prometheus histogram.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockPrometheusHistogram.Reset();
            MockLabelledPrometheusHistogram.Reset();
            MockPrometheusHistogram
                .Setup(x => x.WithLabels(It.IsAny<string[]>()))
                .Returns(MockLabelledPrometheusHistogram.Object);
            histogram = CreateHistogram();
        }

        /// <summary>
        /// Tests that we implement the IHistogram interface.
        /// </summary>
        [TestMethod]
        public void TestImplementsInterface() => CreateHistogram();

        /// <summary>
        /// Tests that we are able to extract the correct count value from histogram.
        /// </summary>
        [TestMethod]
        public void TestCountReturnsPrometheusCount()
        {
            MockPrometheusHistogram.SetupGet(x => x.Count).Returns(10);

            histogram.Count.Should().Be(10);
            MockPrometheusHistogram.Verify(x => x.Count);
        }

        /// <summary>
        /// Tests that we are able to extract the correct sum value from histogram.
        /// </summary>
        [TestMethod]
        public void TestSumReturnsPrometheusSum()
        {
            MockPrometheusHistogram.SetupGet(x => x.Sum).Returns(15);

            histogram.Sum.Should().Be(15);
            MockPrometheusHistogram.Verify(x => x.Sum);
        }

        /// <summary>
        /// Tests that our histogram's call to Observe calls prometheus's Observe.
        /// </summary>
        [TestMethod]
        public void TestCallsObserveWithRandomValue()
        {
            var val = RandomDouble();
            histogram.Observe(val);

            MockPrometheusHistogram.Verify(x => x.Observe(val), Times.Once);
        }

        /// <summary>
        /// Tests Observe with one user supplied value.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsObserveOneUserValue()
        {
            var val = RandomDouble();
            string labelValue = "foo";

            histogram.WithLabels(labelValue).Observe(val);

            MockPrometheusHistogram.Verify(x => x.Observe(val), Times.Never);
            MockPrometheusHistogram.Verify(x => x.WithLabels("foo"), Times.Once);
            MockLabelledPrometheusHistogram.Verify(x => x.Observe(val), Times.Once);
        }

        /// <summary>
        /// Tests Observe with multiple user supplied values.
        /// This is essentially testing that supplying multiple user label values to
        /// WithLabels works.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsObserveMultipleUserValues()
        {
            var val = RandomDouble();

            histogram.WithLabels("foo", "bar").Observe(val);

            MockPrometheusHistogram.Verify(x => x.Observe(val), Times.Never);
            MockPrometheusHistogram.Verify(x => x.WithLabels("foo", "bar"), Times.Once);
            MockLabelledPrometheusHistogram.Verify(x => x.Observe(val), Times.Once);
        }

        /// <summary>
        /// Tests that our histogram's call to Observe with multiple events calls prometheus's
        /// Observe with multiple events.
        /// </summary>
        [TestMethod]
        public void TestCallsMultipleEventsObserveWithRandomValues()
        {
            var val = RandomDouble();
            var val2 = Convert.ToInt64(RandomDouble());
            histogram.Observe(val, val2);

            MockPrometheusHistogram.Verify(x => x.Observe(val, val2), Times.Once);
        }

        /// <summary>
        /// Tests NewTimer implements the ITimer interface, sets up the counter, and calls the timer.
        /// </summary>
        [TestMethod]
        public void TestNewTimerReturnsITimerAndSetsUpLabels()
        {
            ITimer t = histogram.NewTimer();

            MockPrometheusHistogram.Verify(x => x.NewTimer(), Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static IHistogram CreateHistogram() => new PulsePrometheusHistogram(MockPrometheusHistogram.Object);

        private static double RandomDouble() => RNG.NextDouble();
    }
}