using System;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.ViewModels;

namespace TimeLogger.MVVM
{
    [AsSingletone]
    public class TimeViewModelFactory : CombinedFactoryBase<TimeViewModel>
    {
        public TimeViewModelFactory(IServiceProvider services) : base(services) { }

        public ITimeViewModel Create(string name)
        {
            return base.Create(name);
        }
    }
}
