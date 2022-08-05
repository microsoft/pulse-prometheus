// <copyright file="IPrometheusCounterAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Interfaces
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Mockable interface to Prometheus.Counter.
    /// </summary>
    public interface IPrometheusCounterAdapter
    {
        /// <summary>
        /// Gets counter value.
        /// </summary>
        /// <returns><see cref="double"/>.</returns>
        public double Value { get; }

        /// <summary>
        /// Set label values on this counter metric.
        /// </summary>
        /// <param name="labels">Label values.</param>
        /// <returns><see cref="IPrometheusCounterAdapter"/>.</returns>
        public IPrometheusCounterAdapter WithLabels(params string[] labels);

        /// <summary>
        /// Increment this counter metric.
        /// </summary>
        /// <param name="value">Value to increment by.</param>
        public void Inc(double value = 1.0);

        /// <summary>
        /// Increment this counter metric to a target value.
        /// </summary>
        /// <param name="targetValue">Value to increment to.</param>
        public void IncTo(double targetValue);

        /// <summary>
        /// Tracks the time it takes to complete a block of code in seconds and increments this counter
        /// to that value.
        /// </summary>
        /// <returns><see cref="PrometheusITimer"/>.</returns>
        public PrometheusITimer NewTimer();

        /// <summary>
        /// Executes the provided operation and increments this counter if an exception occurs. The exception is re-thrown.
        /// If an exception filter is specified, only counts exceptions for which the filter returns true.
        /// </summary>
        /// <param name="wrapped"><see cref="Action"/> to count exceptions in.</param>
        /// <param name="exceptionFilter"><see cref="Func{T, TResult}"/> that returns false for exceptions you want to exclude.</param>
        public void CountExceptions(Action wrapped, Func<Exception, bool> exceptionFilter = null);

        /// <summary>
        /// Executes the provided operation and increments this counter if an exception occurs. The exception is re-thrown.
        /// If an exception filter is specified, only counts exceptions for which the filter returns true.
        /// </summary>
        /// <typeparam name="TResult">Generic type.</typeparam>
        /// <param name="wrapped"><see cref="Func{TResult}"/> to count exceptions in.</param>
        /// <param name="exceptionFilter"><see cref="Func{T, TResult}"/> that returns false for exceptions you want to exclude.</param>
        /// <returns>Return value from wrapped.</returns>
        public TResult CountExceptions<TResult>(Func<TResult> wrapped, Func<Exception, bool> exceptionFilter = null);

        /// <summary>
        /// Executes the provided async operation and increments this counter if an exception occurs. The exception is re-thrown.
        /// If an exception filter is specified, only counts exceptions for which the filter returns true.
        /// </summary>
        /// <param name="wrapped"><see cref="Func{Task}"/> to count exceptions in.</param>
        /// <param name="exceptionFilter"><see cref="Func{T, TResult}"/> that returns false for exceptions you want to exclude.</param>
        /// <returns><see cref="Task"/>.</returns>
        public Task CountExceptionsAsync(Func<Task> wrapped, Func<Exception, bool> exceptionFilter = null);

        /// <summary>
        /// Executes the provided async operation and increments this counter if an exception occurs. The exception is re-thrown.
        /// If an exception filter is specified, only counts exceptions for which the filter returns true.
        /// </summary>
        /// <typeparam name="TResult">Generic type.</typeparam>
        /// <param name="wrapped"><see><cref>Func{Task{TResult}}</cref></see> to count exceptions in.</param>
        /// <param name="exceptionFilter"><see cref="Func{T, TResult}"/>  that returns false for exceptions you want to exclude.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        public Task<TResult> CountExceptionsAsync<TResult>(Func<Task<TResult>> wrapped, Func<Exception, bool> exceptionFilter = null);
    }
}
