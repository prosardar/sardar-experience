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
    /// Класс, реализующий прослушивание канала по TCP, для последующей обработки входящих сообшений.
    /// Обработка сообщений осуществляется в дочерних domain приложениях, кол-во которых ограничено.
    /// </summary>
    public class LowTCPServer : LowServer
    {
        /// <summary>
        /// Класс для прослушивания порта
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// 
        /// </summary>
        private ServerConfig _serverConfig;

        /// <summary>
        /// Конструктор класса по прослушиванию входящего канала используюя TCP соединение.
        /// </summary>
        /// <param name="servicesProvider">Контейнер для внедрения зависимости</param>
        public LowTCPServer(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serverConfig = serviceProvider.GetRequiredService<ServerConfig>();
        }

        /// <summary>
        /// Инициализация прослушивания TCP порта.
        /// </summary>
        /// <param name="servicesProvider">Контейнер для внедрения зависимости</param>
        public override void Init()
        {
            if (Initialized)
            {
                return;
            }
            Logger.LogTrace("Инициализация...");

            _listener = new TcpListener(IPAddress.Any, _serverConfig.Port);

            Initialized = true;
            Logger.LogTrace("Конец инициализации");
        }

        /// <summary>
        /// Запуск прослушивания TCP порта.
        /// </summary>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogTrace("Запуск сервера...");
            if (_listener == null)
            {
                var msg = "До запуска сервера Вам необходимо сначала вызвать метод Init().";
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
            Logger.LogTrace("Сервер запущен");

            await StartLoopAsync(cancellationToken);
        }

        /// <summary>
        /// Цикл прослушивания и обработки входящего сообщения
        /// </summary>
        /// <param name="cancellationToken">Токен для отмены цикла</param>
        /// <returns>Возвращает Task, необходимый для определения выхода из цикла</returns>
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
                        Logger.LogTrace("Сервер перестал прослушивать порт, потому что был запрошен токен отмены.");
                    }
                    catch (InvalidOperationException)
                    {
                        Logger.LogTrace("Сервер перестал прослушивать порт, потому что был запрошен токен отмены.");
                        cancellationToken.ThrowIfCancellationRequested();
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(new EventId(), ex, $"Получено исключение : {ex.Message}");
                    }
                }
            }
            Started = false;
        }

        /// <summary>
        /// Обработчик полученного сообщения от клиента
        /// </summary>
        /// <param name="state">Объект клиента</param>
        private void ClientThread(object state)
        {
            var client = state as TcpClient;
            if (client == null)
            {
                var msg = "Входящее сообщение не является объектом TcpClient";
                Logger.LogError(msg);
                throw new NullReferenceException(msg);
            }

            var stream = client.GetStream();
            string receiveData = null;
            byte[] receiveBuffer = new byte[256];
            int i;
            try
            {
                // Чтение данных из потока
                while ((i = stream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                {
                    string hex = BitConverter.ToString(receiveBuffer);
                    receiveData = Encoding.ASCII.GetString(receiveBuffer, 0, i);

                    Logger.LogInformation("{1}: Получено: {0}", receiveData, Thread.CurrentThread.ManagedThreadId);

                    // Заносим в очередь на обработку
                    ProcessQueue.Push(receiveData);

                    var response = "OK";
                    byte[] replyBuffer = Encoding.ASCII.GetBytes(response);
                    stream.Write(replyBuffer, 0, replyBuffer.Length);

                    Logger.LogInformation("{1}: Отправлено: {0}", response, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Получено исключение обработки входящего сообщения: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// Остановка прослушивания вх. канала
        /// </summary>
        public override void Stop()
        {
            if (_listener == null)
            {
                var msg = "До остановки сервера, необходимо сначала вызвать Init() потом Start() метод";
                Logger.LogError(msg);
                throw new ApplicationException(msg);
            }
            if (Started == false)
            {
                return;
            }
            Logger.LogTrace("Остановка сервера...");

            _listener.Stop();
            ProcessQueue.Stop();
        }
    }
}