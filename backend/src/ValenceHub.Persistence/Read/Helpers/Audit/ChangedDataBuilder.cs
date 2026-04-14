using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;

namespace ValenceHub.Persistence.Read.Helpers.Audit;

public static class ChangedDataBuilder
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

    public static Dictionary<string, object?> BuildDiff(
        object? oldObject,
        object newObject)
    {
        var result = new Dictionary<string, object?>();

        var type = newObject.GetType();

        var properties = Cache.GetOrAdd(type, t =>
            t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        foreach (var prop in properties)
        {
            if (prop.IsDefined(typeof(IgnoreAuditAttribute), false))
                continue;

            var newValue = prop.GetValue(newObject);
            var oldValue = oldObject != null
                ? prop.GetValue(oldObject)
                : null;

            if (!Equals(oldValue, newValue))
            {
                result[prop.Name] = new
                {
                    Old = oldValue,
                    New = newValue
                };
            }
        }

        return result;
    }
}
