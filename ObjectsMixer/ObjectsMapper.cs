using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ObjectsMixer
{
    public class ObjectsMapper
    {
        private Dictionary<string, PropertyInfo> _propertyMap;

        private void InitMapperType<T>() where T : class
        {
            _propertyMap =
                typeof(T)
                    .GetProperties()
                    .ToDictionary(
                        p => p.Name.ToLower(),
                        p => p
                    );
        }

        public void Map<T>(ExpandoObject source, T destination) where T : class
        {
            InitMapperType<T>();
            InputGuard(source, destination);

            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    var propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (propType.Name.Equals("String"))
                        {
                            p.SetValue(destination, string.Empty, null);
                        }
                        else if (!propType.Name.Equals("Nullable`1"))
                        {
                            throw new ArgumentException("Not Nullable");
                        }
                    }
                    else if (kv.Value.GetType() != propType)
                    {
                        if (propType.FullName.Equals(typeof(Guid).FullName))
                        {
                            if (Guid.TryParse(kv.Value.ToString(), out Guid parsedGuid))
                                p.SetValue(destination, parsedGuid, null);
                            else
                                p.SetValue(destination, Guid.Empty, null);
                            continue;
                        }

                        throw new ArgumentException($"Type mismatch: {propType.FullName} and {kv.Value.GetType()}");
                    }
                    p.SetValue(destination, kv.Value, null);
                }
            }
        }

        /// <summary>
        /// Map into Type that has some nested items set of U type.
        /// </summary>
        /// <typeparam name="T">Main object type</typeparam>
        /// <typeparam name="U">Type argument in collection property</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        public void Map<T, U>(ExpandoObject source, T destination)
            where T : class
            where U : class
        {
            InitMapperType<T>();
            InputGuard(source, destination);

            foreach (var kv in source)
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Key.ToLower(), out p))
                {
                    var propType = p.PropertyType;
                    if (kv.Value == null)
                    {
                        if (!propType.IsByRef)
                        {
                            if (propType.Name.Equals("String"))
                            {
                                p.SetValue(destination, string.Empty, null);
                            }
                            else if (!propType.Name.Equals("Nullable`1"))
                            {
                                throw new ArgumentException("Not Nullable");
                            }
                        }

                    }
                    else if (kv.Value.GetType() != propType)
                    {
                        if (propType.FullName.Equals(typeof(Guid).FullName))
                        {
                            if (Guid.TryParse(kv.Value.ToString(), out Guid parsedGuid))
                                p.SetValue(destination, parsedGuid, null);
                            else
                                p.SetValue(destination, Guid.Empty, null);
                            continue;
                        }
                        if (
                            kv.Value.GetType().Name == "List`1" &&
                            propType.Name == "IEnumerable`1")
                        {
                            var nestedType = propType.GenericTypeArguments[0];
                            var listType = typeof(List<>).MakeGenericType(nestedType);
                            var nestedList = Activator.CreateInstance(listType);
                            dynamic nestedItems = kv.Value;
                            for (int i = 0; i < nestedItems.Count; i++)
                            {
                                var nestedItem = (U)Activator.CreateInstance(nestedType);
                                Map<U>((ExpandoObject)nestedItems[i], nestedItem);

                                ((IList)nestedList).Add(nestedItem);
                            }
                            p.SetValue(destination, nestedList, null);
                            InitMapperType<T>();
                            continue;
                        }

                        throw new ArgumentException($"Type mismatch: {propType.FullName} and {kv.Value.GetType()}");
                    }
                    p.SetValue(destination, kv.Value, null);
                }
                else
                {
                    var va = _propertyMap.GetValueOrDefault(kv.Key);
                    Debug.WriteLine($"Value of {kv.Key}: {va}");
                }
            }
        }

        private static void InputGuard(ExpandoObject source, object destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
        }
    }
}
