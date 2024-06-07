using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.Models;
using TimeLogger.MVVM;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Services
{
    /// <inheritdoc/>
    [AsSingletone(typeof(IIndicatorsService))]
    public class IndicatorsService : IIndicatorsService
    {
        private readonly IRepository<Period> _periodsRepository;
        private readonly IActivityService _activityService;
        private readonly BackgroundWorker _worker;
        private readonly List<BaseIndicatorModel> _indicators;

        private Action? _indicatorsChanged;

        // Событие, которое вызывается при подписке нового подписчика
        public event Action IndicatorsChanged
        {
            add
            {
                _indicatorsChanged += value;
                value.Invoke();
            }
            remove => _indicatorsChanged -= value;
        }

        public event Action<string, TimeSpan>? IndicatorTimeChanged;

        public IEnumerable<string> IndicatorsNames => _indicators.Select(i => i.Name);

        public IndicatorsService(
            IRepository<Period> periodsRepository,
            IActivityService activityService,
            UniversalFactory factory)
        {
            _periodsRepository = periodsRepository;
            _activityService = activityService;
            _activityService.IsStartedChanged += _ => Task.Run(SwitchIndicators).Wait();
            _activityService.ActivitySelected += () => Task.Run(SwitchIndicators).Wait();

            _indicators = new List<BaseIndicatorModel>
            {
                factory.Create<TodayIndicatorModel>(),
                factory.Create<TaskIndicatorModel>(),
                factory.Create<TaskTodayIndicatorModel>(),
                factory.Create<PeriodIndicatorModel>()
            };

            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _worker.ProgressChanged += _worker_ProgressChanged;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();
        }

        private async Task SwitchIndicators()
        {
            foreach (var indicator in _indicators)
                await indicator.Switch();
            BroadcastTime();
        }

        private void _worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                _worker.ReportProgress(0);
                Thread.Sleep(50);
            }
        }

        private void _worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (_activityService.IsStarted)
                BroadcastTime();
        }

        private void BroadcastTime()
        {
            foreach (var indicator in _indicators)
                IndicatorTimeChanged?.Invoke(indicator.Name, indicator.Time);
        }
    }
}
