using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using WebinarTelegram.PilotWrapper.Services;
using WebinarTelegram.TelegramBot.Services;

namespace WebinarTelegram
{
    internal class Program
    {
        private static Logger _logger;

        private static void Main(string[] args)
        {
            try
            {
                StartServicesProvider(args);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Stopped program because of exception " + ex.Message);
                Console.ReadKey();
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void StartServicesProvider(string[] args)
        {
            LogManager.Setup();

            _logger = LogManager.GetCurrentClassLogger();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            CreateHostBuilder(args, config).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IServerConnector, ServerConnector>();
                    services.AddScoped<IObjectModifier, ObjectModifier>();
                    services.AddSingleton<IObjectsRepository, ObjectsRepository>();
                    services.AddSingleton<IObjectManager, ObjectManager>();
                    services.AddSingleton<IBot, Bot>();
                });
        }
    }
}