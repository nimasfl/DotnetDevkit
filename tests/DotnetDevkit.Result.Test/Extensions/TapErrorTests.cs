using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class TapErrorTests
{
    private class TestError
    {
        public string Message { get; init; } = string.Empty;
    }

    [Fact]
    public async Task TapError_ShouldExecuteAction_WhenResultIsFailure()
    {
        var wasCalled = false;
        var result = Result<TestError>.Failure(new TestError());

        await result.TapError(() =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        Assert.True(wasCalled);
    }

    [Fact]
    public async Task TapError_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        var wasCalled = false;
        var result = Result<TestError>.Success();

        await result.TapError(() =>
        {
            wasCalled = true;
            return Task.CompletedTask;
        });

        Assert.False(wasCalled);
    }

    [Fact]
    public void TapErrorSync_ShouldExecuteAction_WhenResultIsFailure()
    {
        var wasCalled = false;
        var result = Result<TestError>.Failure(new TestError());

        result.TapError(() => wasCalled = true);

        Assert.True(wasCalled);
    }

    [Fact]
    public void TapErrorSync_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        var wasCalled = false;
        var result = Result<TestError>.Success();

        result.TapError(() => wasCalled = true);

        Assert.False(wasCalled);
    }

    [Fact]
    public void TapErrorWithValue_ShouldExecuteAction_WhenResultIsFailure()
    {
        TestError? capturedError = null;
        var error = new TestError { Message = "error" };
        var result = Result<string, TestError>.Failure(error);

        result.TapError(err => capturedError = err);

        Assert.Equal("error", capturedError?.Message);
    }

    [Fact]
    public void TapErrorWithValue_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        var wasCalled = false;
        var result = Result<string, TestError>.Success("value");

        result.TapError(_ => wasCalled = true);

        Assert.False(wasCalled);
    }

    [Fact]
    public async Task TapErrorWithValueAsync_ShouldExecuteAction_WhenResultIsFailure()
    {
        TestError? captured = null;
        var error = new TestError { Message = "fail" };
        var result = Result<string, TestError>.Failure(error);

        await result.TapError(async err =>
        {
            captured = err;
            await Task.CompletedTask;
        });

        Assert.Equal("fail", captured?.Message);
    }

    [Fact]
    public async Task TapErrorWithValueAsync_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        var wasCalled = false;
        var result = Result<string, TestError>.Success("success");

        await result.TapError(async _ =>
        {
            wasCalled = true;
            await Task.CompletedTask;
        });

        Assert.False(wasCalled);
    }

    [Fact]
    public async Task TapErrorFromTask_ShouldExecuteAction_WhenTaskResultIsFailure()
    {
        TestError? captured = null;
        var task = Task.FromResult(Result<string, TestError>.Failure(new TestError { Message = "bad" }));

        await task.TapError(async err =>
        {
            captured = err;
            await Task.CompletedTask;
        });

        Assert.Equal("bad", captured?.Message);
    }

    [Fact]
    public async Task TapErrorFromTask_ShouldNotExecuteAction_WhenTaskResultIsSuccess()
    {
        var wasCalled = false;
        var task = Task.FromResult(Result<string, TestError>.Success("ok"));

        await task.TapError(async _ =>
        {
            wasCalled = true;
            await Task.CompletedTask;
        });

        Assert.False(wasCalled);
    }

    [Fact]
    public async Task TapErrorFromTaskSyncAction_ShouldExecuteAction_WhenTaskResultIsFailure()
    {
        TestError? captured = null;
        var task = Task.FromResult(Result<string, TestError>.Failure(new TestError { Message = "oops" }));

        await task.TapError(err => captured = err);

        Assert.Equal("oops", captured?.Message);
    }

    [Fact]
    public async Task TapErrorFromTaskSyncAction_ShouldNotExecuteAction_WhenTaskResultIsSuccess()
    {
        var wasCalled = false;
        var task = Task.FromResult(Result<string, TestError>.Success("hello"));

        await task.TapError(_ => wasCalled = true);

        Assert.False(wasCalled);
    }
}
