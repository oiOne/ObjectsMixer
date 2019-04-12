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
            var defaultSettings = new MixerSettings().WithLeftPriority();
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

            var forComparisonDescr = leftDescriptors.Intersect<PropertyDescriptor>(rightDescriptors);

            var diffLeftDescr = leftDescriptors.Except<PropertyDescriptor>(rightDescriptors);
            var diffRightDescr = rightDescriptors.Except<PropertyDescriptor>(leftDescriptors);

            var resultSet = new Dictionary<string, object>();
            foreach (var propertyDescriptor in diffLeftDescr)
            {
                resultSet.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(left));
            }
            foreach (var propertyDescriptor in diffRightDescr)
            {
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
           
            return resultSet;
        }

        private static void PopulateComparedResultSetWithPriority(Dictionary<string, object> resultSet,
            IEnumerable<PropertyDescriptor> forComparisonDescr, object priorObj)
        {
            foreach (var propertyDescriptor in forComparisonDescr)
            {
                resultSet.Add(propertyDescriptor.Name, propertyDescriptor.GetValue(priorObj));
            }
        }
        
    }
}