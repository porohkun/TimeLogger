using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.Misc;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Services
{
    /// <inheritdoc/>
    [AsSingletone(typeof(IActivityService))]
    public class ActivityService : IActivityService
    {
        private readonly IRepository<Activity> _activityRepository;
        private readonly IRepository<Period> _periodsRepository;
        private readonly IRepository<Tag> _tagsRepository;
        private readonly IValuesService _valuesService;

        private Period? _selectedPeriod;

        public Activity? SelectedActivity { get; private set; }

        public Period? SelectedPeriod
        {
            get => _selectedPeriod;
            private set
            {
                _selectedPeriod = value;
                IsStartedChanged?.Invoke(IsStarted);
            }
        }
        public bool IsStarted => SelectedPeriod != null;

        public event Action? ActivitySelected;
        public event Action<long>? ActivityUpdated;
        public event Action<bool>? IsStartedChanged;

        public ActivityService(
            IRepository<Activity> activityRepository,
            IRepository<Period> periodsRepository,
            IRepository<Tag> tagsRepository,
            IValuesService valuesService)
        {
            _activityRepository = activityRepository;
            _periodsRepository = periodsRepository;
            _tagsRepository = tagsRepository;
            _valuesService = valuesService;

            Task.Run(async () =>
            {
                var selectedActivityId = await _valuesService.Get<long>(Values.SelectedActivity);
                await SelectActivity(selectedActivityId);

                // все начатые периоды
                var periods = (await _periodsRepository
                   .GetAllAsync(q => q
                       .Where(Period.IsActivePredicate)
                       .OrderByDescending(p => p.Start)
                       .IgnoreAutoIncludes()))
                   .ToArray();

                // убрать период текущей активности
                if (periods.Any())
                {
                    if (SelectedActivity is not null)
                    {
                        SelectedPeriod = periods.FirstOrDefault(p => p.OwnerId == selectedActivityId);
                        periods = periods
                            .Except(new[] { SelectedPeriod })
                            .Select(p => p!)
                            .ToArray();
                    }
                }

                // завершить все остальные периоды
                if (periods.Any())
                {
                    foreach (var period in periods)
                    {
                        period.End = period.Start;
                        await _periodsRepository.UpdateAsync(period, false);
                    }
                    await _periodsRepository.CommitAsync();
                }
            });
        }

        public async Task<long> CreateNewActivity(string key, string name, IEnumerable<string> tags)
        {
            var id = await _activityRepository.AddAsync(new Activity
            {
                Key = key,
                Name = name,
            });
            return id;
        }

        public async Task<long> UpdateActivity(long id, string key, string name, IEnumerable<string> tags)
        {
            var activity = await _activityRepository.GetByIdAsync(id);
            if (activity == null)
                return 0;
            activity.Key = key;
            activity.Name = name;
            await _activityRepository.UpdateAsync(activity);
            ActivityUpdated?.Invoke(id);
            return id;
        }

        public async Task SelectActivity(long id)
        {
            if (IsStarted)
                await StopActivity(SelectedActivity!.Id);

            var activity = await _activityRepository.GetByIdAsync(id, q => q.IgnoreAutoIncludes());
            await _valuesService.Set(Values.SelectedActivity, activity?.Id ?? 0);
            SelectedActivity = activity;
            ActivitySelected?.Invoke();
        }

        public async Task StartActivity(long id)
        {
            if (IsStarted)
                await StopActivity(SelectedActivity!.Id);

            if (await _activityRepository.GetByIdAsync(id, q => q.IgnoreAutoIncludes()) is null)
                return;

            var period = new Period
            {
                OwnerId = id,
                Start = DateTime.UtcNow,
            };
            await _periodsRepository.AddAsync(period);

            SelectedPeriod = period;
        }

        public async Task StopActivity(long id)
        {
            if (await _activityRepository.GetByIdAsync(id, q => q.IgnoreAutoIncludes()) is null)
                return;

            var periods = await _periodsRepository.GetAllAsync(q => q
                .Where(p => p.OwnerId == id)
                .Where(Period.IsActivePredicate)
                .OrderBy(p => p.Start));
            Period? period = null;
            foreach (var per in periods)
            {
                if (period is null)
                    period = per;
                else
                {
                    period.End = per.Start;
                    await _periodsRepository.UpdateAsync(period, false);
                }
            }

            if (period is not null)
            {
                period.End = DateTime.UtcNow;
                await _periodsRepository.UpdateAsync(period);
            }

            SelectedPeriod = null;
        }

        public async Task ArchiveActivity(long id, bool archive)
        {
            if (IsStarted && SelectedPeriod!.OwnerId == id)
                await StopActivity(id);

            var activity = await _activityRepository.GetByIdAsync(id);
            if (activity == null)
                return;
            activity.Archived = archive;
            await _activityRepository.UpdateAsync(activity);
            ActivityUpdated?.Invoke(id);
        }
    }
}
