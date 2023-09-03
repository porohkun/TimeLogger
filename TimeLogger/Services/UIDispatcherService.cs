using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;

namespace TimeLogger.Services
{
    /// <inheritdoc/>
    [AsSingletone(typeof(IUIDispatcherService))]
    public class UIDispatcherService : IUIDispatcherService
    {
        private readonly Dispatcher _uiDispatcher = Dispatcher.CurrentDispatcher;

        public void Invoke(Action? action)
        {
            if (action != null)
            {
                _uiDispatcher.Invoke(action);
            }
        }

        public async Task InvokeAsync(Action? action)
        {
            if (action != null)
            {
                await _uiDispatcher.InvokeAsync(action);
            }
        }
    }
}
