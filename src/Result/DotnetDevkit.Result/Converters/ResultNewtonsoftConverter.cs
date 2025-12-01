using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotnetDevkit.Result.Converters;

internal class ResultNewtonsoftConverter : JsonConverter
{
    private static JsonSerializerSettings _converterSettings = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            DefaultMembersSearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        }
    };

    private static string Resolve(string name, JsonSerializer serializer)
    {
        var contractResolver = serializer.ContractResolver as Newtonsoft.Json.Serialization.DefaultContractResolver;
        return contractResolver?.GetResolvedPropertyName(name) ?? name;
    }

    public override bool CanConvert(Type objectType)
    {
        if (!objectType.IsGenericType) return false;
        var def = objectType.GetGenericTypeDefinition();
        return def == typeof(Result<>) || def == typeof(Result<,>);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
        JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var isSuccessName = Resolve(nameof(Result<object, object>.IsSuccess), serializer);
        var errorName = Resolve(nameof(Result<object, object>.Error), serializer);
        var valueName = Resolve(nameof(Result<object, object>.Value), serializer);

        var isSuccess = token[isSuccessName]?.Value<bool>() ?? false;

        if (objectType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var errorType = objectType.GetGenericArguments()[0];
            var resultGenericType = typeof(Result<>).MakeGenericType(errorType);
            if (isSuccess)
            {
                var successMethod = resultGenericType.GetMethod(nameof(Result<object>.Success));
                if (successMethod is null)
                {
                    throw new InvalidOperationException($"No matching method found for {errorType.Name}");
                }

                return successMethod.Invoke(null, []);
            }

            var failureMethod = resultGenericType.GetMethod(nameof(Result<object>.Failure));
            if (failureMethod is null)
            {
                throw new InvalidOperationException($"No matching method found for {errorType.Name}");
            }

            var errorJson = token[errorName]!.ToString();
            var error = JsonConvert.DeserializeObject(errorJson, errorType, _converterSettings);
            return failureMethod.Invoke(null, [error]);
        }

        if (objectType.GetGenericTypeDefinition() == typeof(Result<,>))
        {
            var args = objectType.GetGenericArguments();
            var valueType = args[0];
            var errorType = args[1];
            var resultGenericType = typeof(Result<,>).MakeGenericType(valueType, errorType);

            if (isSuccess)
            {
                var successMethod = resultGenericType.GetMethod(nameof(Result<object, object>.Success), [valueType]);
                if (successMethod is null)
                {
                    throw new InvalidOperationException($"No matching method found for {errorType.Name}");
                }

                var valueJson = token[valueName];
                var value = DeserializeToType(valueJson, valueType);
                return successMethod.Invoke(null, [value]);
            }

            var failureMethod = resultGenericType.GetMethod(nameof(Result<object, object>.Failure), [errorType]);
            if (failureMethod is null)
            {
                throw new InvalidOperationException($"No matching method found for {errorType.Name}");
            }

            var errorJson = token[errorName]!.ToString();
            var error = JsonConvert.DeserializeObject(errorJson, errorType, _converterSettings);
            return failureMethod.Invoke(null, [error]);
        }

        throw new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        dynamic result = value;

        writer.WriteStartObject();

        writer.WritePropertyName(Resolve(nameof(Result<object, object>.IsSuccess), serializer));
        writer.WriteValue(result.IsSuccess);

        if (result.IsSuccess)
        {
            if (IsGenericType(result, typeof(Result<,>)))
            {
                writer.WritePropertyName(Resolve(nameof(Result<object, object>.Value), serializer));
                serializer.Serialize(writer, result.Value);
            }

            writer.WritePropertyName(Resolve(nameof(Result<object, object>.Error), serializer));
            writer.WriteNull();
        }
        else
        {
            writer.WritePropertyName(Resolve(nameof(Result<object, object>.Error), serializer));
            serializer.Serialize(writer, result.Error);

            writer.WritePropertyName(Resolve(nameof(Result<object, object>.Value), serializer));
            writer.WriteNull();
        }

        writer.WriteEndObject();
    }

    private static bool IsGenericType(object? obj, Type genericTypeDefinition)
    {
        if (obj is null)
            return false;

        var type = obj.GetType();

        // Check if it matches directly or through inheritance
        return type.IsGenericType &&
               type.GetGenericTypeDefinition() == genericTypeDefinition;
    }

    private static object? DeserializeToType(JToken? valueToken, Type valueType)
    {
        if (valueToken == null || valueToken.Type == JTokenType.Null)
        {
            return null;
        }
        else if (valueToken.Type is JTokenType.String
                 or JTokenType.Integer
                 or JTokenType.Float
                 or JTokenType.Boolean
                 or JTokenType.Guid
                 or JTokenType.Uri
                 or JTokenType.Date)
        {
            // Use ToObject for primitive and scalar types
            return valueToken.ToObject(valueType);
        }
        else
        {
            // Complex type → use full JSON deserialization
            var valueJson = valueToken.ToString();
            return JsonConvert.DeserializeObject(valueJson, valueType, _converterSettings);
        }
    }
}
