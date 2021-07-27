using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Commands
{
    /// <summary>
    /// Реализация команды для сообщений тип 1
    /// </summary>
    public class JobCommand1 : BaseCommand
    {
        public override void Execute()
        {
            // Какая-то своя логика обработки сообщения

            AppDomain otherDomain = AppDomain.CreateDomain("job domain");

            Type jobType = typeof(Job1);
            var job = otherDomain.CreateInstanceAndUnwrap(jobType.Assembly.FullName, jobType.FullName) as Job1;

            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            job.Process(Args);
        }
    }
}
