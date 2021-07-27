using System;
using System.Threading;

/// <summary>
/// Абстрактный класс для работ в поддоменах приложения
/// </summary>
public abstract class BaseLowJob : MarshalByRefObject
{
    /// <summary>
    /// Метод выполнения работы в отдельном поддомене приложения
    /// </summary>
    /// <param name="data">Входные данные для выполнения Job</param>
    /// <returns></returns>
    public abstract int Process(string data);
}