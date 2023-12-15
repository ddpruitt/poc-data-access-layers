using System.Reflection;
using CommandLine;
using Serilog;

using DC.PoC.DaLayers.Console.Logging;
using DC.PoC.DaLayers.Console.Options;

namespace DC.PoC.DaLayers.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Run the CLI Parser to check for errors.  Stop if stuff is missing.
            var helpWriter = new StringWriter();
            var parser = new Parser(with => with.HelpWriter = helpWriter);
            parser.ParseArguments<UploadOptions>(args)
                //.WithParsed(opts => opts.Dump())
                .WithNotParsed(errs =>
                {
                    if (errs.IsVersion() || errs.IsHelp())
                        System.Console.WriteLine(helpWriter.ToString());
                    else
                        System.Console.Error.WriteLine(helpWriter.ToString());

                    Environment.Exit(0);
                });

            // Bootstrap Logger to ensure logging is functioning during startup.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Assembly", Assembly.GetEntryAssembly()?.GetName()!)
                .CreateBootstrapLogger();

            try
            {
                Log.Information(LoggingResources.Divider);
                Log.Information("Upload starts");

                var host = Host
                    .CreateDefaultBuilder(args)
                    .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddOptions<InputFileProcessOptions>()
                            .Bind(hostContext.Configuration.GetSection(InputFileProcessOptions.ConfigurationSectionName));

                        services
                            .AddOptions<UploadOptions>()
                            .Configure(opt => Parser.Default.ParseArguments(() => opt, args));

                        services.AddHostedService<Worker>();
                    })
                    .UseSerilog((context, services, config) => config
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Assembly", Assembly.GetEntryAssembly()?.GetName()!))
                    .Build();

                host.Run();
            }
            catch (Exception e)
            {
                Log.Fatal(LoggingResources.Divider);
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("Upload ends.");
                Log.Information(LoggingResources.Divider);
                Log.CloseAndFlush();
            }

        }
    }
}