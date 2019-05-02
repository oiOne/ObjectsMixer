using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectsMixer
{
    public class PropertyComparer<T> : IEqualityComparer<T>
    {
        private Expression<Func<T, object>>[] properties;

        public PropertyComparer(Expression<Func<T, object>> property)
        {
            ThrowExceptionIfPropertyIsNull(property);

            this.properties = new Expression<Func<T, object>>[] { property };
        }

        private static void ThrowExceptionIfPropertyIsNull(Expression<Func<T, object>> property)
        {
            if (property == null)
                throw new NullReferenceException("Property expression cannot be null");
        }

        public PropertyComparer(Expression<Func<T, object>>[] properties)
        {
            if (properties.Length == 0)
                throw new ArgumentException("Array must contain at least on property to compare");

            foreach (var property in properties)
                ThrowExceptionIfPropertyIsNull(property);

            this.properties = properties;
        }

        public bool Equals(T x, T y)
        {
            foreach (Expression<Func<T, object>> property in properties)
            {
                if (!PropertyEquals(x, y, property))
                    return false;
            }

            return true;
        }

        private bool PropertyEquals(T x, T y, Expression<Func<T, object>> property)
        {
            object xValue = property.Compile()(x);
            object yValue = property.Compile()(y);

            if (xValue == null)
                return yValue == null;

            return xValue.Equals(yValue);
        }

        public int GetHashCode(T obj)
        {
            if (properties.Length == 1)
                return GetHashCodeForSingleProperty(obj);

            return GetHashCodeForMultipleProperties(obj);
        }

        private int GetHashCodeForSingleProperty(T obj)
        {
            object objValue = this.properties[0].Compile()(obj);

            if (objValue == null)
                return 0;
            else
                return objValue.GetHashCode();
        }

        private int GetHashCodeForMultipleProperties(T obj)
        {
            int hash = 17;

            foreach (Expression<Func<T, object>> property in properties)
            {
                object objValue = property.Compile()(obj);

                if (objValue == null)
                    hash = hash * 31;
                else
                    hash = hash * 31 + objValue.GetHashCode();
            }

            return hash;
        }
    }

}