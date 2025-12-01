namespace DotnetDevkit.Result.Test.Converts;

public class SerializeTests
{
    private record ErrorTest(string Message, int Code);

    [Fact]
    public void NewtonsoftSerializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<string, ErrorTest>.Success("test");

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<string, ErrorTest>.Success("test");

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":true,\"Value\":\"test\",\"Error\":null}", s);
    }

    [Fact]
    public void NewtonsoftSerializeOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<string, ErrorTest>.Failure(new ErrorTest("err", 1));

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<string, ErrorTest>.Failure(new ErrorTest("err", 1));

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":false,\"Value\":null,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}", s);
    }

    [Fact]
    public void
        NewtonsoftSerializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<ErrorTest>.Success();

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":true,\"Error\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValidAndIsSuccess()
    {
        var result = Result<ErrorTest>.Success();

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":true,\"Error\":null}", s);
    }

    [Fact]
    public void NewtonsoftSerializeOnResultWithoutValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<ErrorTest>.Failure(new ErrorTest("err", 1));

        var s = Newtonsoft.Json.JsonConvert.SerializeObject(result);

        Assert.Equal("{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500},\"Value\":null}", s);
    }

    [Fact]
    public void SystemTextJsonSerializeOnResultWithoutValueOnResultWithValue_ShouldWorkCorrectly_WhenDataIsValid()
    {
        var result = Result<ErrorTest>.Failure(new ErrorTest("err", 1));

        var s = System.Text.Json.JsonSerializer.Serialize(result);

        Assert.Equal("{\"IsSuccess\":false,\"Error\":{\"Code\":1,\"Message\":\"err\",\"Type\":500}}", s);
    }
}
