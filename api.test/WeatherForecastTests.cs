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
    private const char _indentChar = '\t';

    [TestInitialize]
    public void Init()
    {
        // Acquire logger by building it explicitly
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration( (builderContext, configBuilder) => {
                System.Console.WriteLine($"Environment: {builderContext.HostingEnvironment.EnvironmentName}");
                // configBuilder.AddJsonFile("appsettings.json");
                // configBuilder.AddEnvironmentVariables(prefix: "net6_cicd_");
            })
            .ConfigureLogging((builderContext, loggingBuilder) => {
                loggingBuilder.AddSimpleConsole((options) => {
                    options.IncludeScopes = true;
                });
            });

        _host = builder.Build();
    }


    [TestMethod]
    public void CanWalkConfig()
    {
        if (_host != null)
        {
            var cfg = _host.Services.GetRequiredService<IConfiguration>();
            DumpConfigInfo(cfg);
            Assert.IsNotNull(cfg);
        }
        else {
            Assert.Inconclusive("IHost not available");
        }
    }

    [TestMethod]
    public void RetrieveConfig()
    {
        if (_host != null)
        {
            var cfg = _host.Services.GetRequiredService<IConfiguration>();
            Assert.IsNotNull(cfg);
            var y = cfg["AllowedHosts"];
            Assert.AreEqual(y, "*");

            var s = cfg.GetSection("Logging");
            Assert.IsNotNull(s);
            System.Console.WriteLine($"Current section is {s.Key}; iterate children");
            foreach (var c in s.GetChildren())
            {
                System.Console.WriteLine($"\tKey: {c.Key}");
            }
            
            s = cfg.GetSection("Logging.LogLevel");
            Assert.IsNotNull(s);

            // System.Console.WriteLine($"Iterating children of {s}");
            System.Console.WriteLine($"Current section is {s.Key}; iterate children");
            foreach (var c in s.GetChildren())
            {
                System.Console.WriteLine($"\tKey: {c.Key}");
            }

            // var v = s.GetValue(typeof(string), "Default");
            // Assert.IsNotNull(v);

            Assert.IsNotNull(s.Key);
            Assert.IsNotNull(s.GetChildren());
        }
    }


    [TestMethod]
    public void RetrieveFooConfig()
    {
        if (_host != null)
        {
            var cfg = _host.Services.GetRequiredService<IConfiguration>();
            Assert.IsNotNull(cfg);
            var y = cfg["AllowedHosts"];
            Assert.AreEqual(y, "*");

        }
        else {
            Assert.Inconclusive("IHost not available");
        }
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
        }
        else {
            Assert.Inconclusive("IHost not available");
        }
    }



    [TestMethod]
    public void TestMethod2()
    {
        // Acquire logger by building directly via factory
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


    private string BuildIndent(int indentLevel = 0)
    {
        if (0 < indentLevel)
        {
            return new string(_indentChar, indentLevel);
        }
        else
        {
            return string.Empty;
        }
    }


    private void DumpConfigInfo(object cfg, int indentLevel = 0)
    {
        if (null != cfg) 
        {
            string indent = BuildIndent(indentLevel);

            if (cfg is IConfiguration)
            {
                System.Console.WriteLine($"{indent}{cfg} is IConfiguration");
            }
            if (cfg is IConfigurationRoot)
            {
                System.Console.WriteLine($"{indent}{cfg} is IConfigurationRoot with Providers:");
                foreach (var p in ((IConfigurationRoot)cfg).Providers)
                {
                    System.Console.WriteLine($"{indent}{_indentChar}{p}");
                }
                System.Console.WriteLine($"{indent}---- end Providers -----");
            }
            if (cfg is IConfigurationSection)
            {
                System.Console.WriteLine($"{indent}{cfg} is IConfigurationSection");
            }

            System.Console.WriteLine($"{indent}===== ReportSections =====");
            foreach (var c in ((IConfiguration)cfg).GetChildren())
            {
                ReportSections(c, indentLevel + 1);
            }
        }
    }

    private void ReportSections(IConfigurationSection s, int indentLevel = 0)
    {
        if (null != s)
        {
            string indent = BuildIndent(indentLevel);
            if (!string.IsNullOrWhiteSpace(s.Key))
            {
                System.Console.WriteLine($"{indent}ConfigSection: {s.Key} = {s.Value}");
            }

            foreach (var c in s.GetChildren())
            {
                if (c is IConfigurationSection)
                {
                    System.Console.WriteLine("<recurse>");
                    ReportSections(c, indentLevel + 1);
                    System.Console.WriteLine("<ascend>");
                }
            }
        }
    }

}