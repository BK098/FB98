using FB98.Shared.Abstractions.Entities;
using MediatR;

namespace FB98.Shared.Abstractions.CQRS
{
	public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
		where TCommand : ICommand<TResponse>, IResponse
	{
	}
}