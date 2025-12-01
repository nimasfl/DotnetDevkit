using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class MatchTests
{
    private class TestError
    {
        public string Message { get; set; } = string.Empty;
    }

    [Fact]
    public void Match_ShouldReturnSuccessProjection_WhenResultIsSuccess()
    {
        var result = Result<TestError>.Success();
        var output = result.Match(
            onSuccess: () => "success",
            onFailure: _ => "failure");

        Assert.Equal("success", output);
    }

    [Fact]
    public void Match_ShouldReturnFailureProjection_WhenResultIsFailure()
    {
        var result = Result<TestError>.Failure(new TestError { Message = "fail" });
        var output = result.Match(
            onSuccess: () => "success",
            onFailure: _ => "failure");

        Assert.Equal("failure", output);
    }

    [Fact]
    public void MatchTIn_ShouldReturnSuccessProjection_WhenResultIsSuccess()
    {
        var result = Result<string, TestError>.Success("value");
        var output = result.Match(
            onSuccess: val => val.ToUpper(),
            onFailure: _ => "failure");

        Assert.Equal("VALUE", output);
    }

    [Fact]
    public void MatchTIn_ShouldReturnFailureProjection_WhenResultIsFailure()
    {
        var result = Result<string, TestError>.Failure(new TestError { Message = "fail" });
        var output = result.Match(
            onSuccess: val => val.ToUpper(),
            onFailure: _ => "failure");

        Assert.Equal("failure", output);
    }

    [Fact]
    public async Task MatchAsync_ShouldReturnSuccessProjection_WhenTaskOfResultIsSuccess()
    {
        var result = Task.FromResult(Result<TestError>.Success());
        var output = await result.Match(
            onSuccess: () => "ok",
            onFailure: _ => "not ok");

        Assert.Equal("ok", output);
    }

    [Fact]
    public async Task MatchAsync_ShouldReturnFailureProjection_WhenTaskOfResultIsFailure()
    {
        var result = Task.FromResult(Result<TestError>.Failure(new TestError()));
        var output = await result.Match(
            onSuccess: () => "ok",
            onFailure: _ => "not ok");

        Assert.Equal("not ok", output);
    }

    [Fact]
    public async Task MatchAsyncTIn_ShouldReturnSuccessProjection_WhenTaskOfGenericResultIsSuccess()
    {
        var result = Task.FromResult(Result<string, TestError>.Success("value"));
        var output = await result.Match(
            onSuccess: val => val + "_ok",
            onFailure: _ => "fail");

        Assert.Equal("value_ok", output);
    }

    [Fact]
    public async Task MatchAsyncTIn_ShouldReturnFailureProjection_WhenTaskOfGenericResultIsFailure()
    {
        var result = Task.FromResult(Result<string, TestError>.Failure(new TestError()));
        var output = await result.Match(
            onSuccess: val => val + "_ok",
            onFailure: _ => "fail");

        Assert.Equal("fail", output);
    }
}
