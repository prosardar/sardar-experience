using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// ����������� ����� ��� ���� ������ ��������� ���������
    /// </summary>
    public abstract class BaseCommand
    {
        protected readonly ILogger<BaseCommand> Logger;

        /// <summary>
        /// ������������ ��������� ��� �������
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// ��� ������� ��������� ���������� �������
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// ��������� ������������
        /// </summary>
        protected IServiceProvider ServiceProvider;

        /// <summary>
        /// ����������� ������� �������
        /// <param name="provider">��������� ��� ��������� �����������</param>
        /// </summary>
        protected BaseCommand(IServiceProvider provider)
        {
            ServiceProvider = provider;
            Logger = provider.GetRequiredService<ILogger<BaseCommand>>();
        }

        /// <summary>
        /// ����� ���������� �������
        /// </summary>
        public abstract Task ExecuteAsync();
    }
}