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
        private ObjectsMapperService _mapperSvc;

        private void InitMapperType<T>() where T : class
        {
            if (_mapperSvc == null)
                _mapperSvc = new ObjectsMapperService();
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

        public void MapInto<T>(dynamic source, T destination) where T : class
        {
            InitMapperType<T>();
            //InputGuard(source, destination);

            IEnumerable<string> ignoreNames = _mapperSvc.GetNamesOfPropertiesWhichAreAnonymous(source);
            var ignoreNamesList = ignoreNames?.ToList();

            foreach (var kv in source.GetType().GetProperties())
            {
                PropertyInfo p;
                if (_propertyMap.TryGetValue(kv.Name.ToLower(), out p))
                {
                    var propName = _mapperSvc.GetNameOrDisplayNameOfProperty(p, source);

                    if (!ignoreNamesList.Contains(propName))
                    {
                        if (kv.PropertyType != p.PropertyType)
                        {
                            // todo: set default for type if we can't parse and set value
                            throw new Exception("Type mismatch");
                        }
                        else
                        {
                            var targetVal = Convert.ChangeType(kv.GetValue(source, null), p.PropertyType);
                            p.SetValue(destination, targetVal, null);
                        }
                        
                    }
                    else
                    {
                        var propType = p.PropertyType;
                        var val = kv.GetValue(source, null);

                        dynamic targetValue = Activator.CreateInstance(propType);
                        this.MapInto(val, targetValue);

                        p.SetValue(destination, targetValue, null);
                    }

                    // todo: convert code below into safe initialization
                    //if (kv.Value == null)
                    //{
                    //    if (propType.Name.Equals("String"))
                    //    {
                    //        p.SetValue(destination, string.Empty, null);
                    //    }
                    //    else if (!propType.Name.Equals("Nullable`1"))
                    //    {
                    //        throw new ArgumentException("Not Nullable");
                    //    }
                    //}
                    // if (kv.GetType() != propType)
                    //{
                    //    if (propType.FullName.Equals(typeof(Guid).FullName))
                    //    {
                    //        if (Guid.TryParse(kv.GetValue(source, null).ToString(), out Guid parsedGuid))
                    //            p.SetValue(destination, parsedGuid, null);
                    //        else
                    //            p.SetValue(destination, Guid.Empty, null);
                    //        continue;
                    //    }

                    //    throw new ArgumentException($"Type mismatch: {propType.FullName} and {kv.Value.GetType()}");
                    //}
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
        /*
            * convert Anonymous => object
            * convert Dictionary`2 as Dictioanry<string, object>
            * convert IList`1 каких-то объектов типа T 
         */

        public static T MapInto<T>(dynamic resource) where T : class
        {
            var target = Activator.CreateInstance<T>();
            new ObjectsMapper().MapInto<T>(resource, target);
            return target;
        }

        public static T MapInto<T>(Dictionary<string, object> config) where T : class
        {
            var target = Activator.CreateInstance<T>();
            new ObjectsMapper().MapInto<T>(config, target);
            return target;
        }

        public void MapInto<T>(Dictionary<string, object> config, T destination) where T : class
        {
            if (_mapperSvc == null)
                _mapperSvc = new ObjectsMapperService();

            var target = destination;

            var ignoreNames = _mapperSvc.GetNamesOfPropertiesWhichAreAnonymous(config);
            var ignoreEnumPropNames = _mapperSvc.GetNamesOfPropertiesWhichAreList(config);

            foreach (var prop in target.GetType().GetProperties())
            {
                var propName = _mapperSvc.GetNameOrDisplayNameOfProperty(prop, target);
                object targetValue = null;
                if (!ignoreNames.Contains(propName) && !ignoreEnumPropNames.Contains(propName))
                {
                    targetValue = config[propName];
                }
                else
                {
                    if (ignoreEnumPropNames.Any())
                    {
                        var innerType = target.GetType().GetProperty(prop.Name).PropertyType.GenericTypeArguments[0];
                        var innerArray = config[propName];

                        targetValue = this.GetType()
                            .GetMethod("CreateListOfAnonymousTypesFromConfig")
                            .MakeGenericMethod(innerType)
                            .Invoke(this, new object[] { innerArray });

                    }

                    if (ignoreNames.Any())
                    {
                        var currentProp = target.GetType().GetProperty(prop.Name);
                        var currentPropValue = config[propName];

                        dynamic result = Activator.CreateInstance(currentProp.PropertyType);
                        this.MapInto(currentPropValue, result);
                        targetValue = result;
                    }
                    // todo: dictionary case if type has such type of prop
                }

                target.GetType().GetProperty(prop.Name)
                    .SetValue(target, targetValue, null);
            }
        }

        // using in 267
        public IEnumerable<T> CreateListOfAnonymousTypesFromConfig<T>(dynamic resource)
            where T : class
        {
            foreach (var item in resource)
            {
                var mappedItem = Activator.CreateInstance<T>();
                this.MapInto(item, mappedItem);

                yield return mappedItem;
            }
        }

    }
}
