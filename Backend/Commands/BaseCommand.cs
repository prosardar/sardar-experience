using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// Абстрактный класс для всех команд обработки сообщений
    /// </summary>
    public abstract class BaseCommand
    {
        protected readonly ILogger<BaseCommand> Logger;

        /// <summary>
        /// Передаваемые параметры для команды
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// При наличии результат выполнения команды
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Контейнер зависимостей
        /// </summary>
        protected IServiceProvider ServiceProvider;

        /// <summary>
        /// Конструктор базовой команды
        /// <param name="provider">Контейнер для внедрения зависимости</param>
        /// </summary>
        protected BaseCommand(IServiceProvider provider)
        {
            ServiceProvider = provider;
            Logger = provider.GetRequiredService<ILogger<BaseCommand>>();
        }

        /// <summary>
        /// Метод выполнения команды
        /// </summary>
        public abstract Task ExecuteAsync();
    }
}