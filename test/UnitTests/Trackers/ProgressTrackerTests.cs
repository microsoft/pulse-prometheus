// <copyright file="ProgressTrackerTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.UnitTests.Trackers
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Pulse.Prometheus.Trackers;

    /// <summary>
    /// Tests for the tracker metric helper.
    /// </summary>
    [TestClass]
    public class ProgressTrackerTests
    {
        private static readonly Mock<IDisposable> MockProgressTracker = new ();

        /// <summary>
        /// Tests our progress tracker calls dispose when finished.
        /// </summary>
        [TestMethod]
        public void TestCallsIDisposableDispose()
        {
            using (new PulsePrometheusProgressTracker(MockProgressTracker.Object))
            {
            }

            MockProgressTracker.Verify(x => x.Dispose(), Times.Once);
        }
    }
}