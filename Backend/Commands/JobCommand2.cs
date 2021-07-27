using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// Реализация команды для сообщений тип 2
    /// </summary>
    public class JobCommand2 : BaseCommand
    {
        public override void Execute()
        {
            // Какая-то иная логика обработки сообщения

            AppDomain otherDomain = AppDomain.CreateDomain("job domain");

            Type jobType = typeof(Job2);
            var job = otherDomain.CreateInstanceAndUnwrap(jobType.Assembly.FullName, jobType.FullName) as Job2;

            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            job.Process(Args);
        }
    }
}
