// <copyright file="PrometheusHistogramAdapter.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using static Prometheus.TimerExtensions;
using PrometheusHistogram = Prometheus.Histogram;
using PrometheusHistogramChild = Prometheus.Histogram.Child;
using PrometheusIHistogram = Prometheus.IHistogram;
using PrometheusITimer = Prometheus.ITimer;

namespace Pulse.Prometheus.Adapters
{
    using System;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Mockable adapter around Prometheus.Histogram.
    /// </summary>
    public class PrometheusHistogramAdapter : IPrometheusHistogramAdapter
    {
        private readonly PrometheusIHistogram histogram;

        private readonly PrometheusHistogram unlabelledHistogram;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusHistogramAdapter"/> class.
        /// </summary>
        /// <param name="histogram"><see cref="PrometheusHistogram"/>.</param>
        public PrometheusHistogramAdapter(PrometheusHistogram histogram)
        {
            this.histogram = histogram;
            unlabelledHistogram = histogram;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrometheusHistogramAdapter"/> class.
        /// This is a labelled instance.
        /// </summary>
        /// <param name="histogram"><see cref="PrometheusHistogramChild"/>.</param>
        public PrometheusHistogramAdapter(PrometheusHistogramChild histogram)
        {
            this.histogram = histogram;
            unlabelledHistogram = null;
        }

        /// <inheritdoc/>
        public double Sum => histogram.Sum;

        /// <inheritdoc/>
        public long Count => histogram.Count;

        private bool IsLabelled
        {
            get
            {
                return unlabelledHistogram is null;
            }
        }

        /// <summary>
        ///  Creates '<paramref name="count"/>' buckets, where the lowest bucket has an
        ///  upper bound of '<paramref name="start"/>' and each following bucket's upper bound is '<paramref name="factor"/>'
        ///  times the previous bucket's upper bound.
        ///  The function throws if '<paramref name="count"/>' is 0 or negative, if '<paramref name="start"/>' is 0 or negative,
        ///  or if '<paramref name="factor"/>' is less than or equal 1.
        /// </summary>
        /// <param name="start">The upper bound of the lowest bucket. Must be positive.</param>
        /// <param name="factor">The factor to increase the upper bound of subsequent buckets. Must be greater than 1.</param>
        /// <param name="count">The number of buckets to create. Must be positive.</param>
        /// <returns>a <see cref="double"/>[] representing buckets.</returns>
        public static double[] ExponentialBuckets(double start, double factor, int count) => PrometheusHistogram.ExponentialBuckets(start, factor, count);

        /// <summary>
        ///  Creates '<paramref name="count"/>' buckets, where the lowest bucket has an
        ///  upper bound of '<paramref name="start"/>' and each following bucket's upper bound is the upper bound of the
        ///  previous bucket, incremented by '<paramref name="width"/>'
        ///  The function throws if '<paramref name="count"/>' is 0 or negative.
        /// </summary>
        /// <param name="start">The upper bound of the lowest bucket.</param>
        /// <param name="width">The width of each bucket (distance between lower and upper bound).</param>
        /// <param name="count">The number of buckets to create. Must be positive.</param>
        /// <returns>A <see cref="double"/>[] representing buckets.</returns>
        public static double[] LinearBuckets(double start, double width, int count) => PrometheusHistogram.LinearBuckets(start, width, count);

        /// <summary>
        /// Divides each power of 10 into N divisions.
        /// </summary>
        /// <param name="startPower">The starting range includes 10 raised to this power.</param>
        /// <param name="endPower">The ranges end with 10 raised to this power (this no longer starts a new range).</param>
        /// <param name="divisions">How many divisions to divide each range into.</param>
        /// <returns>A <see cref="double"/>[] representing buckets.</returns>
        /// <remarks>
        /// For example, with startPower=-1, endPower=2, divisions=4 we would get:
        /// 10^-1 == 0.1 which defines our starting range, giving buckets: 0.25, 0.5, 0.75, 1.0
        /// 10^0 == 1 which is the next range, giving buckets: 2.5, 5, 7.5, 10
        /// 10^1 == 10 which is the next range, giving buckets: 25, 50, 75, 100
        /// 10^2 == 100 which is the end and the top level of the preceding range.
        /// Giving total buckets: 0.25, 0.5, 0.75, 1.0, 2.5, 5, 7.5, 10, 25, 50, 75, 100
        /// </remarks>
        public static double[] PowersOfTenDividedBuckets(int startPower, int endPower, int divisions) => PrometheusHistogram.PowersOfTenDividedBuckets(startPower, endPower, divisions);

        /// <inheritdoc/>
        /// <exception><see cref="InvalidOperationException"/>.</exception>
        public IPrometheusHistogramAdapter WithLabels(params string[] labels)
        {
            if (IsLabelled)
            {
                throw new InvalidOperationException();
            }

            return new PrometheusHistogramAdapter(unlabelledHistogram.WithLabels(labels));
        }

        /// <inheritdoc/>
        public void Observe(double val) => histogram.Observe(val, 1);

        /// <inheritdoc/>
        public void Observe(double val, long count) => histogram.Observe(val, count);

        /// <inheritdoc/>
        public PrometheusITimer NewTimer() => histogram.NewTimer();
    }
}