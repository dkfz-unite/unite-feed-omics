using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Unite.Mutations.Feed.Data.Repositories.Entities.Extensions
{
    public static class EntityExtensions
    {
        public static T With<T, TValue>(this T target, Expression<Func<T, TValue>> property, TValue value) where T : class
        {
            var memberExpression = property.Body as MemberExpression;
            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(target, value, null);
                }
            }

            return target;
        }
    }
}
