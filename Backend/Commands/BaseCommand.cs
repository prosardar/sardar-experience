using System;

namespace Backend.Commands
{
    /// <summary>
    /// Абстрактный класс для всех команд обработки сообщений
    /// </summary>
    public abstract class BaseCommand
    {
        public string Args { get; set; }

        public string Result { get; set; }

        public abstract void Execute();
    }
}