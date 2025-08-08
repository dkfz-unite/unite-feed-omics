using System.Reflection;
using System.Runtime.Serialization;

namespace Unite.Omics.Feed.Web.Models.Base.Binders;

public static class EnumBinder
{
    public static T? Bind<T>(string value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var type = typeof(T);
        
        foreach (var field in type.GetFields())
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();

            if (attribute != null && attribute.Value.Equals(value))
                return (T)field.GetValue(null);

            if (field.Name.Equals(value))
                return (T)field.GetValue(null);
        }

        return null;
    }
}
