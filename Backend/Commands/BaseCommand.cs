using System;

namespace Backend.Commands
{
    /// <summary>
    /// ����������� ����� ��� ���� ������ ��������� ���������
    /// </summary>
    public abstract class BaseCommand
    {
        public string Args { get; set; }

        public string Result { get; set; }

        public abstract void Execute();
    }
}