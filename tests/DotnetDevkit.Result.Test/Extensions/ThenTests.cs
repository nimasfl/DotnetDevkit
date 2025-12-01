using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class ResultExtensionsTests
{
    private record Error;

    [Fact]
    public void Then_ShouldInvokeCallback_WhenResultIsSuccess_WithTInTOut()
    {
        var input = Result<int, Error>.Success(5);
        var result = input.Then(x => Result<string, Error>.Success(x.ToString()));

        Assert.Equal("5", result.Value);
    }

    [Fact]
    public void Then_ShouldReturnError_WhenResultIsFailure_WithTInTOut()
    {
        var error = new Error();
        var input = Result<int, Error>.Failure(error);
        var result = input.Then(x => Result<string, Error>.Success("should not run"));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Then_ShouldInvokeCallback_WhenResultIsSuccess_WithTOutOnly()
    {
        var input = Result<Error>.Success();
        var result = input.Then(() => Result<string, Error>.Success("done"));

        Assert.Equal("done", result.Value);
    }

    [Fact]
    public void Then_ShouldReturnError_WhenResultIsFailure_WithTOutOnly()
    {
        var error = new Error();
        var input = Result<Error>.Failure(error);
        var result = input.Then(() => Result<string, Error>.Success("fail"));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Then_ShouldInvokeCallback_WhenResultIsSuccess_WithTInOnly()
    {
        var input = Result<int, Error>.Success(10);
        var result = input.Then(x => Result<Error>.Success());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Then_ShouldReturnError_WhenResultIsFailure_WithTInOnly()
    {
        var error = new Error();
        var input = Result<int, Error>.Failure(error);
        var result = input.Then(x => Result<Error>.Success());

        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Then_ShouldInvokeCallback_WhenResultIsSuccess_WithoutGenericInput()
    {
        var input = Result<Error>.Success();
        var result = input.Then(() => Result<Error>.Success());

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Then_ShouldReturnError_WhenResultIsFailure_WithoutGenericInput()
    {
        var error = new Error();
        var input = Result<Error>.Failure(error);
        var result = input.Then(() => Result<Error>.Success());

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldAwaitCallback_WhenResultIsSuccess_AsyncTInTOut()
    {
        var input = Result<int, Error>.Success(42);
        var result = await input.Then(x => Task.FromResult(Result<string, Error>.Success(x.ToString())));

        Assert.Equal("42", result.Value);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultIsFailure_AsyncTInTOut()
    {
        var error = new Error();
        var input = Result<int, Error>.Failure(error);
        var result = await input.Then(x => Task.FromResult(Result<string, Error>.Success("won't run")));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldAwaitCallback_WhenResultTaskIsSuccess_AsyncTaskTInTOutFunc()
    {
        Task<Result<int, Error>> input = Task.FromResult(Result<int, Error>.Success(99));
        var result = await input.Then(x => Task.FromResult(Result<string, Error>.Success(x.ToString())));

        Assert.Equal("99", result.Value);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultTaskIsFailure_AsyncTaskTInTOutFunc()
    {
        var error = new Error();
        Task<Result<int, Error>> input = Task.FromResult(Result<int, Error>.Failure(error));
        var result = await input.Then(x => Task.FromResult(Result<string, Error>.Success("skipped")));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldInvokeCallback_WhenResultIsSuccess_AsyncNoInputTOut()
    {
        var input = Result<Error>.Success();
        var result = await input.Then(() => Task.FromResult(Result<string, Error>.Success("async done")));

        Assert.Equal("async done", result.Value);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultIsFailure_AsyncNoInputTOut()
    {
        var error = new Error();
        var input = Result<Error>.Failure(error);
        var result = await input.Then(() => Task.FromResult(Result<string, Error>.Success("ignored")));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldInvokeCallback_WhenResultTaskIsSuccess_AsyncTaskNoInputTOut()
    {
        Task<Result<Error>> input = Task.FromResult(Result<Error>.Success());
        var result = await input.Then(() => Task.FromResult(Result<string, Error>.Success("yay")));

        Assert.Equal("yay", result.Value);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultTaskIsFailure_AsyncTaskNoInputTOut()
    {
        var error = new Error();
        var input = Task.FromResult(Result<Error>.Failure(error));
        var result = await input.Then(() => Task.FromResult(Result<string, Error>.Success("nope")));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldAwaitCallback_WhenResultIsSuccess_AsyncTInOnly()
    {
        var input = Result<int, Error>.Success(1);
        var result = await input.Then(x => Task.FromResult(Result<Error>.Success()));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultIsFailure_AsyncTInOnly()
    {
        var error = new Error();
        var input = Result<int, Error>.Failure(error);
        var result = await input.Then(x => Task.FromResult(Result<Error>.Success()));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldInvokeCallback_WhenTaskResultIsSuccess_AsyncTaskTInOnly()
    {
        Task<Result<int, Error>> input = Task.FromResult(Result<int, Error>.Success(123));
        var result = await input.Then(x => Task.FromResult(Result<Error>.Success()));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenTaskResultIsFailure_AsyncTaskTInOnly()
    {
        var error = new Error();
        Task<Result<int, Error>> input = Task.FromResult(Result<int, Error>.Failure(error));
        var result = await input.Then(x => Task.FromResult(Result<Error>.Success()));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldAwaitCallback_WhenResultIsSuccess_AsyncNoInput()
    {
        var input = Result<Error>.Success();
        var result = await input.Then(() => Task.FromResult(Result<Error>.Success()));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultIsFailure_AsyncNoInput()
    {
        var error = new Error();
        var input = Result<Error>.Failure(error);
        var result = await input.Then(() => Task.FromResult(Result<Error>.Success()));

        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task Then_ShouldInvokeCallback_WhenResultTaskIsSuccess_AsyncTaskNoInput()
    {
        Task<Result<Error>> input = Task.FromResult(Result<Error>.Success());
        var result = await input.Then(() => Task.FromResult(Result<Error>.Success()));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Then_ShouldReturnError_WhenResultTaskIsFailure_AsyncTaskNoInput()
    {
        var error = new Error();
        Task<Result<Error>> input = Task.FromResult(Result<Error>.Failure(error));
        var result = await input.Then(() => Task.FromResult(Result<Error>.Success()));

        Assert.Same(error, result.Error);
    }
}
