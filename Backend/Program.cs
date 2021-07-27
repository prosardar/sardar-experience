using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Commands;
using Backend.Core;
using Backend.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace Backend
{
    /// <summary>
    /// Консольное приложение слушающее входящие сообщения, для их обработки в под-доменных приложениях
    /// </summary>
    class Program
    {
        private static Logger _logger;

        private static ILowServer _server;
        private static bool _isCtrlC;

        private static void Main(string[] args)
        {
            IConfiguration config = null;
            try
            {
                config = new ConfigurationBuilder()
                       .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                       .Build();

                _logger = LogManager.Setup()
                                    .LoadConfigurationFromSection(config)
                                    .GetCurrentClassLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Приложение остановлено, по причине невозможности инициализации NLog. {ex.Message}");
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                IServiceProvider servicesProvider = BuildDI(config);
                using (servicesProvider as IDisposable)
                {
                    InitAndStartServer(servicesProvider);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Приложение сервера остановлено, по причине исключения перед запуском.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void InitAndStartServer(IServiceProvider servicesProvider)
        {
            _server = servicesProvider.GetRequiredService<ILowServer>();
            _server.Init();

            CancellationTokenSource cts = new CancellationTokenSource();

            // Регистрация комбинации клавиш Cntl+C
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                _isCtrlC = e.SpecialKey == ConsoleSpecialKey.ControlC;
                var isCtrlBreak = e.SpecialKey == ConsoleSpecialKey.ControlBreak;

                if (_isCtrlC)
                {
                    e.Cancel = true;
                    Console.WriteLine("Ctrl+C словили завершаем работу. ");
                    Console.WriteLine("Нажмите любую клавиши для остановки и выхода из приложения.");
                    cts.Cancel();
                }
            };

            Task startTask = _server.StartAsync(cts.Token);

            Console.WriteLine("Нажмите любую клавиши для остановки и выхода из приложения.");
            Console.ReadKey(true);

            cts.Cancel();

            // Ожидаем 10 сек завершения остановки сервера, иначе идём дальше на завершение приложения
            startTask.Wait(10000);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var msg = e.ExceptionObject.ToString();
            _logger.Error(msg, "Приложение сервера остановлено, по причине необработанного исключения (UnhandledException).");
        }

        private static IServiceProvider BuildDI(IConfiguration config)
        {
            return new ServiceCollection()
               .AddSingleton<ILowServer, LowTCPServer>()
               .AddSingleton<ILowQueue, LowQueue>()
               .AddSingleton<ICommandDispatcher, CommandDispatcher>()
               .AddSingleton(provider =>
               {
                   return config.GetSection(ServerConfig.TagName).Get<ServerConfig>();
               })
               .AddLogging(loggingBuilder =>
               {
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog(config);
               })
               .BuildServiceProvider();
        }
    }
}
