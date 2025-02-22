using FB98.Shared.Abstractions.Entities;
using MediatR;

namespace FB98.Shared.Abstractions.CQRS
{
	public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
		where TQuery : IQuery<TResponse>
		where TResponse : class, IResponse
	{
	}
}