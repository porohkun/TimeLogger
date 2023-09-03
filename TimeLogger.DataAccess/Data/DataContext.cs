using Microsoft.EntityFrameworkCore;
using TimeLogger.Domain.Common.Contracts;
using TimeLogger.Domain.Data;

namespace TimeLogger.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Value> Values { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public async Task<int> ClearSet<T>()
        {
            var entityType = Model.FindEntityType(typeof(T));
            if (entityType == null) throw new ArgumentException($"Type '{typeof(T)}' is not mapped");

            var count = await Database.ExecuteSqlRawAsync($"DELETE FROM {entityType.GetTableName()};");
            await Database.ExecuteSqlRawAsync($"VACUUM;");
            return count;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var applyConfigurationMethod = modelBuilder.GetType().GetMethod(nameof(modelBuilder.ApplyConfiguration));
            if (applyConfigurationMethod == null) throw new Exception("no ApplyConfiguration method found");

            foreach (var type in typeof(BaseEntity).GetInheritors())
            {
                var configurationType = type.GetNestedType("Configuration");
                if (configurationType == null) continue;
                var applyConfigurationGenericMethod = applyConfigurationMethod.MakeGenericMethod(type);
                applyConfigurationGenericMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(configurationType) });
            }
        }
    }
}
