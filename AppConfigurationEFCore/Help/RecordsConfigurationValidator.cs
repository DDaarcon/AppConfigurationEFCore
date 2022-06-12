using AppConfigurationEFCore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppConfigurationEFCore.Help
{
    internal interface IRecordsConfigurationValidator
    {
        bool IsPropertyValid(PropertyInfo property);
        bool IsPropertyValidGroup(PropertyInfo property);
    }

    internal class RecordsConfigurationValidator : IRecordsConfigurationValidator
    {
        public bool IsPropertyValid(PropertyInfo property) => 
            IsAssignableTo(property.PropertyType, typeof(RecordHandler<>))
            && property.GetCustomAttribute<RecordKeyAttribute>() is not null;

        public bool IsPropertyValidGroup(PropertyInfo property)
        {
            var properties = property.PropertyType.GetProperties()!;

            bool propertiesValid = properties.All(x => IsPropertyValid(x) || IsPropertyValidGroup(x));
            if (!propertiesValid)
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
