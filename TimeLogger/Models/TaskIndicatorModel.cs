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
    public class TaskIndicatorModel : BaseIndicatorModel
    {
        private readonly IRepository<Period> _periodsRepository;
        private readonly IActivityService _activityService;
        public override string Name => "Task";

        public TaskIndicatorModel(IRepository<Period> periodsRepository, IActivityService activityService)
        {
            _periodsRepository = periodsRepository;
            _activityService = activityService;
        }

        protected override async Task<(TimeSpan StoredTime, DateTime? PeriodStartTime)> CalculateTime()
        {
            if (_activityService.SelectedActivity == null)
                return (StoredTime: TimeSpan.Zero, PeriodStartTime: null);

            var periods = await _periodsRepository.GetAllAsync(q => q.Where(p => p.OwnerId == _activityService.SelectedActivity.Id));

            var storedTime = periods
                .Where(p => !p.IsActive)
                .Aggregate(TimeSpan.Zero, (sum, p) => sum + (p.End - p.Start));

            var periodStartTime = _activityService.SelectedPeriod?.Start;

            return (StoredTime: storedTime, PeriodStartTime: periodStartTime);
        }
    }
}
