// <copyright file="PulsePrometheusHistogram.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Histograms
{
    using Pulse.Interfaces;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Timers;

    /// <summary>
    /// Implementation for our histogram metric.
    /// </summary>
    public class PulsePrometheusHistogram : IHistogram
    {
        private readonly IPrometheusHistogramAdapter histogram;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusHistogram"/> class.
        /// </summary>
        /// <param name="histogram"><see cref="IPrometheusHistogramAdapter"/>.</param>
        public PulsePrometheusHistogram(IPrometheusHistogramAdapter histogram)
        {
            this.histogram = histogram;
        }

        /// <inheritdoc/>
        public long Count => histogram.Count;

        /// <inheritdoc/>
        public double Sum => histogram.Sum;

        /// <inheritdoc/>
        public IHistogram WithLabels(params string[] labels) => new PulsePrometheusHistogram(histogram.WithLabels(labels));

        /// <inheritdoc/>
        public ITimer NewTimer() => new PulsePrometheusTimer(histogram.NewTimer());

        /// <inheritdoc/>
        public void Observe(double value) => histogram.Observe(value);

        /// <inheritdoc/>
        public void Observe(double value, long count) => histogram.Observe(value, count);
    }
}
