using api.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace api.test;

[TestClass]
[ExcludeFromCodeCoverage]
public class UnitTest1
{
    private ILogger _logger = NullLogger.Instance;
    private IHost? _host;

    [TestInitialize]
    public void Init()
    {
        // Acquire logger by building it explicitly
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration( (hostingContext, config) => {
                config.AddJsonFile("appsettings.json");
                config.AddEnvironmentVariables(prefix: "net6_cicd_");
            })
            .ConfigureLogging((builderContext, loggingBuilder) => {
                loggingBuilder.AddSimpleConsole((options) => {
                    options.IncludeScopes = true;
                });
            });

        _host = builder.Build();

    }

    [TestMethod]
    public void TestMethod1()
    {
        if (_host != null)
        {
            // Acquire logger via host's services
            var l = _host.Services.GetRequiredService<ILogger<WeatherForecastController>>();
            var ctrl = new WeatherForecastController(l);

            var resp = ctrl.Get();
            Assert.IsNotNull(resp);

            foreach (var wf in resp) {
                System.Console.WriteLine($"{wf.Date}: {wf.Summary}");
            }
            // Assert.IsNull(resp);
        }
    }



    [TestMethod]
    public void TestMethod2()
    {
        using (var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole())) {
            var logger = loggerFactory.CreateLogger<WeatherForecastController>();
            var ctrl = new WeatherForecastController(logger);
            var resp = ctrl.Get();
            Assert.IsNotNull(resp);

            foreach (var wf in resp) {
                System.Console.WriteLine($"{wf.Date}: {wf.Summary}");
            }

        };

        // Assert.IsNull("Force fail on TestMethod2");
    }
}