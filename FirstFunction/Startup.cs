using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

[assembly: FunctionsStartup(typeof(FirstFunction.Startup))]
namespace FirstFunction
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddLogging(loggingBuilder
            //    => loggingBuilder.AddSerilog(new LoggerConfiguration()
            //    .WriteTo.ApplicationInsights(Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY").ToString(), new TraceTelemetryConverter()).CreateLogger())
            //    );
        }
    }
}
