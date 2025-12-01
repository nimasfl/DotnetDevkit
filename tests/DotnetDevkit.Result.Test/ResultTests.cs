using DotnetDevkit.Result.Extensions;
using FakeItEasy;

namespace DotnetDevkit.Result.Test;

public class ResultTests
{
    [Fact]
    public void Success_ShouldReturnSuccessResult_WhenCalled()
    {
        var result = Result<ErrorBase>.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Begin_ShouldReturnSuccessResult_WhenCalled()
    {
        var result = Result<ErrorBase>.Begin();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void BeginWithValue_ShouldReturnSuccessResult_WhenCalled()
    {
        var result = Result<bool, ErrorBase>.Begin(true);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void SuccessGeneric_ShouldReturnSuccessResultWithValue_WhenCalled()
    {
        var result = Result<int, ErrorBase>.Success(123);

        Assert.True(result.IsSuccess);
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void FailureGeneric_ShouldReturnFailureResult_WhenCalledWithError()
    {
        var error = A.Fake<ErrorBase>();
        var result = Result<ErrorBase>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void GenericImplicitConversion_ShouldReturnFailureResult_WhenErrorIsProvided()
    {
        var error = A.Fake<ErrorBase>();
        Result<ErrorBase> result = error;

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Value_ShouldReturnValue_WhenResultIsSuccess()
    {
        var result = Result<string, ErrorBase>.Success("test");

        Assert.True(result.IsSuccess);
        Assert.Equal("test", result.Value);
    }

    [Fact]
    public void Value_ShouldThrowInvalidOperationException_WhenResultIsFailure()
    {
        var error = A.Fake<ErrorBase>();
        var result = Result<string, ErrorBase>.Failure(error);

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var _ = result.Value;
        });

        Assert.Equal("The value of a failure result can't be accessed.", ex.Message);
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnSuccessResult_WhenValueIsProvided()
    {
        Result<string, ErrorBase> result = "value";

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnFailureResult_WhenErrorIsProvided()
    {
        var error = A.Fake<ErrorBase>();
        Result<string, ErrorBase> result = error;

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void ValidationFailure_ShouldReturnFailureResult_WhenCalledWithError()
    {
        var error = A.Fake<ErrorBase>();
        var result = Result<string, ErrorBase>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public async Task ChainMethods_ShouldReturnSuccessResult_WhenNoErrorHappens()
    {
        var a = await "a".ToResult<string, ErrorBase>()
            .Then(async a2 =>
            {
                Assert.Equal("a", a2);
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success("b");
            })
            .Then(b2 =>
            {
                Assert.Equal("b", b2);
                return Result<string, ErrorBase>.Success("c");
            })
            .Then(async c2 =>
            {
                Assert.Equal("c", c2);
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success("d");
            });

        Assert.Equal("d", a.Value);

        var b = Task.FromResult(Result<ErrorBase>.Success())
            .Then(() => Result<string, ErrorBase>.Success("a"));

        Assert.Equal("a", await b);

        var c = Task.FromResult(Result<ErrorBase>.Success())
            .Then(async () =>
            {
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success("c");
            });

        Assert.Equal("c", await c);

        var d = await "d".ToResult<string, ErrorBase>()
            .Then(async _ =>
            {
                await Task.Delay(1);
                return Result<ErrorBase>.Success();
            });

        Assert.True(d.IsSuccess);

        var e = await "e"
            .ToResult<string, ErrorBase>()
            .Then(async e =>
            {
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success(e);
            })
            .Then(e1 =>
            {
                Assert.Equal("e", e1);
                return Result<ErrorBase>.Success();
            });
        Assert.True(e.IsSuccess);

        var f = await "f".ToResult<string, ErrorBase>()
            .Then(async f =>
            {
                Assert.Equal("f", f);
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success("g");
            })
            .Then(async g =>
            {
                Assert.Equal("g", g);
                await Task.Delay(1);
                return Result<ErrorBase>.Success();
            });
        Assert.True(f.IsSuccess);

        var g = await Result<ErrorBase>.Success()
            .Then(async () =>
            {
                await Task.Delay(1);
                return Result<ErrorBase>.Success();
            })
            .Then(Result<ErrorBase>.Success)
            .Then(async () =>
            {
                await Task.Delay(1);
                return Result<ErrorBase>.Success();
            });
        Assert.True(g.IsSuccess);

        var h = await "a".ToResult<string, ErrorBase>()
            .Then(a1 =>
            {
                Assert.Equal("a", a1);
                return Result<string, ErrorBase>.Success("b");
            })
            .Then(() => Result<string, ErrorBase>.Success("c"))
            .Then(c1 =>
            {
                Assert.Equal("c", c1);
                return Result<ErrorBase>.Success();
            })
            .Then(Result<ErrorBase>.Success)
            .Then(async () =>
            {
                await Task.Delay(1);
                return Result<string, ErrorBase>.Success("d");
            });

        Assert.Equal("d", h.Value);
    }

    public record ErrorBase(string Message);
}
