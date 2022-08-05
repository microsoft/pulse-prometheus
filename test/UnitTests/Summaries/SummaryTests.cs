// <copyright file="SummaryTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.UnitTests.Summaries
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Summaries;

    /// <summary>
    /// Tests for summary.
    /// </summary>
    [TestClass]
    public class SummaryTests
    {
        private static readonly Random RNG = new ();

        private static readonly Mock<IPrometheusSummaryAdapter> MockPrometheusSummary = new ();

        private static readonly Mock<IPrometheusSummaryAdapter> MockLabelledPrometheusSummary = new ();

        private static ISummary summary;

        /// <summary>
        /// Initializes and sets up a mock prometheus summary.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockPrometheusSummary.Reset();
            MockLabelledPrometheusSummary.Reset();
            MockPrometheusSummary
                .Setup(x => x.WithLabels(It.IsAny<string[]>()))
                .Returns(MockLabelledPrometheusSummary.Object);
            summary = CreateSummary();
        }

        /// <summary>
        /// Tests that we implement the ISummary interface.
        /// </summary>
        [TestMethod]
        public void TestImplementsInterface() => CreateSummary();

        /// <summary>
        /// Tests that our summary's call to Observe calls prometheus's Observe.
        /// </summary>
        [TestMethod]
        public void TestCallsObserveWithRandomValue()
        {
            var val = RandomDouble();
            summary.Observe(val);

            MockPrometheusSummary.Verify(x => x.Observe(val), Times.Once);
        }

        /// <summary>
        /// Tests Observe with one user supplied value.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsObserveOneUserValue()
        {
            var val = RandomDouble();
            string labelValue = "foo";

            summary.WithLabels(labelValue).Observe(val);

            MockPrometheusSummary.Verify(x => x.Observe(val), Times.Never);
            MockPrometheusSummary.Verify(x => x.WithLabels(labelValue), Times.Once);
            MockLabelledPrometheusSummary.Verify(x => x.Observe(val), Times.Once);
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

            summary.WithLabels("foo", "bar").Observe(val);

            MockPrometheusSummary.Verify(x => x.Observe(val), Times.Never);
            MockPrometheusSummary.Verify(x => x.WithLabels("foo", "bar"), Times.Once);
            MockLabelledPrometheusSummary.Verify(x => x.Observe(val), Times.Once);
        }

        /// <summary>
        /// Tests NewTimer implements the ITimer interface, sets up the counter, and calls the timer.
        /// </summary>
        [TestMethod]
        public void TestNewTimerReturnsITimerAndSetsUpLabels()
        {
            ITimer t = summary.NewTimer();

            MockPrometheusSummary.Verify(x => x.NewTimer(), Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static ISummary CreateSummary() => new PulsePrometheusSummary(MockPrometheusSummary.Object);

        private static double RandomDouble() => RNG.NextDouble();
    }
}
