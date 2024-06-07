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
    public class TodayIndicatorModel : BaseIndicatorModel
    {
        private readonly IRepository<Period> _periodsRepository;
        private readonly IActivityService _activityService;
        public override string Name => "Today";

        public TodayIndicatorModel(IRepository<Period> periodsRepository, IActivityService activityService)
        {
            _periodsRepository = periodsRepository;
            _activityService = activityService;
        }

        protected override async Task<(TimeSpan StoredTime, DateTime? PeriodStartTime)> CalculateTime()
        {
            var utcTodayStart = TimeZoneInfo.ConvertTimeToUtc(DateTime.Today);
            var periods = await _periodsRepository.GetAllAsync(q => q.Where(p => p.End > utcTodayStart));

            var storedTime = periods.Aggregate(TimeSpan.Zero, (sum, p) => sum + (p.End - Last(p.Start, utcTodayStart)));

            var periodStartTime = _activityService.SelectedPeriod is null
                ? (DateTime?)null
                : Last(_activityService.SelectedPeriod.Start, utcTodayStart);

            return (StoredTime: storedTime, PeriodStartTime: periodStartTime);
        }
    }
}
