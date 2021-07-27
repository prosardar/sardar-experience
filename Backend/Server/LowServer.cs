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
    /// ����������� �����, ����������� ������ ��� ������������� ������, � ����������� ���������� �������� ���������.
    /// ��������� ��������� �������������� � �������� domain �����������.
    /// </summary>
    public abstract class LowServer : ILowServer
    {
        protected readonly ILogger<LowServer> Logger;

        /// <summary>
        /// ������� ��������� ���������
        /// </summary>
        protected readonly ILowQueue ProcessQueue;

        /// <summary>
        /// ����, ����������� �� ��, ��� ������ ��� �������.
        /// </summary>
        protected bool Started;

        /// <summary>
        /// ����, ����������� �� ��, ��� ������ ��� ���������������.
        /// </summary>
        protected bool Initialized;

        /// <summary>
        /// ����������� ������ �� ������������� ��������� ������, � ����������� ����������
        /// </summary>
        /// <param name="servicesProvider">��������� ��� ��������� �����������</param>
        public LowServer(IServiceProvider servicesProvider)
        {
            Logger = servicesProvider.GetRequiredService<ILogger<LowServer>>();
            ProcessQueue = servicesProvider.GetRequiredService<ILowQueue>();
        }

        /// <summary>
        /// ������������� ������������� ������. ��������� ����������� ��. ���������.
        /// </summary>
        /// <param name="servicesProvider">��������� ��� ��������� �����������</param>
        public abstract void Init();

        /// <summary>
        /// ������ ������������� ��. ������
        /// </summary>
        public abstract Task StartAsync(CancellationToken cancellationToken);
    }
}