using System.Collections;
using System.Reflection;
using System.Text;
using EasyCaching.Core.Interceptor;

namespace Icomm.Infrastructure.Caching;

public class CustomEasyCachingKeyGenerator : IEasyCachingKeyGenerator
{
    private static readonly IEasyCachingKeyGenerator def = new DefaultEasyCachingKeyGenerator();

    private const char LinkChar = '#';

    public string GetCacheKey(MethodInfo methodInfo, object[] args, string prefix)
    {
        var methodArguments = args?.Any() == true
            ? args.Select(GenerateCacheKey)
            : new[] { "0" };
        return GenerateCacheKey(methodInfo, prefix, methodArguments);
    }

    public string GetCacheKeyPrefix(MethodInfo methodInfo, string prefix)
    {
        if (!string.IsNullOrWhiteSpace(prefix)) return $"{prefix}{LinkChar}";
        string typeName = null;
        if (methodInfo.DeclaringType != null && methodInfo.DeclaringType.IsGenericType)
        {
            return GetKeyType(methodInfo.DeclaringType);
        }
        else
        {
            typeName = methodInfo.DeclaringType?.FullName;
        }

        var methodName = methodInfo.Name;

        return $"{typeName}{LinkChar}{methodName}{LinkChar}";
    }

    private static string GetKeyType(Type type)
    {
        if (type.IsGenericType)
        {
            return string.Join(LinkChar, type.Name,
                string.Join(LinkChar, type.GetGenericArguments().Select(m => GetKeyType(m))));
        }

        return type.Name;
    }

    private static string GetKeyType(Type[] types)
    {
        return string.Join(LinkChar, types.Select(t => GetKeyType(t)));
    }

    private string GenerateCacheKey(MethodInfo methodInfo, string prefix, IEnumerable<string> parameters)
    {
        var cacheKeyPrefix = GetCacheKeyPrefix(methodInfo, prefix);

        var builder = new StringBuilder();
        builder.Append(cacheKeyPrefix).Append(LinkChar);
        builder.Append(string.Join(LinkChar.ToString(), parameters));
        return builder.ToString();
    }

    public static string GenerateCacheKey(object parameter)
    {
        if (parameter == null) return string.Empty;
        if (parameter is ICachable cachable) return cachable.CacheKey;
        if (parameter is string key) return key;
        if (parameter is DateTime dateTime) return dateTime.ToString("O");
        if (parameter is DateTimeOffset dateTimeOffset) return dateTimeOffset.ToString("O");
        if (parameter is IEnumerable enumerable) return GenerateCacheKey(enumerable.Cast<object>());
        return parameter.ToString();
    }

    private static string GenerateCacheKey(IEnumerable<object> parameter)
    {
        if (parameter == null) return string.Empty;
        return "[" + string.Join(",", parameter) + "]";
    }
}