using System.Threading;

namespace Backend.Commands
{
    /// <summary>
    /// Класс для обработки команды типа 1
    /// </summary>
    public class Job1 : BaseLowJob
    {
        public override int Process(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return 0;
            }

            // Выполняем какую-то долгую работу, в дочернем домене
            // Родительское приложение при необходимости может нас выгрузить        
            Thread.Sleep(10000);

            return cmd.Length;
        }
    }
}
