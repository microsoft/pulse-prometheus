// <copyright file="MetricServerMiddlewareExtensionsTests.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

namespace Pulse.Prometheus.ComponentTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Pulse.Interfaces;
    using Pulse.Prometheus.Extensions;

    /// <summary>
    /// Test class for service extensions.
    /// </summary>
    [TestClass]
    public class MetricServerMiddlewareExtensionsTests
    {
        /// <summary>
        /// Test add metrics returns itself.
        /// </summary>
        [TestMethod]
        public void TestAddMetricsReturnsSelf()
        {
            IEndpointRouteBuilder builder = new TestEndpointRouteBuilder();
            builder.MapMetrics().Should().BeAssignableTo<IEndpointConventionBuilder>();
        }

        private class TestEndpointRouteBuilder : IEndpointRouteBuilder
        {
            public IServiceProvider ServiceProvider => new DefaultServiceProviderFactory().CreateServiceProvider(new ServiceCollection());

            public ICollection<EndpointDataSource> DataSources => new List<EndpointDataSource>();

            public IApplicationBuilder CreateApplicationBuilder()
            {
                return new ApplicationBuilder(ServiceProvider);
            }
        }
    }
}