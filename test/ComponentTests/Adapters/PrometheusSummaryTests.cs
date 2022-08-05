// <copyright file="PrometheusSummaryTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryConfiguration = Prometheus.SummaryConfiguration;

namespace Pulse.Prometheus.ComponentTests.Adapters
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for the prometheus summary adapter.
    /// </summary>
    [TestClass]
    public class PrometheusSummaryTests
    {
        private static readonly Random RNG = new ();

        private static readonly IPrometheusMetricFactoryAdapter PromMetricFactory = new PrometheusMetricFactoryAdapter();

        private static PrometheusSummary summary;

        private static PrometheusSummaryAdapter summaryAdapter;

        /// <summary>
        /// Initializes a new prometheus summary and adapter.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            summary = CreatePrometheusSummary(RandomMetricName());
            summaryAdapter = new PrometheusSummaryAdapter(summary);
        }

        /// <summary>
        /// Test summary's individual observe.
        /// </summary>
        /// <remarks>
        /// Summary does not expose a sum or count like histogram.
        /// We also can't verify Observe was actually called as Prometheus.Summary
        /// is not mockable.
        /// We can't test that observe actually did something, so the next best thing is to
        /// check to make sure no exceptions result due to our call.
        /// </remarks>
        [TestMethod]
        public void TestObserve()
        {
            var val = RandomDouble();
            try
            {
                summaryAdapter.Observe(val);
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        /// <summary>
        /// Tests that new timer returns a correct duration.
        /// </summary>
        [TestMethod]
        public void TestNewTimer()
        {
            PrometheusITimer timer;
            using (timer = summaryAdapter.NewTimer())
            {
                Thread.Sleep(10);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(10));

            // sanity check it goes up if we add time
            using (timer = summaryAdapter.NewTimer())
            {
                Thread.Sleep(20);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(20));
        }

        /// <summary>
        /// Tests that with labels sets the label values.
        /// </summary>
        [TestMethod]
        public void TestWithLabels()
        {
            IPrometheusSummaryAdapter labelledSummary = summaryAdapter.WithLabels("foo", "bar", "baz");
            labelledSummary.Should().NotBeNull();
            labelledSummary.Should().NotBeSameAs(summaryAdapter);

            Action action = () => labelledSummary.WithLabels("foo");
            action.Should().Throw<InvalidOperationException>();
        }

        /************************************ HELPER METHODS *************************************/

        private static string RandomMetricName() => $"a{Guid.NewGuid().ToString().Replace("-", string.Empty)}";

        private static PrometheusSummary CreatePrometheusSummary(string name) => PromMetricFactory.CreateSummary(name, string.Empty, new PrometheusSummaryConfiguration() { LabelNames = new string[] { "foo", "bar", "baz" } });

        private static double RandomDouble() => RNG.NextDouble();
    }
}
