using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectsMixer
{
    public class ObjectsMapperService
    {
        public IEnumerable<string> GetNamesOfPropertiesWhichAreEnumerable<T>(T target) where T : class
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.GetValue(target).GetType().GetInterfaces()
                    .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    yield return prop.Name;
                }
            }
        }

        public IEnumerable<string> GetNamesOfPropertiesWhichAreAnonymous<T>(T target) where T : Dictionary<string, object>
        {
            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();

                if (
                    tType.UnderlyingSystemType.Name.Contains("Dictionary`2") ||
                    tType.UnderlyingSystemType.Name.Contains("AnonymousType"))
                {
                    yield return key;
                }
            }
        }

        public IEnumerable<T> CreateListOfAnonymousTypesFromConfig<T>(Dictionary<string, object> config)
            where T : class
        {
            var anonymousPropNames = GetNamesOfPropertiesWhichAreAnonymous(config);
            foreach (var anonymousPropName in anonymousPropNames)
            {
                dynamic valueFromConfig = config[anonymousPropName];
                var configInner = GetPairsOfPropsAndValues(valueFromConfig);
                var resultType = AssignConfigurationWithObject<T>(configInner);

                yield return resultType;
            }
        }

        public T AssignConfigurationWithComplexObject<T>(Dictionary<string, object> config)
            where T : class
        {
            var target = Activator.CreateInstance<T>();
            var toIgnoreNamesInAssigment = GetNamesOfPropertiesWhichAreAnonymous(config); // "Property3"

            foreach (var prop in target.GetType().GetProperties())
            {
                var propName = GetNameOrDisplayNameOfProperty(prop, target);
                object targetValue = null;
                if (!toIgnoreNamesInAssigment.Contains(propName))
                {
                    targetValue = config[propName];
                }
                else
                {
                    var innerType = target.GetType().GetProperty(prop.Name).PropertyType.GenericTypeArguments[0];
                    targetValue = this.GetType()
                        .GetMethod("CreateListOfAnonymousTypesFromConfig")
                        .MakeGenericMethod(innerType)
                        .Invoke(this, new object[] { config });  // [ { Id: 11, Name: "Property3" } ]
                }

                target.GetType().GetProperty(prop.Name)
                    .SetValue(target, targetValue, null);
            }

            return target;
        }

        public T AssignConfigurationWithObject<T>(Dictionary<string, object> config)
            where T : class
        {
            var target = Activator.CreateInstance<T>();
            foreach (var prop in target.GetType().GetProperties())
            {
                var propName = GetNameOrDisplayNameOfProperty(prop, target);
                var valueFromConfig = config[propName];

                target.GetType().GetProperty(prop.Name)
                    .SetValue(target, valueFromConfig, null);
            }

            return target;
        }

        public T AssignConfigurationWithObject<T>(Dictionary<string, string> config)
            where T : class
        {
            var target = Activator.CreateInstance<T>();
            foreach (var prop in target.GetType().GetProperties())
            {
                var propName = GetNameOrDisplayNameOfProperty(prop, target);
                var valueFromConfig = config[propName];

                target.GetType().GetProperty(prop.Name)
                    .SetValue(target, valueFromConfig, null);
            }

            return target;
        }

        public Dictionary<string, string> GetPairsOfPropsAndValues(object obj)
        {
            var result = new Dictionary<string, string>();

            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                var key = GetNameOrDisplayNameOfProperty(prop, obj);
                var value = GetValueOfProperty(prop, obj);
                result.Add(key, value);
            }

            return result;
        }

        public string GetNameOrDisplayNameOfProperty(PropertyInfo propertyInfo, object obj)
        {
            string keyName = propertyInfo.Name;

            if (Attribute.IsDefined(propertyInfo, typeof(DisplayNameAttribute)))
            {
                var attribute = Attribute.GetCustomAttribute(propertyInfo, typeof(DisplayNameAttribute));
                keyName = ((DisplayNameAttribute)attribute).DisplayName;
            }

            return keyName;
        }

        public string GetValueOfProperty(PropertyInfo propertyInfo, object obj)
        {
            var val = propertyInfo.GetValue(obj);
            var propValue = val == null ? string.Empty : val.ToString();

            return propValue;
        }
    }
}
