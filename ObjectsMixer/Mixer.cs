using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ObjectsMixer
{
    public class Mixer
    {
        private object _right;
        private object _left;
        private ExpandoObject _expando;
        private MixerSettings _settings;

        public Mixer()
        {
            CreateResultObject();
        }

        private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        public static MixerSettings WithRightPriority()
        {
            return new MixerSettings().WithRightPriority();
        }
        public static MixerSettings WithLeftPriority()
        {
            return new MixerSettings().WithLeftPriority();
        }
        public static MixerSettings Ignore(Expression<Func<object>> ignoreProperty)
        {
            return new MixerSettings().Ignore(ignoreProperty);
        }
        private void CreateResultObject()
        {
            _expando = new ExpandoObject();
        }

        private static IEnumerable<Object> GetPropsArray(object obj)
        {
            var propDescriptors = TypeDescriptor.GetProperties(obj);
            for (int i = 0; i < propDescriptors.Count; i++)
            {
                yield return propDescriptors[i].GetValue(obj);
            }
        }

        private static IEnumerable<PropertyDescriptor> GetPropDescriptorsArray(object obj)
        {
            var propDescriptors = TypeDescriptor.GetProperties(obj);
            for (int i = 0; i < propDescriptors.Count; i++)
            {
                yield return propDescriptors[i];
            }
        }

        private static Object CreateResultObject(IEnumerable<Object> props)
        {
            var x = new { };
            return Activator.CreateInstance(x.GetType(), props);
        }

        public ExpandoObject MixObjects(object left, object right, MixerSettings settings)
        {
            _left = left;
            _right = right;

            var props = GetPropertiesResultSet(_left, _right, settings);
            foreach (var prop in props)
            {
                AddProperty(_expando, prop.Key, prop.Value);
            }

            return _expando;
        }

        public static ExpandoObject MixObjects(object left, object right)
        {
            return new Mixer().MergeObjects(left, right);
        }

        public ExpandoObject MergeObjects(object left, object right)
        {
            _left = left;
            _right = right;
            var defaultSettings = new MixerSettings();
            var props = GetPropertiesResultSet(_left, _right, defaultSettings);
            foreach (var prop in props)
            {
                AddProperty(_expando, prop.Key, prop.Value);
            }

            return _expando;
        }

        private IEnumerable<PropertyDescriptor> FilterIgnoredProperties(
            IEnumerable<PropertyDescriptor> propertyDescriptors, object source, MixerSettings settings
        )
        {
            foreach (var prop in propertyDescriptors)
            {
                var comparison = new Tuple<string, string>(source.GetType().Name, prop.Name);
                if (!settings.IgnoredProperties.Contains(comparison))
                {
                    yield return prop;
                }
            }
        }

        private Dictionary<string, object> GetPropertiesResultSet(object left, object right, MixerSettings settings)
        {
            var leftDescriptors = GetPropDescriptorsArray(left);
            var rightDescriptors = GetPropDescriptorsArray(right);
            PropertyComparer<PropertyDescriptor> nameComparer = new PropertyComparer<PropertyDescriptor>(x => x.Name);

            var forComparisonDescr = leftDescriptors.Intersect<PropertyDescriptor>(rightDescriptors, nameComparer);

            var diffLeftDescr = leftDescriptors.Except<PropertyDescriptor>(rightDescriptors, nameComparer);

            var diffRightDescr = rightDescriptors.Except<PropertyDescriptor>(leftDescriptors, nameComparer);

            // todo: test ignoring but filter should be earlier than intersect or except operations
            diffLeftDescr = FilterIgnoredProperties(diffLeftDescr, left, settings);
            diffRightDescr = FilterIgnoredProperties(diffRightDescr, right, settings);

            var resultSet = new Dictionary<string, object>();
            foreach (var propertyDescriptor in diffLeftDescr)
            {
                resultSet.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(left));
            }
            foreach (var propertyDescriptor in diffRightDescr)
            {
                if (!resultSet.ContainsKey(propertyDescriptor.Name))
                    resultSet.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(right));
            }


            if (settings.Priority == Priority.Left)
            {
                PopulateComparedResultSetWithPriority(resultSet, forComparisonDescr, _left);
            }
            else if (settings.Priority == Priority.Right)
            {
                forComparisonDescr = rightDescriptors.Intersect<PropertyDescriptor>(leftDescriptors);
                PopulateComparedResultSetWithPriority(resultSet, forComparisonDescr, _right);
            }
            else if (settings.Priority == Priority.Merge)
            {
                PopulateComparedResultSetWithWerge(resultSet, forComparisonDescr);
            }

            return resultSet;
        }

        private static void PopulateComparedResultSetWithPriority(
            Dictionary<string, object> resultSet,
            IEnumerable<PropertyDescriptor> forComparisonDescr,
            object priorObj)
        {
            foreach (var propertyDescriptor in forComparisonDescr)
            {
                resultSet.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(priorObj));
            }
        }
        private void PopulateComparedResultSetWithWerge(
            Dictionary<string, object> resultSet,
            IEnumerable<PropertyDescriptor> forComparisonDescr)
        {
            foreach (var propertyDescriptor in forComparisonDescr)
            {
                var propValueObj = GetNotEmptyPropValueObject(propertyDescriptor);
                if (propValueObj == null)
                {
                    resultSet.Add(propertyDescriptor.Name, propValueObj);
                }
                else if (propValueObj.GetType().GetInterfaces()
                    .Any(t => t.IsGenericType&& t.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    resultSet.Add(propertyDescriptor.Name, propValueObj);
                }
                else if (!propValueObj.GetType().IsPrimitive &&
                    propValueObj.GetType().UnderlyingSystemType.Name.Contains("Anonymous"))
                {
                    var nestedObj = Mixer.MixObjects(
                        ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left),
                        ExtractPropValueOfObjectBy(propertyDescriptor.Name, _right)
                        );

                    resultSet.Add(propertyDescriptor.Name, nestedObj);
                }
                else
                {
                    resultSet.Add(propertyDescriptor.Name, propValueObj);
                }

            }
        }

        private object GetNotEmptyPropValueObject(PropertyDescriptor propertyDescriptor)
        {
            object result = null;
            try
            {
                var leftVal = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left);
                var rightVal = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _right);

                bool leftIsEmpty = leftVal == null || leftVal.ToString() == string.Empty;
                bool rightIsEmpty = rightVal == null || rightVal.ToString() == string.Empty;

                if ((leftIsEmpty && rightIsEmpty) || (!leftIsEmpty && !rightIsEmpty))
                {

                    if (!leftIsEmpty && !rightIsEmpty && IsFormula(leftVal.ToString()))
                        return leftVal;
                    if (!rightIsEmpty && !leftIsEmpty && IsFormula(rightVal.ToString()))
                        return rightVal;
                    if (leftVal.GetType().GetInterfaces()
                        .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>)))
                    {
                        var internalList = new List<object>();
                        dynamic leftObj = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left);
                        dynamic rightObj = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _right);
                        var leftArray = Enumerable.ToList(leftObj);
                        var rightArray = Enumerable.ToList(rightObj);

                        for (int i = 0; i < leftArray.Count; i++)
                        {
                            dynamic mixedObject = Mixer.MixObjects(leftArray[i], rightArray[i]);
                            internalList.Add(mixedObject);
                        }

                        return internalList;
                    }
                    return leftVal;
                }

                if (leftIsEmpty)
                    return rightVal;
                if (rightIsEmpty)
                    return leftVal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return result;
        }

        private static bool IsFormula(string input)
        {
            return Regex.IsMatch(input, @"(\{.+\})");
        }

        private static object ExtractPropValueOfObjectBy(string name, object obj)
        {
            var objDescriptors = GetPropDescriptorsArray(obj);
            return objDescriptors.FirstOrDefault(x => x.Name == name).GetValue(obj);
        }

        private static object ExtractPropOfObjectBy(string name, object obj)
        {
            var objDescriptors = GetPropDescriptorsArray(obj);
            return objDescriptors.FirstOrDefault(x => x.Name == name);
        }
        
    }
}