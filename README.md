# pulse-prometheus
[![Build Status](https://github.com/microsoft/pulse-prometheus/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/microsoft/pulse-prometheus/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/microsoft/pulse-prometheus/branch/main/graph/badge.svg?token=q1quhRyQgj)](https://codecov.io/gh/microsoft/pulse-prometheus)
[![Nuget](https://img.shields.io/nuget/v/pulse.prometheus.svg)](https://www.nuget.org/packages/pulse.prometheus/)
[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/microsoft/pulse-prometheus/blob/main/LICENSE.txt)

pulse-prometheus is a .NET library that implements the [pulse](https://github.com/microsoft/pulse) interface and abstracts the C# [prometheus-net](https://github.com/prometheus-net/prometheus-net) library.

Abstracted metrics libraries are helpful in the event the underlying monitoring system changes. Whether the underlying monitoring library experiences breaking changes or [you decide to do a complete swap of the underlying monitoring library](#switching-to-a-different-metrics-library), rest assured that you will only have to update the abstracted library and not your service code.

# Table of Contents
* [Requirements](#requirements)
* [Download](#download)
* [Best Practices and Usage](#best-practices-and-usage)
* [Quick Start](#quick-start)
* [Middleware Extensions](#middleware-extensions)
* [Depedency Injection](#dependency-injection)
* [Metric Factory](#metric-factory)
* [Counter](#counter)
* [Gauge](#gauge)
* [Histogram](#histogram)
* [Summary](#summary)
* [Tracking Operation Duration](#tracking-operation-duration)
* [Counting In-Progress Operations](#counting-in-progress-operations)
* [Counting Exceptions](#counting-exceptions)
* [Mutable Labels](#mutable-labels)
* [Immutable Labels](#immutable-labels)
* [Switching to a Different Metrics Library?](#switching-to-a-different-metrics-library)
* [Contributing](CONTRIBUTING.md)
* [Security](SECURITY.md)
* [Support](SUPPORT.md)
* [License](LICENSE.txt)

## Requirements

* [.NET 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0), [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0), or [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Download

[pulse-prometheus](https://www.nuget.org/packages/pulse.prometheus/) is distributed via the NuGet gallery.

## Best Practices and Usage

This library allows you to instrument your code with custom metrics.
You are expected to be familiar with:
* [Prometheus user guide](https://prometheus.io/docs/introduction/overview/)
* [Prometheus metric types](http://prometheus.io/docs/concepts/metric_types/)
* [Prometheus metric best practices](http://prometheus.io/docs/practices/instrumentation/#counter-vs.-gauge-vs.-summary)

## Quick Start

1. Configure the endpoint. See [Middleware Extensions](#middleware-extensions).
1. Register the [IMetricFactory](#metric-factory). See [Dependency Injection](#dependency-injection). Optionally, [create an IMetricFactory](#metric-factory) instead of injecting it.
1. Use your [IMetricFactory](#metric-factory) to create [counters](#counter), [gauges](#gauge), [histograms](#histogram), and [summaries](#summary).
1. Use your metrics to do other cool things like [track operation duration](#tracking-operation-duration), [count in-progress operations](#counting-in-progress-operations), and [count exceptions](#counting-exceptions). Also check out how to use [mutable labels](#mutable-labels) and [immutable labels](#immutable-labels)

## Middleware Extensions

Use metric middleware extensions to output metrics to an endpoint.

The default is `/metrics`.

```csharp
public class Startup
{
    ...

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseEndpoints(endpoints =>
        {
            ...
            endpoints.MapMetrics();
        });
    }
    
    ...
}
```

## Dependency Injection

Use [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-6.0) extensions to make it easy for consumers to register implementations for the included [IMetricFactory](#metric-factory).

```csharp
public class Startup
{
    ...
    
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddMetricFactory();
    }
    
    ...
}
```

In subsequent code, request an implementation for the [IMetricFactory](#metric-factory) by including it in the constructor of the classes which require it.

```csharp
public class Example
{
    private readonly pulseMetricFactory;

    public Example(IMetricFactory metricFactory)
    {
        pulseMetricFactory = metricFactory;
    }

    public void TrackSomething()
    {
        var counter = pulseMetricFactory.CreateCounter("counter", "this is a counter metric");
        using (counter.NewTimer())
        {
            Thread.Sleep(1000);
        }
    }
}
```

## Metric Factory

Create a metric factory as an entry point into the library, or use [dependency injection](#dependency-injection).

```csharp
private static readonly IMetricFactory MyAppMetricFactory = new PulseMetricFactory(new PrometheusMetricFactory());
```

## Counter

Counters only increase in value and reset to zero when the process restarts.

```csharp
private static readonly ICounter LogCounter = 
    MyAppMetricFactory.CreateCounter("myapp_number_of_logs_emitted", "Number of logs emitted.");

...

Log();
LogCounter.Increment();
```

## Gauge

Gauges can have any numeric value and change arbitrarily.

```csharp
private static readonly IGauge JobQueueGauge = 
    MyAppMetricFactory.CreateGauge("myapp_jobs_queued", "Number of jobs queued.");

...

jobs.Enqueue(job);
JobQueueGauge.Increment();

...

jobs.Dequeue(job);
JobQueueGauge.Decrement();
```

## Histogram

Histograms track the size and number of events in buckets. This allows for aggregatable calculation over a set of buckets.

```csharp
double[] buckets = new double[] { 100.0, 200.0, 300.0, 400.0, 500.0 } 

private static readonly IHistogram OrderValueHistogram = 
    MyAppMetricFactory.CreateHistogram(
        "myapp_order_value_usd", 
        "Histogram of received order values (in USD).", 
        new HistogramConfiguration()
        {
            Buckets = buckets
        });

...

OrderValueHistogram.Observe(order.TotalValueUsd);
```

## Summary

Summaries track events over time, with a default of 10 minutes.

```csharp
private static readonly ISummary UploadSizeSummary = 
    MyAppMetricFactory.CreateSummary(
        "myapp_upload_size_bytes", 
        "Summary of upload sizes (in bytes) over last 10 minutes.");

...

UploadSizeSummary.Observe(file.Length);
```

## Tracking Operation Duration

Timers can be used to report the duration of an operation (in seconds) to a Summary, Histogram, Gauge or Counter. Wrap the operation you want to measure in a using block.

```csharp
double[] buckets = new double[] { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };

private static readonly IHistogram UploadDuration = 
    MyAppMetricFactory.CreateHistogram(
        "myapp_upload_duration_seconds", 
        "Histogram of file upload durations.", 
        new HistogramConfiguration()
        {
            Buckets = buckets
        });

...

using (UploadDuration.NewTimer())
{
    Scheduler.Upload(file);
}
```

## Counting In-Progress Operations

You can use `Gauge.TrackInProgress()` to track how many concurrent operations are taking place. Wrap the operation you want to track in a using block.

```csharp
private static readonly IGauge UploadsInProgress = 
    MyAppMetricFactory.CreateGauge(
        "myapp_uploads_in_progress", 
        "Number of upload operations occuring.");

...

using (UploadsInProgress.TrackInProgress())
{
    Scheduler.Upload(file);
}
```

## Counting Exceptions

You can use `Counter.CountExceptions()` to count the number of exceptions that occur while executing some code.

```csharp
private static readonly ICounter FailedExtractions =
    MyAppMetricFactory.CreateCounter(
        "myapp_extractions_failed_total", 
        "Number of extraction operations that failed.");

...

FailedExtractions.CountExceptions(() => Extractor.Extract(file));
```

You can also filter the exception types to observe:

```csharp
FailedExtractions.CountExceptions(() => Extractor.Extract(file), IsExtractionRelatedException);

bool IsExtractionRelatedException(Exception ex)
{
    return ex is ExtractionException; // Only count ExtractionExceptions.
}
```

## Mutable Labels

All metrics can have mutable labels, allowing grouping of related time series.

See the best practices on [naming](http://prometheus.io/docs/practices/naming/)
and [labels](http://prometheus.io/docs/practices/instrumentation/#use-labels).
* Labels should contain a limited set of label values.
    * URLs would be a bad choice. There are infinite options.
    * HTTP response codes would be a good choice. There is a finite set of options.

Taking a counter as an example:

```csharp
private static readonly ICounter HttpResponseCounter = 
    MyAppMetricFactory.CreateCounter(
        "myapp_http_responses_total", 
        "Number of responses received by http method and response code.", 
        new CounterConfiguration()
        {
            MutableLabelNames = new string[] { "http_method", "http_response_code" }
        });

...

// Specify the value(s) for the label(s) when you want to call a metric operation.
HttpResponseCounter.WithLabels("GET", "200").Inc();
```

## Immutable Labels

You can add immutable labels that always have fixed values.

Taking a counter as an example with immutable labels and mutable labels:

```csharp
Dictionary<string, string> immutableLabels = new Dictionary<string, string>() { { "service_name", "scheduler" } };
...
private static readonly ICounter HttpResponseCounter = 
    MyAppMetricFactory.CreateCounterWithStaticLabels(
        "myapp_http_responses_received", 
        "Count of responses received, labelled by response code.", 
        new CounterConfiguration()
        {
            ImmutableLabels = immutableLabels
            MutableLabelNames = new string[] { "http_response_code" }
        });

...

// Labels applied to individual instances of the metric.
HttpResponseCounter.WithLabels("404").Inc();
HttpResponseCounter.WithLabels("200").Inc();
```

## Switching to a Different Metrics Library?

* All pulse-projects implement the [pulse](https://github.com/microsoft/pulse) interface, meaning all pulse-projects are interchangable. 
* If you need to change monitoring systems in the future, you can do so without having to change your projects code!
* If a pulse-project does not exist for the metric monitoring system you need to use, you can easily create one by implementing the [pulse](https://github.com/microsoft/pulse) common interface.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).

Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
