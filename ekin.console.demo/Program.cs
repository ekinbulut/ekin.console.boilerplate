using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace ekin.console.demo
{
    static class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static CancellationTokenSource _cancellationTokenSource;

        static void Main(string[] args)
        {
            try
            {
                System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                _cancellationTokenSource = new CancellationTokenSource();
                
                
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var serviceCollection = SetupContainer(config);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                using (serviceProvider)
                {
                    var process = serviceProvider.GetRequiredService<IDemoProcess>();
                    process.Execute(_cancellationTokenSource.Token);


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
                _cancellationTokenSource.Cancel();
                Console.WriteLine("Process finished...");
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
            serviceCollection.AddTransient(typeof(IDemoProcess), typeof(DemoProcess));
        }

        static void AddSections(IServiceCollection serviceCollection, IConfiguration config)
        {
            serviceCollection.Configure<GeneralApplicationConfig>(t =>
            {
                config.GetSection(nameof(GeneralApplicationConfig));
            });
        }
    }
}