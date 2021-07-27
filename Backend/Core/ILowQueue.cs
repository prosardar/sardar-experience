using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task StartAsync();

        /// <summary>
        /// Остановка выполнения Job из очережи
        /// </summary>
        void Stop();
    }
}
