// <copyright file="PulsePrometheusSummary.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Summaries
{
    using Pulse.Interfaces;
    using Pulse.Prometheus.Interfaces;
    using Pulse.Prometheus.Timers;

    /// <summary>
    /// Implementation for our summary metric.
    /// </summary>
    public class PulsePrometheusSummary : ISummary
    {
        private readonly IPrometheusSummaryAdapter summary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusSummary"/> class.
        /// </summary>
        /// <param name="summary"><see cref="IPrometheusSummaryAdapter"/>.</param>
        public PulsePrometheusSummary(IPrometheusSummaryAdapter summary)
        {
            this.summary = summary;
        }

        /// <inheritdoc/>
        public ISummary WithLabels(params string[] labels) => new PulsePrometheusSummary(summary.WithLabels(labels));

        /// <inheritdoc/>
        public void Observe(double value) => summary.Observe(value);

        /// <inheritdoc/>
        public ITimer NewTimer() => new PulsePrometheusTimer(summary.NewTimer());
    }
}
