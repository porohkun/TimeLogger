using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLogger.Domain.Common.Contracts;

namespace TimeLogger.Domain.Data
{
    public record Period : BaseEntityWithOwner<Activity>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsActive => End < Start;

        public class Configuration : IEntityTypeConfiguration<Period>
        {
            public void Configure(EntityTypeBuilder<Period> builder)
            {
                builder.HasOne(v => v.Owner)
                    .WithMany(p => p.Periods)
                    .HasForeignKey(v => v.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }

        [Flags]
        public enum Fill
        {
            None = 0b0
        }
    }
}
