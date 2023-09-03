using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLogger.Domain.Common.Contracts;

namespace TimeLogger.Domain.Data
{
    public record Tag : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<Activity>? Activities { get; init; }

        public class Configuration : IEntityTypeConfiguration<Tag>
        {
            public void Configure(EntityTypeBuilder<Tag> builder)
            {
                builder.HasMany(t => t.Activities)
                    .WithMany(a => a.Tags);
            }
        }

        [Flags]
        public enum Fill
        {
            None = 0b0,
            Activities = 0b1,
        }
    }
}
