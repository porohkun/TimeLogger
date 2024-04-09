using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLogger.Domain.Common.Contracts;

namespace TimeLogger.Domain.Data
{
    public record Period : BaseEntityWithOwner<Activity>
    {
        public DateTime Start { get; set; } = DateTime.UnixEpoch;
        public DateTime End { get; set; } = DateTime.UnixEpoch;
        public bool IsActive => IsActivePredicate.Compile().Invoke(this);

        public static readonly Expression<Func<Period, bool>> IsActivePredicate = p => p.End < p.Start;

        public class Configuration : IEntityTypeConfiguration<Period>
        {
            public void Configure(EntityTypeBuilder<Period> builder)
            {
                builder.HasOne(v => v.Owner)
                    .WithMany(p => p.Periods)
                    .HasForeignKey(v => v.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.Property(p => p.Start)
                    .HasConversion(
                        v => (long)v.Subtract(DateTime.UnixEpoch).TotalSeconds,
                        v => DateTime.UnixEpoch.AddSeconds(v));

                builder.Property(p => p.End)
                    .HasConversion(
                        v => (long)v.Subtract(DateTime.UnixEpoch).TotalSeconds,
                        v => DateTime.UnixEpoch.AddSeconds(v));
            }
        }

        [Flags]
        public enum Fill
        {
            None = 0b0
        }
    }
}
