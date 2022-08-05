// <copyright file="PrometheusCounterAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using static Prometheus.CounterExtensions;
using static Prometheus.TimerExtensions;
using PrometheusCounter = Prometheus.Counter;
using PrometheusCounterChild = Prometheus.Counter.Child;
using PrometheusICounter = Prometheus.ICounter;
using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Adapters
{
    using System;
    using System.Threading.Tasks;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Mockable adapter around Prometheus.Counter.
    /// </summary>
    public class PrometheusCounterAdapter : IPrometheusCounterAdapter
    {
        private readonly PrometheusICounter counter;

        private readonly PrometheusCounter unlabelledCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusCounterAdapter"/> class.
        /// </summary>
        /// <param name="counter"><see cref="PrometheusCounter"/>.</param>
        public PrometheusCounterAdapter(PrometheusCounter counter)
        {
            this.counter = counter;
            unlabelledCounter = counter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusCounterAdapter"/> class.
        /// This is a labelled instance.
        /// </summary>
        /// <param name="counter"><see cref="PrometheusCounterChild"/>.</param>
        public PrometheusCounterAdapter(PrometheusCounterChild counter)
        {
            this.counter = counter;
            unlabelledCounter = null;
        }

        /// <inheritdoc/>
        public double Value => counter.Value;

        private bool IsLabelled
        {
            get
            {
                return unlabelledCounter is null;
            }
        }

        /// <inheritdoc/>
        /// <exception><see cref="InvalidOperationException"/>.</exception>
        public IPrometheusCounterAdapter WithLabels(params string[] labels)
        {
            if (IsLabelled)
            {
                throw new InvalidOperationException();
            }

            return new PrometheusCounterAdapter(unlabelledCounter.WithLabels(labels));
        }

        /// <inheritdoc/>
        public void Inc(double value = 1.0) => counter.Inc(value);

        /// <inheritdoc/>
        public void IncTo(double targetValue) => counter.IncTo(targetValue);

        /// <inheritdoc/>
        public PrometheusITimer NewTimer() => counter.NewTimer();

        /// <inheritdoc/>
        public void CountExceptions(Action wrapped, Func<Exception, bool> exceptionFilter = null)
        {
            counter.CountExceptions(wrapped, exceptionFilter);
        }

        /// <inheritdoc/>
        public TResult CountExceptions<TResult>(Func<TResult> wrapped, Func<Exception, bool> exceptionFilter = null)
        {
            return counter.CountExceptions(wrapped, exceptionFilter);
        }

        /// <inheritdoc/>
        public async Task CountExceptionsAsync(Func<Task> wrapped, Func<Exception, bool> exceptionFilter = null)
        {
            await counter.CountExceptionsAsync(wrapped, exceptionFilter);
        }

        /// <inheritdoc/>
        public async Task<TResult> CountExceptionsAsync<TResult>(Func<Task<TResult>> wrapped, Func<Exception, bool> exceptionFilter = null)
        {
            return await counter.CountExceptionsAsync(wrapped, exceptionFilter);
        }
    }
}
