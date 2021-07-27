
using System;
using System.Collections.Concurrent;

namespace Backend.Commands
{
    /// <summary>
    /// ���������� ������ ���������� ������, ������� ������ ������� �� ������ �������
    /// </summary>
    public class CommandDispatcher : ICommandDispatcher
    {
        /// <summary>
        /// ���������������� ������� �������� � ���� �������
        /// </summary>
        private readonly ConcurrentDictionary<string, Type> _commandTypes;

        /// <summary>
        /// ��������� ������������
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// ����������� ������ ���������������� ������ �� ������ ��������
        /// </summary>
        /// <param name="provider">��������� ������������</param>
        public CommandDispatcher(IServiceProvider provider)
        {
            serviceProvider = provider;
            _commandTypes = new ConcurrentDictionary<string, Type>();
            _commandTypes.TryAdd("Command1", typeof(JobCommand1));
            _commandTypes.TryAdd("Command2", typeof(JobCommand2));
        }

        /// <summary>
        /// ����� ��������������� �������� ������� �� ������
        /// </summary>
        /// <param name="request">������</param>
        /// <returns></returns>
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

            BaseCommand command = (BaseCommand)Activator.CreateInstance(commandType, new[] { serviceProvider });
            command.Args = request;

            return command;
        }
    }
}
