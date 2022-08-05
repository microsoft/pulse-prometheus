// <copyright file="PrometheusGaugeAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using static Prometheus.GaugeExtensions;
using static Prometheus.TimerExtensions;
using PrometheusGauge = Prometheus.Gauge;
using PrometheusGaugeChild = Prometheus.Gauge.Child;
using PrometheusIGauge = Prometheus.IGauge;
using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Adapters
{
    using System;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Mockable adapter around Prometheus.Gauge.
    /// </summary>
    public class PrometheusGaugeAdapter : IPrometheusGaugeAdapter
    {
        private readonly PrometheusIGauge gauge;

        private readonly PrometheusGauge unlabelledGauge;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusGaugeAdapter"/> class.
        /// </summary>
        /// <param name="gauge"><see cref="PrometheusGauge"/>.</param>
        public PrometheusGaugeAdapter(PrometheusGauge gauge)
        {
            this.gauge = gauge;
            unlabelledGauge = gauge;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusGaugeAdapter"/> class.
        /// This is a labelled instance.
        /// </summary>
        /// <param name="gauge"><see cref="PrometheusGaugeChild"/>.</param>
        public PrometheusGaugeAdapter(PrometheusGaugeChild gauge)
        {
            this.gauge = gauge;
            unlabelledGauge = null;
        }

        /// <inheritdoc/>
        public double Value => gauge.Value;

        private bool IsLabelled
        {
            get
            {
                return unlabelledGauge is null;
            }
        }

        /// <inheritdoc/>
        public IPrometheusGaugeAdapter WithLabels(params string[] labels)
        {
            if (IsLabelled)
            {
                throw new InvalidOperationException();
            }

            return new PrometheusGaugeAdapter(unlabelledGauge.WithLabels(labels));
        }

        /// <inheritdoc/>
        public void Set(double targetValue) => gauge.Set(targetValue);

        /// <inheritdoc/>
        public void Inc(double value = 1.0) => gauge.Inc(value);

        /// <inheritdoc/>
        public void IncTo(double targetValue) => gauge.IncTo(targetValue);

        /// <inheritdoc/>
        public void Dec(double value = 1.0) => gauge.Dec(value);

        /// <inheritdoc/>
        public void DecTo(double targetValue) => gauge.DecTo(targetValue);

        /// <inheritdoc/>
        public PrometheusITimer NewTimer() => gauge.NewTimer();

        /// <inheritdoc/>
        public IDisposable TrackInProgress() => gauge.TrackInProgress();
    }
}