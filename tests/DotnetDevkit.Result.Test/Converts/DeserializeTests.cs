namespace DotnetDevkit.Result.Test.Converts;

public class DeserializeTests
{
    private class ErrorTest(string message, int code)
    {
        public string Message { get; init; } = message;
        public int Code { get; init; } = code;
    }

    [Fact]
    public void
        NewtonsoftDeserializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<string, ErrorTest>>(text);
        var expected = Result<string, ErrorTest>.Success("test");

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.Value, result.Value);
        Assert.Equal(expected.IsFailure, result.IsFailure);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<string, ErrorTest>>(text);
        var expected = Result<string, ErrorTest>.Success("test");

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.Value, result.Value);
        Assert.Equal(expected.IsFailure, result.IsFailure);
    }

    [Fact]
    public void NewtonsoftDeserializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<string, ErrorTest>>(text);
        var expected = Result<string, ErrorTest>.Failure(new ErrorTest("err", 1));

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.IsFailure, result.IsFailure);
        Assert.Equal(expected.Error.Code, result.Error.Code);
        Assert.Equal(expected.Error.Message, result.Error.Message);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Value\":null,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<string, ErrorTest>>(text);
        var expected = Result<string, ErrorTest>.Failure(new ErrorTest("err", 1));

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.Error.Code, result.Error.Code);
        Assert.Equal(expected.Error.Message, result.Error.Message);
    }

    [Fact]
    public void
        NewtonsoftDeserializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Error\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<ErrorTest>>(text);
        var expected = Result<ErrorTest>.Success();

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.IsFailure, result.IsFailure);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var text = "{\"IsSuccess\":true,\"Error\":null}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<ErrorTest>>(text);
        var expected = Result<ErrorTest>.Success();

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.IsFailure, result.IsFailure);
    }

    [Fact]
    public void NewtonsoftDeserializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}";
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<Result<ErrorTest>>(text);
        var expected = Result<ErrorTest>.Failure(new ErrorTest("err", 1));

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.Error.Code, result.Error.Code);
        Assert.Equal(expected.Error.Message, result.Error.Message);
    }

    [Fact]
    public void SystemTextJsonDeserializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var text = "{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\"}}";
        var result = System.Text.Json.JsonSerializer.Deserialize<Result<ErrorTest>>(text);
        var expected = Result<ErrorTest>.Failure(new ErrorTest("err", 1));

        Assert.NotNull(result);
        Assert.Equal(expected.IsSuccess, result.IsSuccess);
        Assert.Equal(expected.Error.Code, result.Error.Code);
        Assert.Equal(expected.Error.Message, result.Error.Message);
    }
}
