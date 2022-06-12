using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Help;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
            var properties = _records.GetType().GetProperties()!;

            foreach (var property in properties)
            {
                if (_configurationValidator.IsPropertyValid(property))
                    property.SetValue(_records, CreateRecordOperations(property));

                else if (_configurationValidator.IsPropertyValidGroup(property))
                    throw new NotImplementedException();

                else
                    throw new FormatException($"Incorrectly formed TRecords class. Property {property.Name} is either of invalid type (valid are RecordHandler<> and VTRecordHandler<>) or missing RecordKeyAttribute");
            }
        }

        private object CreateRecordOperations(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<RecordKeyAttribute>()!;
            var genericType = property.PropertyType.GenericTypeArguments[0];

            object? handler = null;
            if (genericType.IsValueType)
                handler = _handlerFactory.GetVT(genericType, attr.Key, _getContext);
            else
                handler = _handlerFactory.Get(genericType, attr.Key, _getContext);

            if (handler is null)
                throw new ArgumentException($"Missing type handler. Storing/fetching rules for type {genericType.Name} has not been specified. To specify it use `customRecordTypesAction` parameter in AddAppConfiguration");
            return handler;
        }
    }
}
