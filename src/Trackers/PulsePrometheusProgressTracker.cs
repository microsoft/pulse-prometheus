// <copyright file="PulsePrometheusProgressTracker.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Trackers
{
    using System;

    /// <summary>
    /// Implementation for our progress tracker metric helper.
    /// </summary>
    public class PulsePrometheusProgressTracker : IDisposable
    {
        private readonly IDisposable progressTracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePrometheusProgressTracker"/> class.
        /// </summary>
        /// <param name="progressTracker"><see cref="IDisposable"/>.</param>
        public PulsePrometheusProgressTracker(IDisposable progressTracker)
        {
            this.progressTracker = progressTracker;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            progressTracker.Dispose();
        }
    }
}
