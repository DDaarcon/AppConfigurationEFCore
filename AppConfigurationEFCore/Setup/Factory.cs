using AppConfigurationEFCore.Configuration;
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
        private static TDbContext? _context;

        private TRecords? _records;


        public Factory(
            IRecordHandlerFactory handlerFactory)
        {
            _handlerFactory = handlerFactory;
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
                if (IsPropertyValid(property))
                    property.SetValue(_records, CreateRecordOperations(property));
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

        private bool IsPropertyValid(PropertyInfo property)
        {
            return IsAssignableTo(property.PropertyType, typeof(RecordHandler<>))
                && property.GetCustomAttribute<RecordKeyAttribute>() is not null;
        }

        private bool IsAssignableTo(Type givenType, Type genericType)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type? baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableTo(baseType, genericType);
        }
    }
}
