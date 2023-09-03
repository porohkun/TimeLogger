using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace TimeLogger.DataAccess.Data
{
    public class DatabaseInitializer
    {
        private readonly DataContext _context;

        public DatabaseInitializer(DataContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            var connection = _context.Database.GetDbConnection();
            var connectionString = connection.ConnectionString;
            var filePath = new DbConnectionStringBuilder { ConnectionString = connectionString }["Data Source"] as string;
            var path = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(path))
                Directory.CreateDirectory(path);
            _context.Database.Migrate();
        }
    }
}
