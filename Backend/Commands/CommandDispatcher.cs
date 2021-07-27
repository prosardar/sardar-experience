
using System;
using System.Collections.Concurrent;

namespace Backend.Commands
{
    /// <summary>
    /// Реализация класса диспетчера команд, который создаёт команду на основе запроса
    /// </summary>
    public class CommandDispatcher : ICommandDispatcher
    {
        /// <summary>
        /// Потокобезопасный словарь запросов и типа команды
        /// </summary>
        private readonly ConcurrentDictionary<string, Type> _commandTypes;

        /// <summary>
        /// Контейнер зависимостей
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Конструктор класса диспетчеризатора команд на основе запросов
        /// </summary>
        /// <param name="provider">Контейнер зависимостей</param>
        public CommandDispatcher(IServiceProvider provider)
        {
            serviceProvider = provider;
            _commandTypes = new ConcurrentDictionary<string, Type>();
            _commandTypes.TryAdd("Command1", typeof(JobCommand1));
            _commandTypes.TryAdd("Command2", typeof(JobCommand2));
        }

        /// <summary>
        /// Метод диспетчиризации создания команды по заросу
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns></returns>
        public BaseCommand Dispatch(string request)
        {
            if (string.IsNullOrEmpty(request))
            {
                throw new ArgumentNullException(nameof(request), "Пустой запрос сообщения");
            }

            if (_commandTypes.ContainsKey(request) == false)
            {
                throw new ApplicationException($"Не найден команда обработчика для запроса {request}");
            }

            // Тут вроде как можно обойтись обычным взятием без TryGetValue, так как словарь не меняется во время работы
            Type commandType;
            if (_commandTypes.TryGetValue(request, out commandType) == false)
            {
                throw new ApplicationException($"Не удалось получить тип команды обработчика для запроса {request}");
            }

            BaseCommand command = (BaseCommand)Activator.CreateInstance(commandType, new[] { serviceProvider });
            command.Args = request;

            return command;
        }
    }
}
