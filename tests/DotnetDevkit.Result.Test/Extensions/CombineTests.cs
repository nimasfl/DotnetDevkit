using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class CombineTests
{
    private record DummyError;

    [Fact]
    public void Combine_ShouldReturnSuccess_WhenAllResultsAreSuccessful_WithoutValues()
    {
        var results = new[]
        {
            Result<DummyError>.Success(),
            Result<DummyError>.Success(),
            Result<DummyError>.Success()
        };

        var combined = ResultExtensions.Combine(results);

        Assert.True(combined.IsSuccess);
    }

    [Fact]
    public void Combine_ShouldReturnFirstError_WhenOneResultIsFailure_WithoutValues()
    {
        var error = new DummyError();
        var results = new[]
        {
            Result<DummyError>.Success(),
            Result<DummyError>.Failure(error),
            Result<DummyError>.Success()
        };

        var combined = ResultExtensions.Combine(results);

        Assert.Same(error, combined.Error);
    }

    [Fact]
    public void Combine_ShouldReturnFirstError_WhenMultipleResultsAreFailure_WithoutValues()
    {
        var error1 = new DummyError();
        var error2 = new DummyError();
        var results = new[]
        {
            Result<DummyError>.Failure(error1),
            Result<DummyError>.Failure(error2)
        };

        var combined = ResultExtensions.Combine(results);

        Assert.Same(error1, combined.Error);
    }

    [Fact]
    public void Combine_ShouldReturnSuccess_WhenEmptyInput_WithoutValues()
    {
        var combined = ResultExtensions.Combine<DummyError>();

        Assert.True(combined.IsSuccess);
    }

    // ----- ResultExtensions.Combine<T, TError>() -----

    [Fact]
    public void Combine_ShouldReturnAllValues_WhenAllResultsAreSuccessful_WithValues()
    {
        var results = new[]
        {
            Result<int, DummyError>.Success(1),
            Result<int, DummyError>.Success(2),
            Result<int, DummyError>.Success(3)
        };

        var combined = ResultExtensions.Combine(results);

        Assert.Equal(new List<int> { 1, 2, 3 }, combined.Value);
    }

    [Fact]
    public void Combine_ShouldReturnError_WhenOneResultIsFailure_WithValues()
    {
        var error = new DummyError();
        var results = new[]
        {
            Result<int, DummyError>.Success(1),
            Result<int, DummyError>.Failure(error),
            Result<int, DummyError>.Success(3)
        };

        var combined = ResultExtensions.Combine(results);

        Assert.Same(error, combined.Error);
    }

    [Fact]
    public void Combine_ShouldReturnFirstError_WhenMultipleResultsAreFailure_WithValues()
    {
        var error1 = new DummyError();
        var error2 = new DummyError();
        var results = new[]
        {
            Result<int, DummyError>.Failure(error1),
            Result<int, DummyError>.Failure(error2)
        };

        var combined = ResultExtensions.Combine(results);

        Assert.Same(error1, combined.Error);
    }

    [Fact]
    public void Combine_ShouldReturnEmptyList_WhenEmptyInput_WithValues()
    {
        var combined = ResultExtensions.Combine<int, DummyError>();

        Assert.Empty(combined.Value);
    }
}
