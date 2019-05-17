using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectsMixer
{
    public class MapperSettings
    {
        private IList<Tuple<string, string>> _ignoredProperties;
        internal IList<Tuple<string, string>> IgnoredProperties => _ignoredProperties ?? (_ignoredProperties = new List<Tuple<string, string>>());

        public MapperSettings Ignore(Expression<Func<object>> ignoreProperty)
        {
            IgnoredProperties.Add(GetObjectTypeAndProperty(ignoreProperty));
            return this;
        }
        //public T MapInto<T>(object source)
        //{
        //    return ObjectsMapper.MapInto<T>(source, this);
        //}

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
