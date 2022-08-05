// <copyright file="PrometheusCounterTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusCounter = Prometheus.Counter;
using PrometheusCounterConfiguration = Prometheus.CounterConfiguration;
using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.ComponentTests.Adapters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for the prometheus counter adapter.
    /// </summary>
    [TestClass]
    public class PrometheusCounterTests
    {
        private static readonly Random RNG = new ();

        private static readonly IPrometheusMetricFactoryAdapter PromMetricFactory = new PrometheusMetricFactoryAdapter();

        private static PrometheusCounter counter;

        private static PrometheusCounterAdapter counterAdapter;

        /// <summary>
        /// Initializes a new prometheus counter and adapter.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            counter = CreatePrometheusCounter(RandomMetricName());
            counterAdapter = new PrometheusCounterAdapter(counter);
        }

        /// <summary>
        /// Tests that we are able to extract the correct value from counter.
        /// </summary>
        [TestMethod]
        public void TestValue()
        {
            counterAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that the adapters call to Inc increments the counter by the default value of 1.
        /// </summary>
        [TestMethod]
        public void TestInc()
        {
            counterAdapter.Inc();
            counterAdapter.Value.Should().Be(1);
        }

        /// <summary>
        /// Tests that the adapters call to Inc increments the counter by a provided value to increment by.
        /// </summary>
        [TestMethod]
        public void TestIncBy()
        {
            var val = RandomDouble();
            counterAdapter.Inc(val);
            counterAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that the adapters call to IncTo increments the counter to the provided value.
        /// </summary>
        [TestMethod]
        public void TestIncTo()
        {
            var val = RandomDouble();
            counterAdapter.IncTo(val);
            counterAdapter.Value.Should().Be(val);
        }

        /// <summary>
        /// Tests that new timer returns a postive duration.
        /// </summary>
        [TestMethod]
        public void TestNewTimer()
        {
            PrometheusITimer timer;
            using (timer = counterAdapter.NewTimer())
            {
                Thread.Sleep(10);
            }

            timer.ObserveDuration().Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(10));

            // sanity check it goes up if we add time
            using (timer = counterAdapter.NewTimer())
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
            IPrometheusCounterAdapter labelledCounter = counterAdapter.WithLabels("foo", "bar", "baz");
            labelledCounter.Should().NotBeNull();
            labelledCounter.Should().NotBeSameAs(counterAdapter);

            Action action = () => labelledCounter.WithLabels("foo");
            action.Should().Throw<InvalidOperationException>();
        }

        /// <summary>
        /// Tests that with labels chained with a call uses the correct counter.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsInc()
        {
            counterAdapter.WithLabels("foo", "bar", "baz").Value.Should().Be(counter.WithLabels("foo", "bar", "baz").Value);
            counterAdapter.WithLabels("foo", "bar", "baz").Inc();
            counterAdapter.WithLabels("foo", "bar", "baz").Value.Should().Be(counter.WithLabels("foo", "bar", "baz").Value);
        }

        /// <summary>
        /// Tests that counter adapaters void CountExceptions value is 0 when no exceptions
        /// are thrown.
        /// </summary>
        [TestMethod]
        public void TestCountExceptionsNoExceptions()
        {
            Action wrapped = () => { };
            counterAdapter.CountExceptions(wrapped);
            counterAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that counter adapters void CountExceptions value is 1 when an exception
        /// is thrown and the exception is passed through.
        /// </summary>
        [TestMethod]
        public void TestCountExceptionsWithException()
        {
            Action wrapped = () => throw new Exception();
            Action action = () => counterAdapter.CountExceptions(wrapped);
            action.Should().Throw<Exception>();
            counterAdapter.Value.Should().Be(1);
        }

        /// <summary>
        /// Tests that counter adapaters void CountExceptions value is 0 when no exceptions
        /// are thrown.
        /// </summary>
        [TestMethod]
        public void TestTResultCountExceptionsNoExceptions()
        {
            Func<int> wrapped = () => { return 1; };
            counterAdapter.CountExceptions(wrapped).Should().Be(1);
            counterAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that counter adapters void CountExceptions value is 1 when an exception
        /// is thrown and the exception is passed through.
        /// </summary>
        [TestMethod]
        public void TestTResultCountExceptionsWithException()
        {
            Func<int> wrapped = () => throw new Exception();
            Action action = () => counterAdapter.CountExceptions(wrapped);
            action.Should().Throw<Exception>();
            counterAdapter.Value.Should().Be(1);
        }

        /// <summary>
        /// Tests that counter adapaters void CountExceptionsAsync value is 0 when no exceptions
        /// are thrown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestCountExceptionsAsyncNoExceptions()
        {
            Func<Task> wrapped = async () => await Task.CompletedTask;
            await counterAdapter.CountExceptionsAsync(wrapped);
            counterAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that counter adapters void CountExceptions value is 1 when an exception
        /// is thrown and the exception is passed through.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestCountExceptionsAsyncWithException()
        {
            Func<Task> wrapped = () => throw new Exception();
            Func<Task> action = async () => await counterAdapter.CountExceptionsAsync(wrapped);
            await action.Should().ThrowAsync<Exception>();
            counterAdapter.Value.Should().Be(1);
        }

        /// <summary>
        /// Tests that counter adapaters void CountExceptions value is 0 when no exceptions
        /// are thrown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestTResultCountExceptionsAsyncNoExceptions()
        {
            Func<Task<int>> wrapped = async () => await Task.FromResult(1);
            var val = await counterAdapter.CountExceptionsAsync(wrapped);
            val.Should().Be(1);
            counterAdapter.Value.Should().Be(0);
        }

        /// <summary>
        /// Tests that counter adapters void CountExceptions value is 1 when an exception
        /// is thrown and the exception is passed through.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task TestTResultCountExceptionsAsyncWithException()
        {
            Func<Task<int>> wrapped = () => throw new Exception();
            Func<Task> action = async () => await counterAdapter.CountExceptionsAsync(wrapped);
            await action.Should().ThrowAsync<Exception>();
            counterAdapter.Value.Should().Be(1);
        }

        /************************************ HELPER METHODS *************************************/

        private static string RandomMetricName() => $"a{Guid.NewGuid().ToString().Replace("-", string.Empty)}";

        private static PrometheusCounter CreatePrometheusCounter(string name) => PromMetricFactory.CreateCounter(name, string.Empty, new PrometheusCounterConfiguration() { LabelNames = new string[] { "foo", "bar", "baz" } });

        private static double RandomDouble() => RNG.NextDouble();
    }
}
