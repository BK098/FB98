using FB98.Shared.Abstractions.Entities;
using MediatR;

namespace FB98.Shared.Abstractions.CQRS
{
	public interface IQuery<out TResponse> : IRequest<TResponse>
		where TResponse : class, IResponse
	{
	}
}