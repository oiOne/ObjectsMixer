using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace ObjectsMixer
{
    public class MixerSettings
    {
        private IList<Tuple<string, string>> _ignoredProperties;
        internal IList<Tuple<string, string>> IgnoredProperties => _ignoredProperties ?? (_ignoredProperties = new List<Tuple<string, string>>());
        private Priority _priority = Priority.Merge;
        public Priority Priority => _priority;

        public MixerSettings WithLeftPriority()
        {
            _priority = Priority.Left;
            return this;
        }
        public MixerSettings WithRightPriority()
        {
            _priority = Priority.Right;
            return this;
        }
        public ExpandoObject Mix(object left, object right)
        {
            return new Mixer().MixObjects(left, right, this);
        }
        public MixerSettings Ignore(Expression<Func<object>> ignoreProperty)
        {
            IgnoredProperties.Add(GetObjectTypeAndProperty(ignoreProperty));
            return this;
        }
        private Tuple<string, string> GetObjectTypeAndProperty(Expression<Func<object>> property)
        {

            var objType = string.Empty;
            var propName = string.Empty;

            try
            {
                if (property.Body is MemberExpression)
                {
                    objType = ((MemberExpression)property.Body).Member.ReflectedType.UnderlyingSystemType.Name;
                    propName = ((MemberExpression)property.Body).Member.Name;
                }
                else if (property.Body is UnaryExpression)
                {
                    objType = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.ReflectedType.UnderlyingSystemType.Name;
                    propName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
                }
                else
                {
                    throw new Exception("Expression type unknown.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong during ignoring particular property:", ex);
            }

            return new Tuple<string, string>(objType, propName);
        }
    }
}