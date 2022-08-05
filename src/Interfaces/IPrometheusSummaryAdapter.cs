// <copyright file="IPrometheusSummaryAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Interfaces
{
    /// <summary>
    /// Mockable interface to Prometheus.Summary.
    /// </summary>
    public interface IPrometheusSummaryAdapter
    {
        /// <summary>
        /// Set label values on this summary.
        /// </summary>
        /// <param name="labels">Label values.</param>
        /// <returns><see cref="IPrometheusSummaryAdapter"/>.</returns>
        public IPrometheusSummaryAdapter WithLabels(params string[] labels);

        /// <summary>
        /// Observes a single event with the given value.
        /// </summary>
        /// <param name="value">Measured value.</param>
        public void Observe(double value);

        /// <summary>
        /// Tracks the time it takes to complete a block of code in seconds.
        /// </summary>
        /// <returns><see cref="PrometheusITimer"/>.</returns>
        public PrometheusITimer NewTimer();
    }
}
