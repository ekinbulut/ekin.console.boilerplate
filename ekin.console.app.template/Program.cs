using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace ekin.console.app.template
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            try
            {
                System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var serviceCollection = SetupContainer(config);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                using (serviceProvider)
                {

                    //serviceProvider.GetRequiredService<#serviceName#>()

                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception at Program startup.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error(e.ExceptionObject.ToString());
        }

        static IServiceCollection SetupContainer(IConfiguration config)
        {
            var serviceCollection = new ServiceCollection()
                .AddOptions()
                .AddLogging(t =>
                {
                    t.ClearProviders();
                    t.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    t.AddNLog(config);
                });

            //extent register options

            AddComponents(serviceCollection);
            AddSections(serviceCollection, config);

            return serviceCollection;
        }

        static void AddComponents(IServiceCollection serviceCollection)
        {
            // add components
        }

        static void AddSections(IServiceCollection serviceCollection, IConfiguration config)
        {
            // serviceCollection.Configure<#someconfig#>(config.GetSection(nameof(#someconfig#));
        }


    }
}
