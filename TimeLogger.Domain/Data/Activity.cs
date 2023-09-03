using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLogger.Domain.Common.Contracts;

namespace TimeLogger.Domain.Data
{
    public record Activity : BaseEntity
    {
        public string? Key { get; set; }
        public string? Name { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public ICollection<Period>? Periods { get; init; }
        public ICollection<Tag>? Tags { get; init; }

        public class Configuration : IEntityTypeConfiguration<Activity>
        {
            public void Configure(EntityTypeBuilder<Activity> builder)
            {
                builder.HasMany(a => a.Tags)
                    .WithMany(t => t.Activities);
            }
        }

        [Flags]
        public enum Fill
        {
            None = 0b0,
            Periods = 0b1,
            Tags = 0b10,
        }
    }
}
