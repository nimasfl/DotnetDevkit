using DotnetDevkit.Result.Extensions;

namespace DotnetDevkit.Result.Test.Extensions;

public class ToResultTests
{
    private class TestError;

    [Fact]
    public void ToResult_ShouldReturnSuccessResult_WhenValueIsNotIResult()
    {
        // Arrange
        var input = 42;

        // Act
        var result = input.ToResult<int, TestError>();

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ToResult_ShouldWrapValue_WhenValueIsNotIResult()
    {
        // Arrange
        var input = 99;

        // Act
        var result = input.ToResult<int, TestError>();

        // Assert
        Assert.Equal(99, result.Value);
    }
}
