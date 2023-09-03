using System;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.ViewModels;

namespace TimeLogger.MVVM
{
    [AsSingletone]
    public class ActivityViewModelFactory : CombinedFactoryBase<ActivityViewModel>
    {
        public ActivityViewModelFactory(IServiceProvider services) : base(services) { }

        public IActivityViewModel Create(Activity activity, Action<IActivityViewModel>? selectActivityCallback = null)
        {
            return base.Create(activity, selectActivityCallback);
        }
    }
}
