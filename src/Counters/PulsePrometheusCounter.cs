// <copyright file="PulsePrometheusCounter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Counters
{
    using System;
    using System.Threading.Tasks;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Timers;

    /// <summary>
    /// Implementation for our counter metric.
    /// </summary>
    public class PulsePrometheusCounter : ICounter
    {
        private readonly IPrometheusCounterAdapter counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusCounter"/> class.
        /// </summary>
        /// <param name="counter"><see cref="IPrometheusCounterAdapter"/>.</param>
        public PulsePrometheusCounter(IPrometheusCounterAdapter counter)
        {
            this.counter = counter;
        }

        /// <inheritdoc/>
        public double Value => counter.Value;

        /// <inheritdoc/>
        public ICounter WithLabels(params string[] labels) => new PulsePrometheusCounter(counter.WithLabels(labels));

        /// <inheritdoc/>
        public void Increment(double value = 1.0) => counter.Inc(value);

        /// <inheritdoc/>
        public void IncrementTo(double targetValue) => counter.IncTo(targetValue);

        /// <inheritdoc/>
        public ITimer NewTimer() => new PulsePrometheusTimer(counter.NewTimer());

        /// <inheritdoc/>
        public void CountExceptions(Action wrapped, Func<Exception, bool> exceptionFilter = null) => counter.CountExceptions(wrapped, exceptionFilter);

        /// <inheritdoc/>
        public TResult CountExceptions<TResult>(Func<TResult> wrapped, Func<Exception, bool> exceptionFilter = null) => counter.CountExceptions(wrapped, exceptionFilter);

        /// <inheritdoc/>
        public async Task CountExceptionsAsync(Func<Task> wrapped, Func<Exception, bool> exceptionFilter = null) => await counter.CountExceptionsAsync(wrapped, exceptionFilter);

        /// <inheritdoc/>
        public async Task<TResult> CountExceptionsAsync<TResult>(Func<Task<TResult>> wrapped, Func<Exception, bool> exceptionFilter = null) => await counter.CountExceptionsAsync(wrapped, exceptionFilter);
    }
}
