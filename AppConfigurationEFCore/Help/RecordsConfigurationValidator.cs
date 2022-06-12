using AppConfigurationEFCore.Configuration;
using System.Reflection;

namespace AppConfigurationEFCore.Help
{
    internal interface IRecordsConfigurationValidator
    {
        bool IsPropertyValid(PropertyInfo property);
        bool IsPropertyValidGroup(PropertyInfo property, IEnumerable<Type> forbiddenTypes);
    }

    internal class RecordsConfigurationValidator : IRecordsConfigurationValidator
    {
        public bool IsPropertyValid(PropertyInfo property) =>
            IsAssignableTo(property.PropertyType, typeof(RecordHandler<>))
            && property.GetCustomAttribute<RecordKeyAttribute>() is not null;

        public bool IsPropertyValidGroup(PropertyInfo property, IEnumerable<Type> forbiddenTypes)
        {
            if (forbiddenTypes.Contains(property.PropertyType))
                return false;

            bool hasAttribute = property.GetCustomAttribute<RecordGroupAttribute>() is not null
                || property.PropertyType.GetCustomAttribute<RecordGroupAttribute>() is not null;

            return hasAttribute;
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
