using AppConfigurationEFCore.Help;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppConfigurationEFCore.Setup
{
    internal class Factory<TDbContext, TRecords>
        where TDbContext : DbContext
        where TRecords : class, new()
    {
        private readonly IRecordHandlerFactory _handlerFactory;
        private readonly IRecordsConfigurationValidator _configurationValidator;
        private static TDbContext? _context;

        private TRecords? _records;


        public Factory(
            IRecordHandlerFactory handlerFactory,
            IRecordsConfigurationValidator configurationValidator)
        {
            _handlerFactory = handlerFactory;
            _configurationValidator = configurationValidator;
        }

        private readonly Func<TDbContext> _getContext = () => _context ?? throw new ArgumentNullException("DbContext has not been provided");

        public AppConfiguration<TDbContext, TRecords> ConstructAppConfiguration(TDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            if (_records is null)
                SetUpRecords();

            return new AppConfiguration<TDbContext, TRecords>(_getContext(), scopeFactory, _records!);
        }

        private void SetUpRecords()
        {
            _records = new TRecords();

            var setter = new RecordsPropertiesSetter<TDbContext>(_configurationValidator, _handlerFactory, _getContext);

            setter.SetUp(_records);
        }
    }
}
