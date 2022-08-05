// <copyright file="PrometheusSummaryAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using static Prometheus.TimerExtensions;
using PrometheusISummary = Prometheus.ISummary;
using PrometheusITimer = Prometheus.ITimer;
using PrometheusSummary = Prometheus.Summary;
using PrometheusSummaryChild = Prometheus.Summary.Child;

namespace Pulse.Prometheus.Adapters
{
    using System;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Mockable adapter around Prometheus.Summary.
    /// </summary>
    public class PrometheusSummaryAdapter : IPrometheusSummaryAdapter
    {
        private readonly PrometheusISummary summary;

        private readonly PrometheusSummary unlabelledSummary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusSummaryAdapter"/> class.
        /// </summary>
        /// <param name="summary"><see cref="PrometheusSummary"/>.</param>
        public PrometheusSummaryAdapter(PrometheusSummary summary)
        {
            this.summary = summary;
            unlabelledSummary = summary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusSummaryAdapter"/> class.
        /// This is a labelled instance.
        /// </summary>
        /// <param name="summary"><see cref="PrometheusSummaryChild"/>.</param>
        public PrometheusSummaryAdapter(PrometheusSummaryChild summary)
        {
            this.summary = summary;
            unlabelledSummary = null;
        }

        private bool IsLabelled
        {
            get
            {
                return unlabelledSummary is null;
            }
        }

        /// <inheritdoc/>
        public IPrometheusSummaryAdapter WithLabels(params string[] labels)
        {
            if (IsLabelled)
            {
                throw new InvalidOperationException();
            }

            return new PrometheusSummaryAdapter(unlabelledSummary.WithLabels(labels));
        }

        /// <inheritdoc/>
        public void Observe(double val) => summary.Observe(val);

        /// <inheritdoc/>
        public PrometheusITimer NewTimer() => summary.NewTimer();
    }
}
