using System;
using System.Threading.Tasks;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;

namespace TimeLogger.Models
{
    [AsTransient]
    public class PeriodIndicatorModel : BaseIndicatorModel
    {
        private readonly IActivityService _activityService;

        public override string Name => "Period";

        public PeriodIndicatorModel(IActivityService activityService)
        {
            _activityService = activityService;
        }

        protected override Task<(TimeSpan StoredTime, DateTime? PeriodStartTime)> CalculateTime()
        {
            var result = _activityService.SelectedPeriod is null
                ? (StoredTime: TimeSpan.Zero, PeriodStartTime: (DateTime?)null)
                : (StoredTime: TimeSpan.Zero, PeriodStartTime: _activityService.SelectedPeriod.Start);

            return Task.FromResult(result);
        }
    }
}
