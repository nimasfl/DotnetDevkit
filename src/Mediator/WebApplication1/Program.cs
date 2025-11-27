using DotnetDevkit.Mediator.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMediator(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddBehavior(typeof(B1<>));
    config.AddBehavior(typeof(B2<>));
    config.AddBehavior(typeof(B3<>));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/test",
        async (ISender sender, CancellationToken cancellationToken) =>
        {
            await sender.Send(new SomeCommand(), cancellationToken);
        })
    .WithName("GetWeatherForecast");

app.Run();


public record SomeCommand : IRequest;

public class B1<TRequest> : IRequestBehavior<TRequest> where TRequest : IRequest
{
    public async Task Handle(TRequest command, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        await next();
    }
}

public class B2<TRequest> : IRequestBehavior<TRequest> where TRequest : IRequest
{
    public async Task Handle(TRequest command, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        await next();
    }
}

public class B3<TRequest> : IRequestBehavior<TRequest> where TRequest : IRequest
{
    public async Task Handle(TRequest command, CancellationToken cancellationToken, RequestHandlerDelegate next)
    {
        await next();
    }
}


public class SomeCommandHandler : IRequestHandler<SomeCommand>
{
    public async Task Handle(SomeCommand command, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken);
    }
}
