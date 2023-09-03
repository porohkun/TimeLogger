using System;
using System.Threading.Tasks;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Диспетчер работы с UI потоком
    /// </summary>
    public interface IUIDispatcherService
    {
        void Invoke(Action action);
        Task InvokeAsync(Action action);
    }
}
