using Backend.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Server
{
    /// <summary>
    /// �����, ����������� ������������� ������ �� TCP, ��� ����������� ��������� �������� ���������.
    /// ��������� ��������� �������������� � �������� domain �����������, ���-�� ������� ����������.
    /// </summary>
    public class LowTCPServer : LowServer
    {
        /// <summary>
        /// ����� ��� ������������� �����
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// 
        /// </summary>
        private ServerConfig _serverConfig;

        /// <summary>
        /// ����������� ������ �� ������������� ��������� ������ ���������� TCP ����������.
        /// </summary>
        /// <param name="servicesProvider">��������� ��� ��������� �����������</param>
        public LowTCPServer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serverConfig = serviceProvider.GetRequiredService<ServerConfig>();
        }

        /// <summary>
        /// ������������� ������������� TCP �����.
        /// </summary>
        /// <param name="servicesProvider">��������� ��� ��������� �����������</param>
        public override void Init()
        {
            if (Initialized)
            {
                return;
            }
            Logger.LogTrace("�������������...");

            _listener = new TcpListener(IPAddress.Any, _serverConfig.Port);

            Initialized = true;
            Logger.LogTrace("����� �������������");
        }

        /// <summary>
        /// ������ ������������� TCP �����.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace("������ �������...");
            if (_listener == null)
            {
                var msg = "�� ������� ������� ��� ���������� ������� ������� ����� Init().";
                Logger.LogError(msg);
                throw new ApplicationException(msg);
            }
            if (Started)
            {
                return;
            }

            _listener.Start();
            await ProcessQueue.StartAsync();

            Started = true;
            Logger.LogTrace("������ �������");

            await StartLoopAsync(cancellationToken);
        }

        /// <summary>
        /// ���� ������������� � ��������� ��������� ���������
        /// </summary>
        /// <param name="cancellationToken">����� ��� ������ �����</param>
        /// <returns>���������� Task, ����������� ��� ����������� ������ �� �����</returns>
        private async Task StartLoopAsync(CancellationToken cancellationToken)
        {
            using (cancellationToken.Register(Stop))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        TcpClient client = await _listener.AcceptTcpClientAsync();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);
                    }
                    catch (SocketException)
                    {
                        Logger.LogTrace("������ �������� ������������ ����, ������ ��� ��� �������� ����� ������.");
                    }
                    catch (InvalidOperationException)
                    {
                        Logger.LogTrace("������ �������� ������������ ����, ������ ��� ��� �������� ����� ������.");
                        cancellationToken.ThrowIfCancellationRequested();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId(), ex, $"�������� ���������� : {ex.Message}");
                    }
                }
            }
            Started = false;
        }

        /// <summary>
        /// ���������� ����������� ��������� �� �������
        /// </summary>
        /// <param name="state">������ �������</param>
        private void ClientThread(object state)
        {
            var client = state as TcpClient;
            if (client == null)
            {
                var msg = "�������� ��������� �� �������� �������� TcpClient";
                Logger.LogError(msg);
                throw new NullReferenceException(msg);
            }

            var stream = client.GetStream();
            string receiveData = null;
            byte[] receiveBuffer = new byte[256];
            int i;
            try
            {
                // ������ ������ �� ������
                while ((i = stream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                {
                    string hex = BitConverter.ToString(receiveBuffer);
                    receiveData = Encoding.ASCII.GetString(receiveBuffer, 0, i);

                    Logger.LogInformation("{1}: ��������: {0}", receiveData, Thread.CurrentThread.ManagedThreadId);

                    // ������� � ������� �� ���������
                    ProcessQueue.Push(receiveData);

                    var response = "OK";
                    byte[] replyBuffer = Encoding.ASCII.GetBytes(response);
                    stream.Write(replyBuffer, 0, replyBuffer.Length);

                    Logger.LogInformation("{1}: ����������: {0}", response, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"�������� ���������� ��������� ��������� ���������: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// ��������� ������������� ��. ������
        /// </summary>
        public override void Stop()
        {
            if (_listener == null)
            {
                var msg = "�� ��������� �������, ���������� ������� ������� Init() ����� Start() �����";
                Logger.LogError(msg);
                throw new ApplicationException(msg);
            }
            if (Started == false)
            {
                return;
            }
            Logger.LogTrace("��������� �������...");

            _listener.Stop();
            ProcessQueue.Stop();
        }
    }
}