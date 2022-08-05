// <copyright file="PulsePrometheusGauge.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Gauges
{
    using System;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Timers;
    using Pulse.Prometheus.Trackers;

    /// <summary>
    /// Implementation for our gauge metric.
    /// </summary>
    public class PulsePrometheusGauge : IGauge
    {
        private readonly IPrometheusGaugeAdapter gauge;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusGauge"/> class.
        /// </summary>
        /// <param name="gauge"><see cref="IPrometheusGaugeAdapter"/>.</param>
        public PulsePrometheusGauge(IPrometheusGaugeAdapter gauge)
        {
            this.gauge = gauge;
        }

        /// <inheritdoc/>
        public double Value => gauge.Value;

        /// <inheritdoc/>
        public IGauge WithLabels(params string[] labels) => new PulsePrometheusGauge(gauge.WithLabels(labels));

        /// <inheritdoc/>
        public void Set(double targetValue) => gauge.Set(targetValue);

        /// <inheritdoc/>
        public void Increment(double value = 1.0) => gauge.Inc(value);

        /// <inheritdoc/>
        public void IncrementTo(double targetValue) => gauge.IncTo(targetValue);

        /// <inheritdoc/>
        public void Decrement(double value = 1.0)
        {
            gauge.Dec(value);
        }

        /// <inheritdoc/>
        public void DecrementTo(double targetValue)
        {
            gauge.DecTo(targetValue);
        }

        /// <inheritdoc/>
        public ITimer NewTimer() => new PulsePrometheusTimer(gauge.NewTimer());

        /// <inheritdoc/>
        public IDisposable TrackInProgress() => new PulsePrometheusProgressTracker(gauge.TrackInProgress());
    }
}
