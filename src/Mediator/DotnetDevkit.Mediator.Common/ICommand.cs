namespace DotnetDevkit.Mediator.Common;

public interface ICommand: IRequest;

public interface ICommand<TResponse>: IRequest<TResponse>;
