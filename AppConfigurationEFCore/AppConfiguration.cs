using AppConfigurationEFCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppConfigurationEFCore
{
    public interface IAppConfiguration<TRecords>
        where TRecords : class, new()
    {

        /// <summary>
        /// User defined records in type <typeparamref name="TRecords"/>.
        /// </summary>
        TRecords Records { get; }
        /// <summary>
        /// Get handler for custom record, which key is <paramref name="key"/> and value of type <see cref="string"/>.
        /// </summary>
        RecordHandler<string> CustomConfig(string key);

        /// <summary>
        /// Call <see cref="DbContext.SaveChanges"/>.
        /// </summary>
        void Save();
        /// <summary>
        /// Call <see cref="DbContext.SaveChangesAsync"/>.
        /// </summary>
        Task SaveAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Use new <typeparamref name="TDbContext"/> (<see cref="DbContext"/>), requested from <see cref="IServiceScope"/>.
        /// </summary>
        void UseIndependentDbContext();
    }

    internal class AppConfiguration<TDbContext, TRecords> : IAppConfiguration<TRecords>
        where TDbContext : DbContext
        where TRecords : class, new()
    {
        private TDbContext _context;
        private readonly IServiceScopeFactory? _scopeFactory;


        public AppConfiguration(
            TDbContext context,
            IServiceScopeFactory? scopeFactory,
            TRecords records)
        {
            _context = context;
            _scopeFactory = scopeFactory;

            Records = records;
        }

        public TRecords Records { get; init; }

        public RecordHandler<string> CustomConfig(string key) =>
            new RecordHandler<string>(key, () => _context, v => v, v => v);

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void UseIndependentDbContext()
        {
            _context = _scopeFactory?.CreateScope().ServiceProvider.GetRequiredService<TDbContext>() ?? _context;
        }
    }
}
