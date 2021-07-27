using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// Реализация команды для сообщений типа 1
    /// </summary>
    public class JobCommand1 : BaseCommand
    {
        /// <summary>
        /// Конструктор команды типа 1
        /// <param name="provider">Контейнер для внедрения зависимости</param>
        /// </summary>
        public JobCommand1(IServiceProvider provider) : base(provider)
        {
        }

        /// <summary>
        /// Метод выполнения команды в под-домене
        /// </summary>k
        public override async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                // Какая-то иная логика обработки сообщения в дочернем домене приложения,
                // которое при необходимости можно хранить и выгружать
                // К сожалению следующий код не работает для .net core, microsoft ещё не реализовали данную возможность

                /*
                    AppDomain otherDomain = AppDomain.CreateDomain("job domain");

                    Type jobType = typeof(Job1);
                    var job = otherDomain.CreateInstanceAndUnwrap(jobType.Assembly.FullName, jobType.FullName) as Job1;

                    Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
                    job.Process(Args);
                */

                Logger.LogTrace("Обработка запроса 1 ... (2 сек)");
                Thread.Sleep(2000);
            });
        }
    }
}
