using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;

using DbContext = Tests.Help.Db.DbContext;

namespace Tests
{
    internal static class TestDatabase
    {
        private const string ConnectionString = @"Server=.\SQLExpress;Database=EFTestSample;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        private static DbContext? _context;
        public static DbContext Context { get { return CreateContext(); } private set { _context = value; } }


        public static DbContext CreateContext(IServiceCollection? services = null)
        {
            InitializeContext(services);

            return _context!;
        }

        private static void InitializeContext(IServiceCollection? services)
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    if (services is not null)
                    {
                        AddContextToServices(services);
                        _databaseInitialized = true;
                        RequestContextFromServices(services);
                    }
                    else
                    {
                        _context = ConstructContext();
                        _databaseInitialized = true;
                    }

                    _context!.Database.EnsureDeleted();
                    _context.Database.EnsureCreated();
                }
            }
        }

        public static void DisposeContext()
        {
            _context?.Database.EnsureDeleted();
            _context?.Database.CloseConnection();
            _context?.Dispose();
            _databaseInitialized = false;
        }

        private static DbContext ConstructContext()
            => new DbContext(
                new DbContextOptionsBuilder<DbContext>()
                    .LogTo(message => Debug.WriteLine(message))
                    .UseSqlServer(ConnectionString)
                    .Options);

        public static async Task ClearDataAsync<TDbEntity>()
            where TDbEntity : class
        {
            if (_context is null) return;

            var set = _context.Set<TDbEntity>();

            set?.RemoveRange(set);

            await _context.SaveChangesAsync();
        }

        public static void StopTrackingEntities() => _context?.ChangeTracker.Clear();

        public static void RequestContextFromServices(IServiceCollection services)
        {
            if (!_databaseInitialized) InitializeContext(services);
            else _context = services.BuildServiceProvider().GetRequiredService<DbContext>();
        }

        private static void AddContextToServices(IServiceCollection services)
        {
            services.AddDbContext<DbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            });
        }
    }
}
