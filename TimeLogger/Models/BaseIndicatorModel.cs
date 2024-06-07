using System;
using System.Threading.Tasks;
using TimeLogger.Domain.Data;

namespace TimeLogger.Models
{
    public abstract class BaseIndicatorModel
    {
        private DateTime? _periodStartTime;
        private TimeSpan _storedTime;

        public abstract string Name { get; }

        public TimeSpan Time => _periodStartTime is null
            ? _storedTime
            : _storedTime + (TimeSpan)(DateTime.UtcNow - _periodStartTime);

        public async Task Switch()
        {
            var time = await CalculateTime();
            _storedTime = time.StoredTime;
            _periodStartTime = time.PeriodStartTime;
        }

        protected abstract Task<(TimeSpan StoredTime, DateTime? PeriodStartTime)> CalculateTime();

        protected DateTime TrimEnd(Period period) => period.IsActive ? DateTime.UtcNow : period.End;
        protected DateTime Last(DateTime a, DateTime b) => a > b ? a : b;
    }
}
