using AppConfigurationEFCore.Configuration;
using AppConfigurationEFCore.Setup;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AppConfigurationEFCore.Help
{
    internal class RecordsPropertiesSetter<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IRecordsConfigurationValidator _configurationValidator;
        private readonly IRecordHandlerFactory _handlerFactory;
        private readonly Func<TDbContext> _getContext;
        private IEnumerable<Type> _forbiddenGroupTypes;

        private object _records = null!;
        private string? _keyPrefix;

        public RecordsPropertiesSetter(
            IRecordsConfigurationValidator configurationValidator,
            IRecordHandlerFactory handlerFactory,
            Func<TDbContext> getContext,
            IEnumerable<Type>? forbiddenGroupTypes = null)
        {
            _configurationValidator = configurationValidator;
            _handlerFactory = handlerFactory;
            _getContext = getContext;
            _forbiddenGroupTypes = forbiddenGroupTypes ?? new Type[0];
        }

        public void SetUp(object records, string? keyPrefix = null)
        {
            _records = records;
            _keyPrefix = keyPrefix;

            _forbiddenGroupTypes = _forbiddenGroupTypes.Append(_records.GetType());

            var properties = _records.GetType().GetProperties()!;

            foreach (var property in properties)
            {
                if (_configurationValidator.IsPropertyValid(property))
                    property.SetValue(_records, CreateRecordOperations(property));

                else if (_configurationValidator.IsPropertyValidGroup(property, _forbiddenGroupTypes))
                    property.SetValue(_records, CreateRecordsGroup(property));

                else
                    throw new FormatException(
                        $"Incorrectly formed TRecords class. Property {property.Name} is either of invalid type (valid are RecordHandler<> and VTRecordHandler<>) or missing RecordKeyAttribute. " +
                        $"It may also be incorrect or looping Group Record.");
            }
        }

        private object CreateRecordOperations(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<RecordKeyAttribute>()!;
            var genericType = property.PropertyType.GenericTypeArguments[0];

            string key = $"{_keyPrefix}{attr.Key}";

            object? handler = null;
            if (genericType.IsValueType)
                handler = _handlerFactory.GetVT(genericType, key, _getContext);
            else
                handler = _handlerFactory.Get(genericType, key, _getContext);

            if (handler is null)
                throw new ArgumentException($"Missing type handler. Storing/fetching rules for type {genericType.Name} has not been specified. To specify it use `customRecordTypesAction` parameter in AddAppConfiguration");
            return handler;
        }

        private object CreateRecordsGroup(PropertyInfo property)
        {
            var constructor = property.PropertyType.GetConstructor(new Type[0]);
            if (constructor is null)
                throw new Exception($"Type {property.PropertyType.Name} must have parameterless constructor");

            var recordsGroup = constructor.Invoke(null);
            if (recordsGroup is null)
                throw new Exception("Invalid constructor");

            // TODO: Should not be created every time
            var setter = new RecordsPropertiesSetter<TDbContext>(_configurationValidator, _handlerFactory, _getContext, _forbiddenGroupTypes);

            setter.SetUp(recordsGroup, GetGroupKeyPrefix(property));

            return recordsGroup;
        }

        private string? GetGroupKeyPrefix(PropertyInfo property)
        {
            var attr = property.GetCustomAttribute<RecordGroupAttribute>()
                ?? property.PropertyType.GetCustomAttribute<RecordGroupAttribute>();

            if (String.IsNullOrEmpty(attr!.GroupKey))
                return null;

            return $"{attr.GroupKey}{attr.Separator}";
        }
    }
}
