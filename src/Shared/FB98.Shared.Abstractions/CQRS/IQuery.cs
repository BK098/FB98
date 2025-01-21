using MediatR;

namespace FB98.Shared.Abstractions.CQRS
{
	public interface IQuery<out TResponse> : IRequest<TResponse>
	{
	}
}
