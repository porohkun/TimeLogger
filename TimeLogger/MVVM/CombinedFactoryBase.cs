using System;
using System.Linq;

namespace TimeLogger.MVVM
{
    public abstract class CombinedFactoryBase<T>
    {
        private readonly IServiceProvider _services;

        public CombinedFactoryBase(IServiceProvider services)
        {
            _services = services;
        }

        protected T Create(params object?[] externalArguments)
        {
            var constructor = typeof(T).GetConstructors().FirstOrDefault(c => c.IsPublic);
            if (constructor == null)
                throw new Exception($"Фабрике не удалось найти подходящий конструктор для {nameof(T)}");

            var args = externalArguments
                .Concat(constructor
                    .GetParameters()
                    .Skip(externalArguments.Length)
                    .Select(p => _services.GetService(p.ParameterType)))
                .ToArray();

            var result = constructor.Invoke(args);
            if (result == null)
                throw new ArgumentException($"Не удалось создать {nameof(T)}");

            return (T)result;
        }
    }
}
