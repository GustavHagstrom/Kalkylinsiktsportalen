namespace BlazorServer.Shared.Kernel.Features;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<CommandResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
