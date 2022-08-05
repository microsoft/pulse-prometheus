// <copyright file="IPrometheusGaugeAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Interfaces
{
    using System;

    /// <summary>
    /// Mockable interface to Prometheus.Gauge.
    /// </summary>
    public interface IPrometheusGaugeAdapter
    {
        /// <summary>
        /// Gets gauge value.
        /// </summary>
        /// <returns><see cref="double"/>.</returns>
        public double Value { get; }

        /// <summary>
        /// Set this gauge to the supplied value.
        /// </summary>
        /// <param name="targetValue"><see cref="double"/>.</param>
        public void Set(double targetValue);

        /// <summary>
        /// Set label values on this gauge.
        /// </summary>
        /// <param name="labels">Label values.</param>
        /// <returns><see cref="IPrometheusGaugeAdapter"/>.</returns>
        public IPrometheusGaugeAdapter WithLabels(params string[] labels);

        /// <summary>
        /// Increment this gauge.
        /// </summary>
        /// <param name="value">Value to increment by.</param>
        public void Inc(double value = 1.0);

        /// <summary>
        /// Increment this gauge to the target value.
        /// </summary>
        /// <param name="targetValue">Value to increment to.</param>
        public void IncTo(double targetValue);

        /// <summary>
        /// Decrement this gauge by the supplied value.
        /// </summary>
        /// <param name="value">Value to decrement by.</param>
        public void Dec(double value = 1.0);

        /// <summary>
        /// Decrement this gauge metric to the target value.
        /// </summary>
        /// <param name="targetValue">Value to decrement to.</param>
        public void DecTo(double targetValue);

        /// <summary>
        /// Tracks the time it takes to complete a block of code in seconds and sets this gauge
        /// to that value.
        /// </summary>
        /// <returns><see cref="PrometheusITimer"/>.</returns>
        public PrometheusITimer NewTimer();

        /// <summary>
        /// Tracks the number of in progress operations taking place and sets this gauge
        /// to that value.
        /// </summary>
        /// <returns><see cref="IDisposable"/>.</returns>
        /// <remarks>
        /// It is safe to track the sum of multiple concurrent in-progress operations with the same gauge.
        /// </remarks>
        public IDisposable TrackInProgress();
    }
}