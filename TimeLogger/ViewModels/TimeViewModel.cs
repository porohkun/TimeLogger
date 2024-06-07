using System;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="ITagViewModel" />
    [AsTransient(typeof(ITimeViewModel))]
    public class TimeViewModel : BindableBase, ITimeViewModel
    {
        private readonly IIndicatorsService _indicatorsService;

        private TimeSpan _time;

        public string? Name { get; set; }

        public TimeSpan Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public TimeViewModel(
            string name,
            IIndicatorsService indicatorsService)
        {
            Name = name;
            _indicatorsService = indicatorsService;
            _indicatorsService.IndicatorTimeChanged += (n, t) =>
            {
                if (n == Name)
                    Time = t;
            };
        }
    }
}
