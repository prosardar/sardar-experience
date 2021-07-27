using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Server
{
    public interface ILowServer
    {
        /// <summary>
        /// Инициализация прослушивания канала входящих сообщений.
        /// </summary>        
        void Init();

        /// <summary>
        /// Запуск прослушивания входящего канала
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken);
    }
}