using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotnetDevkit.Cache.Converters;

internal class CustomContractResolver: DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        if (!prop.Writable && member is PropertyInfo pi)
        {
            var hasSetter = pi.SetMethod ?? pi.GetSetMethod(true);
            if (hasSetter != null) prop.Writable = true;
        }

        return prop;
    }
}
