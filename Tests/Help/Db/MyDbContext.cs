using Microsoft.EntityFrameworkCore;

namespace Tests.Help.Db
{
    internal class MyDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options) { }

        protected DbSet<AppConfigurationEFCore.Entities.AppConfig> _Config { get; set; } = null!;
    }
}
