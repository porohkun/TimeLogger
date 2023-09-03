using System;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc/>
    [AsTransient(typeof(ITimeViewModel))]
    public class TimeViewModel : BindableBase, ITimeViewModel
    {
        public string? Name { get; set; }
        public TimeSpan Time { get; set; }
    }
}
