using System;
using System.Linq;
using System.Threading.Tasks;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Models
{
    [AsTransient]
    public class TaskTodayIndicatorModel : BaseIndicatorModel
    {
        private readonly IRepository<Period> _periodsRepository;
        private readonly IActivityService _activityService;

        public override string Name => "Task today";

        public TaskTodayIndicatorModel(IRepository<Period> periodsRepository, IActivityService activityService)
        {
            _periodsRepository = periodsRepository;
            _activityService = activityService;
        }

        protected override async Task<(TimeSpan StoredTime, DateTime? PeriodStartTime)> CalculateTime()
        {
            if (_activityService.SelectedActivity == null)
                return (StoredTime: TimeSpan.Zero, PeriodStartTime: null);

            var utcTodayStart = TimeZoneInfo.ConvertTimeToUtc(DateTime.Today);
            var periods = await _periodsRepository.GetAllAsync(q => q.Where(p => p.OwnerId == _activityService.SelectedActivity.Id && p.End > utcTodayStart));

            var storedTime = periods
                .Where(p => !p.IsActive)
                .Aggregate(TimeSpan.Zero, (sum, p) => sum + (p.End - Last(p.Start, utcTodayStart)));

            var periodStartTime = _activityService.SelectedPeriod is null
                ? (DateTime?)null
                : Last(_activityService.SelectedPeriod.Start, utcTodayStart);

            return (StoredTime: storedTime, PeriodStartTime: periodStartTime);
        }
    }
}
