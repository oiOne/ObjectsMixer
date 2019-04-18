using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

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

        private Dictionary<string, object> GetPropertiesResultSet(object left, object right, MixerSettings settings)
        {
            var leftDescriptors = GetPropDescriptorsArray(left);
            var rightDescriptors = GetPropDescriptorsArray(right);
            PropertyComparer<PropertyDescriptor> nameComparer = new PropertyComparer<PropertyDescriptor>(x => x.Name);

            var forComparisonDescr = leftDescriptors.Intersect<PropertyDescriptor>(rightDescriptors, nameComparer);

            var diffLeftDescr = leftDescriptors.Except<PropertyDescriptor>(rightDescriptors, nameComparer);

            var diffRightDescr = rightDescriptors.Except<PropertyDescriptor>(leftDescriptors, nameComparer);

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
                // case we have class as prop value [nested constuction]
                // simple merge objects props
                var propValueObj = GetNotEmptyPropValueObject(propertyDescriptor);
                if (propValueObj == null)
                {
                    resultSet.Add(propertyDescriptor.Name, propValueObj);
                }
                else if (propValueObj.GetType().GetInterfaces()
                    .Any(t => t.IsGenericType&& t.GetGenericTypeDefinition() == typeof(IList<>)))
                {
                    Debug.WriteLine("WE are within the ");

                    var internalList = new List<object>(); // not extracted
                    var leftArray = ((ArrayList) ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left));
                    var rightArray = ((ArrayList) ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left));

                    for (int i = 0; i < leftArray.Count; i++)
                    {
                        var mixedResult = MixObjects(leftArray[i], rightArray[i]);
                        internalList.Add(mixedResult);
                    }
                    resultSet.Add(propertyDescriptor.Name, internalList);
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
                    return leftVal;
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

        private static object ExtractPropValueOfObjectBy(string name, object obj)
        {
            var leftDescriptors = GetPropDescriptorsArray(obj);
            var leftVal = default(object);
            return leftDescriptors.FirstOrDefault(x => x.Name == name).GetValue(obj);
        }

        private static object ExtractPropOfObjectBy(string name, object obj)
        {
            var leftDescriptors = GetPropDescriptorsArray(obj);
            var leftVal = default(object);
            return leftDescriptors.FirstOrDefault(x => x.Name == name);
        }
    }
}