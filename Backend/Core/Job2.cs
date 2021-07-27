using System.Threading;

namespace Backend.Commands
{
    /// <summary>
    /// Класс для обработки команды типа 2
    /// </summary>
    public class Job2 : BaseLowJob
    {
        public override int Process(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
            {
                return 0;
            }

            // Выполняем какую-то долгую работу, в дочернем домене
            // Родительское приложение при необходимости может нас выгрузить        
            Thread.Sleep(5000);

            return cmd.Length;
        }
    }
}
