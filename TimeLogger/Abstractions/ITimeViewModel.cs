using System;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель для отображения суммы времени
    /// </summary>
    public interface ITimeViewModel : IViewModel
    {
        string? Name { get; set; }
        TimeSpan Time { get; set; }
    }
}
