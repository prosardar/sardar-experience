using Backend.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core
{
    public class LowQueue : ILowQueue
    {
        private ConcurrentQueue<string> _queue;

        /// <summary>
        /// Объект создающий команды для обработки сообщений
        /// </summary>
        private ICommandDispatcher _commandDispatcher;

        public LowQueue(ICommandDispatcher dispatcher)
        {
            _commandDispatcher = dispatcher;

            _queue = new ConcurrentQueue<string>();
        }

        public async Task StartAsync()
        {
            await LoopAsync();
        }

        public async void Stop()
        {
           
        }

        private async Task LoopAsync()
        {
            while (true)
            {
                // Peek at the first element.
                string request;
                if (_queue.TryDequeue(out request) == false)
                {

                }
                else
                {
                    try
                    {
                        BaseCommand command = _commandDispatcher.Dispatch(request);

                        command.Execute();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public void Push(string data)
        {
            _queue.Enqueue(data);
        }
    }
}
