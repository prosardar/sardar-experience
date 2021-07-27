using System;
using System.Threading;

/// <summary>
/// ����������� ����� ��� ����� � ���������� ����������
/// </summary>
public abstract class BaseLowJob : MarshalByRefObject
{
    /// <summary>
    /// ����� ���������� ������ � ��������� ��������� ����������
    /// </summary>
    /// <param name="data">������� ������ ��� ���������� Job</param>
    /// <returns></returns>
    public abstract int Process(string data);
}