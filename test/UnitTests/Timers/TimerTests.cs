// <copyright file="TimerTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.UnitTests.Timers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Prometheus.Timers;

    /// <summary>
    /// Tests for the timer metric helper.
    /// </summary>
    [TestClass]
    public class TimerTests
    {
        private static readonly Mock<PrometheusITimer> MockPrometheusTimer = new ();

        /// <summary>
        /// Tests our timer calls prometheus's implementation of dispose when finished.
        /// </summary>
        [TestMethod]
        public void TestCallsPrometheusITimerDispose()
        {
            using (new PulsePrometheusTimer(MockPrometheusTimer.Object))
            {
            }

            MockPrometheusTimer.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
