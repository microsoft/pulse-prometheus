// <copyright file="ServiceCollectionExtensionTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.ComponentTests.Extensions
{
    using System;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Extensions;
    using Pulse.Prometheus.Factories;

    /// <summary>
    /// Test class for service extensions.
    /// </summary>
    [TestClass]
    public class ServiceCollectionExtensionTests
    {
        /// <summary>
        /// Test add metrics returns itself.
        /// </summary>
        [TestMethod]
        public void TestAddMetricsReturnsSelf()
        {
            var services = CreateServiceCollection();
            services.AddMetricFactory().Should().BeSameAs(services);
        }

        /// <summary>
        /// Test add metrics returns a device security metrics factory service.
        /// </summary>
        [TestMethod]
        public void TestAddMetricsRegistersDeviceSecurityMetricsFactory()
        {
            CreateServiceProvider(CreateServiceCollection().AddMetricFactory()).GetService<IMetricFactory>().Should().BeOfType<PulseMetricFactory>();
        }

        private static IServiceCollection CreateServiceCollection()
        {
            IServiceCollection services = new ServiceCollection();
            return services.AddLogging();
        }

        private static IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            var serviceProviderFactory = new DefaultServiceProviderFactory();
            return serviceProviderFactory.CreateServiceProvider(serviceCollection);
        }
    }
}