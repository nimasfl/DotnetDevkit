using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DotnetDevkit.Result.Converters;

internal class ResultSystemTextJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType) return false;
        var def = typeToConvert.GetGenericTypeDefinition();
        return def == typeof(Result<>) || def == typeof(Result<,>);
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        var def = type.GetGenericTypeDefinition();

        if (def == typeof(Result<>))
        {
            var errorType = type.GetGenericArguments()[0];
            var converterType = typeof(ResultErrorJsonConverter<>).MakeGenericType(errorType);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        if (def == typeof(Result<,>))
        {
            var args = type.GetGenericArguments();
            var converterType = typeof(ResultValueErrorJsonConverter<,>).MakeGenericType(args);
            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }

        throw new NotSupportedException();
    }
}

internal class ResultErrorJsonConverter<TError> : JsonConverter<Result<TError>>
    where TError : class
{
    private static string Resolve(string name, JsonSerializerOptions options)
        => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

    public override Result<TError> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var isSuccess = root.GetProperty(Resolve(nameof(Result<>.IsSuccess), options)).GetBoolean();

        if (isSuccess)
        {
            return Result<TError>.Success();
        }

        var errorElem = root.GetProperty(Resolve(nameof(Result<>.Error), options));
        var error = JsonSerializer.Deserialize<TError>(errorElem.GetRawText(), options)!;
        return Result<TError>.Failure(error);
    }

    public override void Write(Utf8JsonWriter writer, Result<TError> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(Resolve(nameof(Result<>.IsSuccess), options));
        writer.WriteBooleanValue(value.IsSuccess);


        if (value.IsSuccess is false)
        {
            writer.WritePropertyName(Resolve(nameof(Result<>.Error), options));
            JsonSerializer.Serialize(writer, value.Error, options);
        }
        else
        {
            writer.WriteNull(Resolve(nameof(Result<>.Error), options));
        }

        writer.WriteEndObject();
    }
}

internal class
    ResultValueErrorJsonConverter<TValue, TError> : JsonConverter<Result<TValue, TError>>
    where TError : class
{
    private static string Resolve(string name, JsonSerializerOptions options)
        => options.PropertyNamingPolicy?.ConvertName(name) ?? name;

    public override Result<TValue, TError> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var isSuccess = root.GetProperty(Resolve(nameof(Result<,>.IsSuccess), options)).GetBoolean();

        if (isSuccess)
        {
            var valueElem = root.GetProperty(Resolve(nameof(Result<,>.Value), options));
            var value = JsonSerializer.Deserialize<TValue>(valueElem.GetRawText(), options)!;
            return Result<TValue, TError>.Success(value);
        }

        var errorElem = root.GetProperty(Resolve(nameof(Result<>.Error), options));
        var error = JsonSerializer.Deserialize<TError>(errorElem.GetRawText(), options)!;
        return Result<TValue, TError>.Failure(error);
    }

    public override void Write(Utf8JsonWriter writer, Result<TValue, TError> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(Resolve(nameof(Result<,>.IsSuccess), options));
        writer.WriteBooleanValue(value.IsSuccess);

        if (value.IsSuccess)
        {
            writer.WritePropertyName(Resolve(nameof(Result<,>.Value), options));
            JsonSerializer.Serialize(writer, value.Value, options);

            writer.WriteNull(Resolve(nameof(Result<>.Error), options));
        }
        else
        {
            writer.WriteNull(Resolve(nameof(Result<,>.Value), options));

            writer.WritePropertyName(Resolve(nameof(Result<>.Error), options));
            JsonSerializer.Serialize(writer, value.Error, options);
        }

        writer.WriteEndObject();
    }
}
