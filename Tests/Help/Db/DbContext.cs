using Microsoft.EntityFrameworkCore;

namespace Tests.Help.Db
{
    internal class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options)
            : base(options) { }

        protected DbSet<AppConfigurationEFCore.Entities.AppConfig> _Config { get; set; } = null!;
    }
}
