using System;
using Microsoft.Extensions.DependencyInjection;
using TimeLogger.Attributes;

namespace TimeLogger.MVVM
{
    [AsSingletone]
    public class UniversalFactory
    {
        private readonly IServiceProvider _services;
        public UniversalFactory(IServiceProvider services)
        {
            _services = services;
        }

        public T Create<T>() where T : class
        {
            return _services.GetService<T>()
                ?? throw new ArgumentException($"Type '{nameof(T)}' is not registered.");
        }
    }
}
