
using System;
using System.Collections.Concurrent;

namespace Backend.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly ConcurrentDictionary<string, Type> _commandTypes;

        public CommandDispatcher()
        {
            _commandTypes = new ConcurrentDictionary<string, Type>();
            _commandTypes.TryAdd("Command1", typeof(JobCommand1));
            _commandTypes.TryAdd("Command2", typeof(JobCommand2));
        }

        public BaseCommand Dispatch(string request)
        {
            if (string.IsNullOrEmpty(request))
            {
                throw new ArgumentNullException(nameof(request), "������ ������ ���������");
            }

            if (_commandTypes.ContainsKey(request) == false)
            {
                throw new ApplicationException($"�� ������ ������� ����������� ��� ������� {request}");
            }

            // ��� ����� ��� ����� �������� ������� ������� ��� TryGetValue, ��� ��� ������� �� �������� �� ����� ������
            Type commandType;
            if (_commandTypes.TryGetValue(request, out commandType) == false)
            {
                throw new ApplicationException($"�� ������� �������� ��� ������� ����������� ��� ������� {request}");
            }

            BaseCommand command = (BaseCommand)Activator.CreateInstance(commandType);
            command.Args = request;

            return command;
        }
    }
}
