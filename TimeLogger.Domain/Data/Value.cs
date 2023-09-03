using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeLogger.Domain.Common.Contracts;

namespace TimeLogger.Domain.Data
{
    public record Value : BaseEntity
    {
        public string? Name { get; set; }
        public string? Caption { get; set; }

        public class Configuration : IEntityTypeConfiguration<Value>
        {
            public void Configure(EntityTypeBuilder<Value> builder)
            {
                builder.HasIndex(v => v.Name).IsUnique();
                builder.Property(v => v.Name).IsRequired();
            }
        }

        [Flags]
        public enum Fill
        {
            None = 0b0
        }
    }
}
