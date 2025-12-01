using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class EnsureTests
{
    private sealed class TestError
    {
        public string Message { get; init; } = "Some error";
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForResultOfTError()
    {
        var result = Result<TestError>.Success();
        var error = new TestError();

        var ensured = await result.Ensure(() => Task.FromResult(false), error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForResultOfTError()
    {
        var result = Result<TestError>.Success();
        var error = new TestError();

        var ensured = await result.Ensure(() => Task.FromResult(true), error);

        Assert.True(ensured.IsSuccess);
    }

    [Fact]
    public void ReturnError_WhenPredicateReturnsFalse_ForResultOfTInTError()
    {
        var result = Result<int, TestError>.Success(42);
        var error = new TestError();

        var ensured = result.Ensure(v => v < 0, error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public void ReturnResult_WhenPredicateReturnsTrue_ForResultOfTInTError()
    {
        var result = Result<int, TestError>.Success(42);
        var error = new TestError();

        var ensured = result.Ensure(v => v > 0, error);

        Assert.True(ensured.IsSuccess);
        Assert.Equal(42, ensured.Value);
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForAsyncPredicateOfResultOfTInTError()
    {
        var result = Result<int, TestError>.Success(42);
        var error = new TestError();

        var ensured = await result.Ensure(v => Task.FromResult(v < 0), error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForAsyncPredicateOfResultOfTInTError()
    {
        var result = Result<int, TestError>.Success(42);
        var error = new TestError();

        var ensured = await result.Ensure(v => Task.FromResult(v > 0), error);

        Assert.True(ensured.IsSuccess);
        Assert.Equal(42, ensured.Value);
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForTaskResultOfTError_WithSyncPredicate()
    {
        var resultTask = Task.FromResult(Result<TestError>.Success());
        var error = new TestError();

        var ensured = await resultTask.Ensure(() => false, error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForTaskResultOfTError_WithSyncPredicate()
    {
        var resultTask = Task.FromResult(Result<TestError>.Success());
        var error = new TestError();

        var ensured = await resultTask.Ensure(() => true, error);

        Assert.True(ensured.IsSuccess);
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForTaskResultOfTError_WithAsyncPredicate()
    {
        var resultTask = Task.FromResult(Result<TestError>.Success());
        var error = new TestError();

        var ensured = await resultTask.Ensure(() => Task.FromResult(false), error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForTaskResultOfTError_WithAsyncPredicate()
    {
        var resultTask = Task.FromResult(Result<TestError>.Success());
        var error = new TestError();

        var ensured = await resultTask.Ensure(() => Task.FromResult(true), error);

        Assert.True(ensured.IsSuccess);
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForTaskResultOfTInTError_WithSyncPredicate()
    {
        var resultTask = Task.FromResult(Result<int, TestError>.Success(42));
        var error = new TestError();

        var ensured = await resultTask.Ensure(v => v < 0, error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForTaskResultOfTInTError_WithSyncPredicate()
    {
        var resultTask = Task.FromResult(Result<int, TestError>.Success(42));
        var error = new TestError();

        var ensured = await resultTask.Ensure(v => v > 0, error);

        Assert.True(ensured.IsSuccess);
        Assert.Equal(42, ensured.Value);
    }

    [Fact]
    public async Task ReturnError_WhenPredicateReturnsFalse_ForTaskResultOfTInTError_WithAsyncPredicate()
    {
        var resultTask = Task.FromResult(Result<int, TestError>.Success(42));
        var error = new TestError();

        var ensured = await resultTask.Ensure(v => Task.FromResult(v < 0), error);

        Assert.True(ensured.IsFailure);
        Assert.Equal(error, ensured.Error);
    }

    [Fact]
    public async Task ReturnResult_WhenPredicateReturnsTrue_ForTaskResultOfTInTError_WithAsyncPredicate()
    {
        var resultTask = Task.FromResult(Result<int, TestError>.Success(42));
        var error = new TestError();

        var ensured = await resultTask.Ensure(v => Task.FromResult(v > 0), error);

        Assert.True(ensured.IsSuccess);
        Assert.Equal(42, ensured.Value);
    }
}
