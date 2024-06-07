using System;
using System.ComponentModel;
using TimeLogger.Abstractions;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="ITagViewModel" />
    public class TimeViewModelDesignDummy : ITimeViewModel
    {
        public string? Name { get; set; }
        public TimeSpan Time { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
