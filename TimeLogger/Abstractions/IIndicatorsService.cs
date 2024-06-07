using System;
using System.Collections.Generic;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Сервис работы с индикаторами времени
    /// </summary>
    public interface IIndicatorsService
    {
        event Action IndicatorsChanged;
        event Action<string, TimeSpan>? IndicatorTimeChanged;

        IEnumerable<string> IndicatorsNames { get; }
    }
}
