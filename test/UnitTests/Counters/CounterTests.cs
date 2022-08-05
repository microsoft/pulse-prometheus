// <copyright file="CounterTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.UnitTests.Counters
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Counters;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Tests for counter.
    /// </summary>
    [TestClass]
    public class CounterTests
    {
        private static readonly Random RNG = new ();

        private static readonly Mock<IPrometheusCounterAdapter> MockPrometheusCounter = new ();

        private static readonly Mock<IPrometheusCounterAdapter> MockLabelledPrometheusCounter = new ();

        private static ICounter counter;

        /// <summary>
        /// Initializes and sets up a mock prometheus counter.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockPrometheusCounter.Reset();
            MockLabelledPrometheusCounter.Reset();
            MockPrometheusCounter
                .Setup(x => x.WithLabels(It.IsAny<string[]>()))
                .Returns(MockLabelledPrometheusCounter.Object);
            counter = CreateCounter();
        }

        /// <summary>
        /// Tests that we implement the ICounter interface.
        /// </summary>
        [TestMethod]
        public void TestImplementsInterface() => CreateCounter();

        /// <summary>
        /// Tests that we are able to extract the correct value from counter.
        /// </summary>
        [TestMethod]
        public void TestValueReturnsPrometheusValue()
        {
            MockPrometheusCounter.SetupGet(x => x.Value).Returns(10);

            counter.Value.Should().Be(10);
            MockPrometheusCounter.Verify(x => x.Value);
        }

        /// <summary>
        /// Tests that our counter's call to Increment calls prometheus's Inc with the
        /// default 1.0 value.
        /// </summary>
        [TestMethod]
        public void TestCallsIncWithDefaultValue()
        {
            counter.Increment();

            MockPrometheusCounter.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests that our counter's call to Increment with a value to increment by calls
        /// prometheus's Inc with the provided value to increment by.
        /// </summary>
        [TestMethod]
        public void TestCallsIncWithSuppliedValue()
        {
            var val = RandomDouble();
            counter.Increment(val);
            MockPrometheusCounter.Verify(x => x.Inc(val), Times.Once);
        }

        /// <summary>
        /// Tests Increment with one user supplied value.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsIncrementOneUserValue()
        {
            string labelValue = "foo";

            counter.WithLabels(labelValue).Increment();

            MockPrometheusCounter.Verify(x => x.Inc(1.0), Times.Never);
            MockPrometheusCounter.Verify(x => x.WithLabels(labelValue), Times.Once);
            MockLabelledPrometheusCounter.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests Increment with multiple user supplied values.
        /// This is essentially testing that supplying multiple user label values to
        /// WithLabels works.
        /// </summary>
        [TestMethod]
        public void TestWithLabelsIncrementMultipleUserValues()
        {
            counter.WithLabels("foo", "bar").Increment();

            MockPrometheusCounter.Verify(x => x.Inc(1.0), Times.Never);
            MockPrometheusCounter.Verify(x => x.WithLabels("foo", "bar"), Times.Once);
            MockLabelledPrometheusCounter.Verify(x => x.Inc(1.0), Times.Once);
        }

        /// <summary>
        /// Tests that our counter's call to IncrementTo calls prometheus's IncTo with the given
        /// value.
        /// </summary>
        [TestMethod]
        public void TestCallsIncTo()
        {
            counter.IncrementTo(500);

            MockPrometheusCounter.Verify(x => x.IncTo(500), Times.Once);
        }

        /// <summary>
        /// Tests NewTimer implements the ITimer interface, sets up the counter, and calls the timer.
        /// </summary>
        [TestMethod]
        public void TestNewTimerReturnsITimerAndSetsUpLabels()
        {
            ITimer t = counter.NewTimer();

            MockPrometheusCounter.Verify(x => x.NewTimer(), Times.Once);
        }

        /// <summary>
        /// Tests that counters void CountExceptions implements prometheus's CountExceptions
        /// without an exception filter and sets up the counter with labels.
        /// </summary>
        [TestMethod]
        public void TestPassesArgumentsToCountExceptionsWithoutExceptionFilter()
        {
            Action action = () => { };

            counter.CountExceptions(action);

            MockPrometheusCounter.Verify(x => x.CountExceptions(action, null), Times.Once);
        }

        /// <summary>
        /// Tests that counters void CountExceptions implements prometheus's CountExceptions
        /// with an exception filter and sets up the counter with labels.
        /// </summary>
        [TestMethod]
        public void TestPassesArgumentsToCountExceptionsWithExceptionFilter()
        {
            Action action = () => { };

            Func<Exception, bool> filter = exc => true;

            counter.CountExceptions(action, filter);
            MockPrometheusCounter.Verify(x => x.CountExceptions(action, filter), Times.Once);
        }

        /// <summary>
        /// Tests that counters non-void CountExceptions implements prometheus's CountExceptions
        /// without an exception filter and sets up the counter with labels.
        /// </summary>
        [TestMethod]
        public void TestPassesArgumentsToNonVoidCountExceptionsWithoutExceptionFilter()
        {
            var retval = RandomDouble();
            MockPrometheusCounter
                .Setup(x => x.CountExceptions(It.IsAny<Func<double>>(), It.IsAny<Func<Exception, bool>>()))
                .Returns(retval);
            Func<double> foo = () => 0.0;

            counter.CountExceptions(foo).Should().Be(retval);

            MockPrometheusCounter.Verify(x => x.CountExceptions(foo, null), Times.Once);
        }

        /// <summary>
        /// Tests that counters non-void CountExceptions implements prometheus's CountExceptions
        /// with an exception filter and sets up the counter with labels.
        /// </summary>
        [TestMethod]
        public void TestPassesArgumentsToNonVoidCountExceptionsWithExceptionFilter()
        {
            var retval = RandomDouble();
            MockPrometheusCounter
                .Setup(x => x.CountExceptions(It.IsAny<Func<double>>(), It.IsAny<Func<Exception, bool>>()))
                .Returns(retval);
            Func<double> foo = () => 0.0;
            Func<Exception, bool> filter = exc => true;

            counter.CountExceptions(foo, filter).Should().Be(retval);

            MockPrometheusCounter.Verify(x => x.CountExceptions(foo, filter), Times.Once);
        }

        /// <summary>
        /// Tests that counters void async CountExceptions implements prometheus's CountExceptions
        /// without an exception filter and sets up the counter with labels.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestPassesArgumentsToCountExceptionsAsyncWithoutExceptionFilterAsync()
        {
            Func<Task> action = async () => await Task.CompletedTask;

            await counter.CountExceptionsAsync(action);

            MockPrometheusCounter.Verify(x => x.CountExceptionsAsync(action, null), Times.Once);
        }

        /// <summary>
        /// Tests that counters void async CountExceptions implements prometheus's CountExceptions
        /// with an exception filter and sets up the counter with labels.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestPassesArgumentsToCountExceptionsAsyncWithExceptionFilter()
        {
            Func<Task> action = async () => await Task.CompletedTask;

            Func<Exception, bool> filter = exc => true;

            await counter.CountExceptionsAsync(action, filter);

            MockPrometheusCounter.Verify(x => x.CountExceptionsAsync(action, filter), Times.Once);
        }

        /// <summary>
        /// Tests that counters non-void async CountExceptions implements prometheus's
        /// CountExceptions without an exception filter and sets up the counter with labels.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestPassesArgumentsToNonVoidCountExceptionsAsyncWithoutExceptionFilterAsync()
        {
            var retval = RandomDouble();
            MockPrometheusCounter
                .Setup(x => x.CountExceptionsAsync(It.IsAny<Func<Task<double>>>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(retval);
            Func<Task<double>> foo = async () => await Task.FromResult(0.0);

            var result = await counter.CountExceptionsAsync(foo);

            result.Should().Be(retval);
            MockPrometheusCounter.Verify(x => x.CountExceptionsAsync(foo, null), Times.Once);
        }

        /// <summary>
        /// Tests that counters non-void async CountExceptions implements prometheus's
        /// CountExceptions with an exception filter and sets up the counter with labels.
        /// </summary>
        /// /// <returns><see cref="Task"/>.</returns>
        [TestMethod]
        public async Task TestPassesArgumentsToNonVoidCountExceptionsAsyncWithExceptionFilter()
        {
            var retval = RandomDouble();
            MockPrometheusCounter
                .Setup(x => x.CountExceptionsAsync(It.IsAny<Func<Task<double>>>(), It.IsAny<Func<Exception, bool>>()))
                .ReturnsAsync(retval);
            Func<Task<double>> foo = async () => await Task.FromResult(0.0);
            Func<Exception, bool> filter = exc => true;

            var result = await counter.CountExceptionsAsync(foo, filter);

            result.Should().Be(retval);
            MockPrometheusCounter.Verify(x => x.CountExceptionsAsync(foo, filter), Times.Once);
        }

        /************************************ HELPER METHODS *************************************/

        private static ICounter CreateCounter() => new PulsePrometheusCounter(MockPrometheusCounter.Object);

        private static double RandomDouble() => RNG.NextDouble();
    }
}
