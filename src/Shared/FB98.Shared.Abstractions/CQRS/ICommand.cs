using MediatR;

namespace FB98.Shared.Abstractions.CQRS
{
	public interface ICommand<out TResponse> : IRequest<TResponse>
	{
	}
}