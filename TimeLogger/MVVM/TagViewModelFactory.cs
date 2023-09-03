using System;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.ViewModels;

namespace TimeLogger.MVVM
{
    [AsSingletone]
    public class TagViewModelFactory : CombinedFactoryBase<TagViewModel>
    {
        public TagViewModelFactory(IServiceProvider services) : base(services) { }

        public ITagViewModel Create(Tag tag)
        {
            return base.Create(tag);
        }
    }
}
