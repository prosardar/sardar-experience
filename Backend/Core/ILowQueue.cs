using System.Threading;
using System.Threading.Tasks;

namespace Backend.Core
{
    /// <summary>
    /// Интерфейс, задающий поведение ограниченной очереди для выполнения каких-либо задач
    /// </summary>
    public interface ILowQueue
    {
        /// <summary>
        /// Добавить в очередь обработку данных
        /// </summary>
        /// <param name="data"></param>
        void Push(string data);

        /// <summary>
        /// Запуск цикла выполнения Job из очереди
        /// </summary>
        /// <param name="cancellationToken">Токен для отмены цикла</param>
        Task StartAsync(CancellationToken cancellationToken);
    }
}
