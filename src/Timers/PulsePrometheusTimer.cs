// <copyright file="PulsePrometheusTimer.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Timers
{
    using Pulse.Interfaces;

    /// <summary>
    /// Implementation for our timer metric helper.
    /// </summary>
    public class PulsePrometheusTimer : ITimer
    {
        private readonly PrometheusITimer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusTimer"/> class.
        /// </summary>
        /// <param name="timer"><see cref="PrometheusITimer"/>.</param>
        public PulsePrometheusTimer(PrometheusITimer timer)
        {
            this.timer = timer;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
