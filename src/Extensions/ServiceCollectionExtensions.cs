﻿// <copyright file="ServiceCollectionExtensions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Adapters;
    using Pulse.Prometheus.Factories;
    using Pulse.Prometheus.Interfaces;

    /// <summary>
    /// Set of Extensions for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// <see cref="IServiceCollection"/> extension to register metrics library interfaces.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <returns>self</returns>
        public static IServiceCollection AddMetricFactory(this IServiceCollection services)
            => services
                .AddSingleton<IPrometheusMetricFactoryAdapter, PrometheusMetricFactoryAdapter>()
                .AddSingleton<IMetricFactory, PulseMetricFactory>();
    }
}