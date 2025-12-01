namespace DotnetDevkit.Result.Test.Converts;

public class DeserializeTests
{
    private class ErrorTest
    {
        public string Message { get; init; }
        public int Code { get; init; }
        public int Type { get; init; }
    }

    [Fact]
    public void
        NewtonsoftDeserializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        const string text = "{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<string, ErrorTest>>(text);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("test", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        const string text = "{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<string, ErrorTest>>(text);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("test", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void NewtonsoftDeserializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<string, ErrorTest>>(text);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal(1, result.Error.Code);
        Assert.Equal("err", result.Error.Message);
        Assert.Equal(500, result.Error.Type);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Value\":null,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<string, ErrorTest>>(text);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal(1, result.Error.Code);
        Assert.Equal("err", result.Error.Message);
        Assert.Equal(500, result.Error.Type);
    }

    [Fact]
    public void
        NewtonsoftDeserializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Error\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<ErrorTest>>(text);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Error\":null}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<ErrorTest>>(text);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void NewtonsoftDeserializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<ErrorTest>>(text);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(1, result.Error.Code);
        Assert.Equal("err", result.Error.Message);
        Assert.Equal(500, result.Error.Type);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<ErrorTest>>(text);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(1, result.Error.Code);
        Assert.Equal("err", result.Error.Message);
        Assert.Equal(500, result.Error.Type);
    }
}
