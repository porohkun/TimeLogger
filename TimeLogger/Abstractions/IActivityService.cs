using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeLogger.Domain.Data;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Сервис работы с активностями
    /// </summary>
    public interface IActivityService
    {
        Activity? SelectedActivity { get; }

        event Action? ActivitySelected;
        event Action<long>? ActivityUpdated;

        Task<long> CreateNewActivity(string key, string name, IEnumerable<string> tags);

        Task<long> UpdateActivity(long id, string key, string name, IEnumerable<string> tags);

        Task SelectActivity(long id);

        Task StartActivity(long id);

        Task StopActivity(long id);

        Task ArchiveActivity(long id, bool archive);
    }
}
