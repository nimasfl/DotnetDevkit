using System.Diagnostics.CodeAnalysis;
using DotnetDevkit.Result.Extensions;
using FakeItEasy;

namespace DotnetDevkit.Result.Test.Extensions;

[SuppressMessage("Assertions", "xUnit2005:Do not use identity check on value type")]
public class TapTests
{
    private class TestError
    {
    }

    [Fact]
    public void TapSyncAction_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<TestError>.Success();
        var action = A.Fake<Action>();

        // Act
        var finalResult = result.Tap(action);

        // Assert
        A.CallTo(() => action()).MustHaveHappenedOnceExactly();
        Assert.NotSame(result, finalResult);
        Assert.Equal(result, finalResult);
    }

    [Fact]
    public void TapSyncAction_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var error = new TestError();
        var result = Result<TestError>.Failure(error);
        var action = A.Fake<Action>();

        // Act
        var finalResult = result.Tap(action);

        // Assert
        A.CallTo(() => action()).MustNotHaveHappened();
        Assert.Equal(error, finalResult.Error);
    }

    [Fact]
    public async Task TapAsyncAction_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var result = Result<TestError>.Success();
        var wasCalled = false;

        Task Action()
        {
            wasCalled = true;
            return Task.CompletedTask;
        }

        // Act
        var finalResult = await result.Tap(Action);

        // Assert
        Assert.True(wasCalled);
        Assert.NotSame(result, finalResult);
        Assert.Equal(result, finalResult);
    }

    [Fact]
    public async Task TapAsyncAction_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var error = new TestError();
        var result = Result<TestError>.Failure(error);
        var wasCalled = false;

        Task Action()
        {
            wasCalled = true;
            return Task.CompletedTask;
        }

        // Act
        var finalResult = await result.Tap(Action);

        // Assert
        Assert.False(wasCalled);
        Assert.Equal(error, finalResult.Error);
    }

    [Fact]
    public void TapSyncTValue_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var action = A.Fake<Action<string>>();
        var result = Result<string, TestError>.Success("hello");

        // Act
        var finalResult = result.Tap(action);

        // Assert
        A.CallTo(() => action("hello")).MustHaveHappenedOnceExactly();
        Assert.Equal("hello", finalResult.Value);
    }

    [Fact]
    public void TapSyncTValue_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var error = new TestError();
        var action = A.Fake<Action<string>>();
        var result = Result<string, TestError>.Failure(error);

        // Act
        var finalResult = result.Tap(action);

        // Assert
        A.CallTo(() => action(A<string>.Ignored)).MustNotHaveHappened();
        Assert.Equal(error, finalResult.Error);
    }

    [Fact]
    public async Task TapAsyncTValue_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var wasCalled = false;
        var result = Result<string, TestError>.Success("hello");

        async Task Action(string _)
        {
            wasCalled = true;
            await Task.CompletedTask;
        }

        // Act
        var finalResult = await result.Tap(Action);

        // Assert
        Assert.True(wasCalled);
        Assert.Equal("hello", finalResult.Value);
    }

    [Fact]
    public async Task TapAsyncTValue_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var wasCalled = false;
        var error = new TestError();
        var result = Result<string, TestError>.Failure(error);

        async Task Action(string _)
        {
            wasCalled = true;
            await Task.CompletedTask;
        }

        // Act
        var finalResult = await result.Tap(Action);

        // Assert
        Assert.False(wasCalled);
        Assert.Equal(error, finalResult.Error);
    }

    [Fact]
    public async Task TapTaskResultAsyncTValue_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var wasCalled = false;
        Task<Result<string, TestError>> GetResult() => Task.FromResult(Result<string, TestError>.Success("test"));

        async Task Action(string _)
        {
            wasCalled = true;
            await Task.CompletedTask;
        }

        // Act
        var finalResult = await GetResult().Tap(Action);

        // Assert
        Assert.True(wasCalled);
        Assert.Equal("test", finalResult.Value);
    }

    [Fact]
    public async Task TapTaskResultAsyncTValue_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var wasCalled = false;
        var error = new TestError();
        Task<Result<string, TestError>> GetResult() => Task.FromResult(Result<string, TestError>.Failure(error));

        async Task Action(string _)
        {
            wasCalled = true;
            await Task.CompletedTask;
        }

        // Act
        var finalResult = await GetResult().Then(Action);

        // Assert
        Assert.False(wasCalled);
        Assert.Equal(error, finalResult.Error);
    }

    [Fact]
    public async Task TapTaskResultSyncTValue_ShouldInvokeAction_WhenResultIsSuccess()
    {
        // Arrange
        var action = A.Fake<Action<string>>();
        Task<Result<string, TestError>> GetResult() => Task.FromResult(Result<string, TestError>.Success("done"));

        // Act
        var finalResult = await GetResult().Tap(action);

        // Assert
        A.CallTo(() => action("done")).MustHaveHappenedOnceExactly();
        Assert.Equal("done", finalResult.Value);
    }

    [Fact]
    public async Task TapTaskResultSyncTValue_ShouldNotInvokeAction_WhenResultIsFailure()
    {
        // Arrange
        var error = new TestError();
        var action = A.Fake<Action<string>>();
        Task<Result<string, TestError>> GetResult() => Task.FromResult(Result<string, TestError>.Failure(error));

        // Act
        var finalResult = await GetResult().Tap(action);

        // Assert
        A.CallTo(() => action(A<string>.Ignored)).MustNotHaveHappened();
        Assert.Equal(error, finalResult.Error);
    }
}
