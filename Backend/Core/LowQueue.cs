using Backend.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Core
{
    /// <summary>
    /// Класс для обработки запросов используя внутреннюю очередь
    /// </summary>    
    public class LowQueue : ILowQueue
    {
        private ILogger<LowQueue> _logger;

        /// <summary>
        /// Время ожидания добавления запроса в очередь
        /// </summary>
        private const int TIMEOUTPULSE = 10000;

        /// <summary>
        /// Событие пульса для обработки запросов
        /// </summary>
        private SemaphoreSlim _pulseRequestProcessEvent;

        /// <summary>
        /// Очередь запросов
        /// </summary>
        private ConcurrentQueue<string> _requestQueue;

        /// <summary>
        /// Объект, создающий команды для обработки запросов
        /// </summary>
        private ICommandDispatcher _commandDispatcher;

        /// <summary>
        /// Конструктор класса обработки запросов из очереди
        /// </summary>
        /// <param name="serviceProvider">Контейнер зависимостей</param>
        public LowQueue(IServiceProvider serviceProvider)
        {
            _commandDispatcher = serviceProvider.GetRequiredService<ICommandDispatcher>();
            _logger = serviceProvider.GetRequiredService<ILogger<LowQueue>>();

            _requestQueue = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Метод запускающий обработку запросов из очереди
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogTrace("Запуск обработки запросов из очереди...");
            _pulseRequestProcessEvent = new SemaphoreSlim(1);

            Task requestTask = LoopAsync(cancellationToken);

            _logger.LogTrace("Цикл обработки запросов из очереди запущен");

            await requestTask;
        }

        /// <summary>
        /// Метод останавливающий обработку запросов из очереди
        /// </summary>
        private void Stop()
        {
            _logger.LogTrace("Остановка выполнения запросов из очереди...");

            // Освобождаем какие-то ресурсы...

            _logger.LogTrace("Очередь запросов остановлена");
        }

        /// <summary>
        /// Глобальный цикл обработки запросов из очереди
        /// </summary>
        private async Task LoopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                // если токен отмены успели отправить до старта глобального ниже цикла, то предварительно останавливаем работу
                using (cancellationToken.Register(Stop))
                {
                    // глобальный цикл обработки запросов, пока нет токена отмены
                    while (cancellationToken.IsCancellationRequested == false)
                    {
                        // Ожидаем добавления запроса в очередь
                        // Если вышли по тайм-ауту, то выходим на глобальный цикл, п
                        if (_pulseRequestProcessEvent.Wait(TIMEOUTPULSE) == false)
                        {
                            _logger.LogTrace("Очередь запросов пустая");
                            continue;
                        }
                        // появились новые запросы в очереди, обрабатываем их пока очередь не пуста и нет отмены завершения глобального цикла
                        while (_requestQueue.IsEmpty == false && cancellationToken.IsCancellationRequested == false)
                        {
                            // Достаём из очереди первый запрос
                            string request;
                            if (_requestQueue.TryDequeue(out request) == false)
                            {
                                continue;
                            }
                            try
                            {
                                BaseCommand command = _commandDispatcher.Dispatch(request);
                                command.ExecuteAsync();
                            }
                            catch (InvalidOperationException)
                            {
                                _logger.LogTrace("Сервер перестал прослушивать порт, потому что был запрошен токен отмены.");
                                cancellationToken.ThrowIfCancellationRequested();
                                throw;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(new EventId(), ex, $"При выполнении команды получено исключение : {ex.Message}");
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Метод добавления запроса в очередь
        /// </summary>
        public void Push(string request)
        {
            _requestQueue.Enqueue(request);
            _pulseRequestProcessEvent.Release();
        }
    }
}
