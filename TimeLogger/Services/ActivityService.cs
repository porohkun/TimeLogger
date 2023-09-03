﻿using System;
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
        public bool IsActivated => _selectedPeriod != null;

        public event Action? ActivitySelected;
        public event Action<bool>? IsActivatedChanged;

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
                var selectedActivityId = (await _valuesService.Get<long>(Values.SelectedActivity));
                await SelectActivity(selectedActivityId);

                var periods = (await _periodsRepository
                   .GetAllAsync(q => q
                       .Where(p => p.IsActive)
                       .OrderByDescending(p => p.Start)
                       .IgnoreAutoIncludes()))
                   .ToArray();

                if (periods.Any())
                {
                    if (SelectedActivity is not null)
                    {
                        _selectedPeriod = periods.Where(p => p.OwnerId == selectedActivityId).FirstOrDefault();
                        periods = periods.Except(new[] { _selectedPeriod }).Cast<Period>().ToArray();
                    }
                    if (periods.Any())
                    {
                        foreach (var period in periods)
                        {
                            period.End = period.Start;
                            await _periodsRepository.UpdateAsync(period, false);
                        }
                        await _periodsRepository.CommitAsync();
                    }
                }
            });
        }

        public async Task<long> CreateNewActivity(string key, string name, IEnumerable<string> tags)
        {
            var id = await _activityRepository.AddAsync(new Activity()
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
            return id;
        }

        public async Task SelectActivity(long id)
        {
            if (IsActivated)
                await StopActivity(SelectedActivity!.Id);

            var activity = await _activityRepository.GetByIdAsync(id, q => q.IgnoreAutoIncludes());
            SelectedActivity = activity;
            ActivitySelected?.Invoke();
        }

        public async Task StartActivity(long id)
        {

        }

        public async Task StopActivity(long id)
        {

        }
    }
}
