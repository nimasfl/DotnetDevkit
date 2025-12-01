namespace DotnetDevkit.Result.Test.Converts;

public class SerializeTests
{
    private record ErrorTest(int Code, string Message, int Type);

    [Fact]
    public void NewtonsoftSerializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<string, ErrorTest>.Success("test");

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":true,\"IsFailure\":false,\"Value\":\"test\",\"Error\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<string, ErrorTest>.Success("test");

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":true,\"IsFailure\":false,\"Value\":\"test\",\"Error\":null}", s);
    }

    [Fact]
    public void NewtonsoftSerializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<string, ErrorTest>.Failure(new ErrorTest(1, "err", 500));

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal(
            "{\"IsSuccess\":false,\"IsFailure\":true,\"Value\":null,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}",
            s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<string, ErrorTest>.Failure(new ErrorTest(1, "err", 500));

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal(
            "{\"IsSuccess\":false,\"IsFailure\":true,\"Value\":null,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}",
            s);
    }

    [Fact]
    public void
        NewtonsoftSerializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<ErrorTest>.Success();

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":true,\"IsFailure\":false,\"Error\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<ErrorTest>.Success();

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":true,\"IsFailure\":false,\"Error\":null}", s);
    }

    [Fact]
    public void NewtonsoftSerializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<ErrorTest>.Failure(new ErrorTest(1, "err", 500));

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":false,\"IsFailure\":true,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}",
            s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<ErrorTest>.Failure(new ErrorTest(1, "err", 500));

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":false,\"IsFailure\":true,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}",
            s);
    }
}
