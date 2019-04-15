using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace ObjectsMixer
{
    public static class ObjectMixer
    {
        
        private static object _right;
        private static object _left;
        private static ExpandoObject _expando;
        private static MixerSettings _settings;

        static ObjectMixer()
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
        private static void CreateResultObject()
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

        public static ExpandoObject MergeObjects(object left, object right, MixerSettings settings)
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

        public static ExpandoObject MergeObjects(object left, object right)
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

        private static Dictionary<string, object> GetPropertiesResultSet(object left, object right, MixerSettings settings)
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
            } else if (settings.Priority == Priority.Merge)
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
        private static void PopulateComparedResultSetWithWerge(
            Dictionary<string, object> resultSet,
            IEnumerable<PropertyDescriptor> forComparisonDescr)
        {
            foreach (var propertyDescriptor in forComparisonDescr)
            {
                var propValueObj = GetNotEmptyPropValueObject(propertyDescriptor);
                resultSet.Add(propertyDescriptor.Name, propValueObj);
            }
        }

        private static object GetNotEmptyPropValueObject(PropertyDescriptor propertyDescriptor)
        {
            object result = null;
            var leftVal = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _left);
            var rightVal = ExtractPropValueOfObjectBy(propertyDescriptor.Name, _right);

            bool leftIsEmpty = leftVal   == null || leftVal.ToString() == string.Empty;
            bool rightIsEmpty = rightVal == null || rightVal.ToString() == string.Empty;

            if ((leftIsEmpty && rightIsEmpty) || (!leftIsEmpty && !rightIsEmpty))
               return leftVal;
            if (leftIsEmpty)
               return rightVal;
            if (rightIsEmpty)
               return leftVal;

            return result;
        }

        private static object ExtractPropValueOfObjectBy(string name, object obj)
        {
            var leftDescriptors = GetPropDescriptorsArray(obj);
            var leftVal = default(object);
            return leftDescriptors.FirstOrDefault(x => x.Name == name).GetValue(obj);
        }
    }
}