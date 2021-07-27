using Backend.Commands;
using Backend.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Server
{
    /// <summary>
    /// Абстрактный класс, описывающий сервер для прослушивание канала, с последующей обработкой входящих сообшений.
    /// Обработка сообщений осуществляется в дочерних domain приложениях.
    /// </summary>
    public abstract class LowServer : ILowServer
    {
        protected readonly ILogger<LowServer> Logger;

        /// <summary>
        /// Очередь обработки сообщений
        /// </summary>
        protected readonly ILowQueue ProcessQueue;

        /// <summary>
        /// Флаг, указывающий на то, что сервер был запущен.
        /// </summary>
        protected bool Started;

        /// <summary>
        /// Флаг, указывающий на то, что сервер был инициализирован.
        /// </summary>
        protected bool Initialized;

        /// <summary>
        /// Конструктор класса по прослушиванию входящего канала, с последующей обработкой
        /// </summary>
        /// <param name="servicesProvider">Контейнер для внедрения зависимости</param>
        public LowServer(IServiceProvider servicesProvider)
        {
            Logger = servicesProvider.GetRequiredService<ILogger<LowServer>>();
            ProcessQueue = servicesProvider.GetRequiredService<ILowQueue>();
        }

        /// <summary>
        /// Инициализация прослушивания канала. Настройка обработчика вх. сообщений.
        /// </summary>
        /// <param name="servicesProvider">Контейнер для внедрения зависимости</param>
        public abstract void Init();

        /// <summary>
        /// Запуск прослушивания вх. канала
        /// </summary>
        public abstract Task StartAsync(CancellationToken cancellationToken);
    }
}