using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

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

        public IEnumerable<PropertyInfo> GetPropertiesWhichAreEnumerable<T>(T target) where T : class
        {

            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.PropertyType.Name.Contains("IEnumerable"))
                {
                    yield return prop;
                }
            }
        }

        public IEnumerable<string> GetNamesOfPropertiesWhichAreAnonymous(dynamic source)
        {
            foreach (var prop in source.GetType().GetProperties())
            {
                var pType = prop.PropertyType;
                if (pType.UnderlyingSystemType.Name.Contains("AnonymousType"))
                {
                    yield return prop.Name;
                }
            }
        }
        public IEnumerable<string> GetNamesOfPropertiesWhichAreJObjects<T>(T target) where T : Dictionary<string, object>
        {
            var result = new List<string>();

            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();

                if (tType.UnderlyingSystemType.Name.Contains("JObject"))
                {
                    //yield return key;
                    result.Add(key);
                }
            }

            return result;
        }
        public IEnumerable<string> GetNamesOfPropertiesWhichAreAnonymous<T>(T target) where T : Dictionary<string, object>
        {
            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();

                if (tType.UnderlyingSystemType.Name.Contains("AnonymousType") && !tType.UnderlyingSystemType.Name.EndsWith("[]"))
                {
                    yield return key;
                }
            }
        }
        public IEnumerable<string> GetNamesOfPropertiesWhichAreDictionary<T>(T target) where T : Dictionary<string, object>
        {
            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();

                if (tType.UnderlyingSystemType.Name.Contains("Dictionary`2"))
                {
                    yield return key;
                }
            }
        }

        public IEnumerable<string> Search4DictionaryOrAnonymous<T>(T target) where T : Dictionary<string, object>
        {
            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();
                var tName = tType.UnderlyingSystemType.Name;
                if (tName.Contains("Dictionary`2") || tName.Contains("AnonymousType"))
                {
                    yield return key;
                }
            }
        }
       
        public IEnumerable<string> GetNamesOfPropertiesWhichAreList<T>(T target) where T : Dictionary<string, object>
        {
            var result = new List<string>();
            foreach (var key in target.Keys)
            {
                var tType = target[key].GetType();

                if (tType.UnderlyingSystemType.Name.Contains("List`1") || tType.UnderlyingSystemType.Name.EndsWith("[]"))
                {
                    result.Add(key);
                    // yield return key;
                }
            }

            return result;
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

                object targetVal = valueFromConfig;

                if (valueFromConfig.GetType() != prop.PropertyType)
                {
                    // todo: set default for type if we can't parse and set value
                    // throw new Exception("Type mismatch");
                    targetVal = Convert.ChangeType(valueFromConfig, prop.PropertyType);
                }

                target.GetType().GetProperty(prop.Name).SetValue(target, targetVal, null);
            }

            return target;
        }

        public IEnumerable<T> CreateListOf<T>(Dictionary<string, object> source) where T : class
        {
            var anonymousKeys = GetNamesOfPropertiesWhichAreJObjects(source);
            foreach (var key in anonymousKeys)
            {
                var converted = JsonConvert.DeserializeObject<ExpandoObject>(source[key].ToString());
                yield return ObjectsMapper.MapInto<T>(converted);
            }
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

        public object GetConvertedValue(object value, Type targetType)
        {
            if (value == null || targetType == null)
            {
                throw new ArgumentNullException($"One of method's parameters is incorrect: 1st \'{value}\'; 2nd \'{targetType}\'.");
            }
            try
            {
                if (targetType == typeof(Guid))
                {
                    if (Guid.TryParse(value?.ToString(), out Guid parsedGuid))
                        return parsedGuid;
                    else
                        return Guid.Empty;
                }

                var changedVal = Convert.ChangeType(value, targetType);
                return changedVal;
            }
            catch
            {
                return GetDefault(targetType);
                // throw new Exception ($"Type conversion error: We can't convert object \'{value?.GetType()}\' into \'{targetType}\'.");
            }
        }

        public object GetDefault(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            if (type == typeof(string))
                return string.Empty;
            return null;
        }
    }
}
