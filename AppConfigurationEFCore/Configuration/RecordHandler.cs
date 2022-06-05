using AppConfigurationEFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppConfigurationEFCore.Configuration
{
    public class VTRecordHandler<T> : RecordHandler<T?>
        where T : struct
    {
        public VTRecordHandler(string key, Func<DbContext> getContext, Func<string?, T?> toType, Func<T?, string?> fromType) : base(key, getContext, toType, fromType)
        {
        }
    }

    public class RecordHandler<T>
    {
        readonly string _key;
        readonly Func<DbContext> _getContext;
        readonly Func<string?, T?> _toType;
        readonly Func<T?, string?> _fromType;

        DbContext _Context => _getContext();
        DbSet<AppConfig> _Repo => _Context.Set<AppConfig>();


        public RecordHandler(
            string key,
            Func<DbContext> getContext,
            Func<string?, T?> toType,
            Func<T?, string?> fromType)
        {
            _key = key;
            _getContext = getContext;
            _toType = toType;
            _fromType = fromType;
        }

        /// <summary>
        /// Get value from database.
        /// </summary>
        public T? Get() => _toType(_Repo.FirstOrDefault(x => x.Key == _key)?.Value);

        /// <summary>
        /// Get value from database.
        /// </summary>
        public async Task<T?> GetAsync(CancellationToken cancellationToken = default) =>
            _toType((await _Repo.FirstOrDefaultAsync(x => x.Key == _key, cancellationToken))?.Value);

        /// <summary>
        /// Set database's entry to <paramref name="value"/>.
        /// </summary>
        public void Set(T? value)
        {
            var entry = _Repo.FirstOrDefault(x => x.Key == _key);
            entry ??= Add();

            entry.Value = _fromType(value);
        }

        /// <summary>
        /// Set database's entry to <paramref name="value"/>.
        /// </summary>
        public async Task SetAsync(T? value, CancellationToken cancellationToken = default)
        {
            var entry = await _Repo.FirstOrDefaultAsync(x => x.Key == _key, cancellationToken);
            entry ??= Add();

            entry.Value = _fromType(value);
        }

        /// <summary>
        /// Set database's entry to <paramref name="value"/> and apply change (call <see cref="DbContext.SaveChanges"/>).
        /// </summary>
        public void SetAndSave(T? value)
        {
            Set(value);
            _Context.SaveChanges();
        }

        /// <summary>
        /// Set database's entry to <paramref name="value"/> and apply change (call <see cref="DbContext.SaveChangesAsync"/>).
        /// </summary>
        public async Task SetAndSaveAsync(T? value, CancellationToken cancellationToken = default)
        {
            await SetAsync(value, cancellationToken);
            await _Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Set database's entry to <paramref name="value"/> if hasn't been set yet
        /// </summary>
        public bool SetIfEmpty(T? value)
        {
            var entry = _Repo.FirstOrDefault(x => x.Key == _key);
            if (!String.IsNullOrEmpty(entry?.Value))
                return false;

            Set(value);
            return true;
        }

        /// <summary>
        /// Set database's entry to <paramref name="value"/> if hasn't been set yet
        /// </summary>
        public async Task<bool> SetIfEmptyAsync(T? value, CancellationToken cancellationToken = default)
        {
            var entry = await _Repo.FirstOrDefaultAsync(x => x.Key == _key, cancellationToken);
            if (!String.IsNullOrEmpty(entry?.Value))
                return false;

            await SetAsync(value, cancellationToken);
            return true;
        }

        private AppConfig Add()
        {
            var entry = new AppConfig
            {
                Key = _key
            };
            _Repo.Add(entry);
            return entry;
        }
    }
}
