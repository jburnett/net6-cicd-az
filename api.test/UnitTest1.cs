using api.Controllers;
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
    private ILogger _logger;
    private IHost _host;

    [TestInitialize]
    public void Init()
    {
        var builder = Host.CreateDefaultBuilder().
            ConfigureLogging((builderContext, loggingBuilder) => {
                loggingBuilder.AddConsole((options) => {
                    options.IncludeScopes = true;
                });
            });

        _host = builder.Build();

    }

    [TestMethod]
    public void TestMethod1()
    {
        var l = _host.Services.GetRequiredService<ILogger<WeatherForecastController>>();
        var ctrl = new WeatherForecastController(l);

        var resp = ctrl.Get();
        Assert.IsNotNull(resp);

        foreach (var wf in resp) {
            System.Console.WriteLine($"{wf.Date}: {wf.Summary}");
        }

        // Assert.IsNull(resp);
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