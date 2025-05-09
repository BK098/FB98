using FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodo;
using FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetAll;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetail;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem;
using FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodo;
using FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodoItem;
using FB98.Shared.Abstractions.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.ShoppingList.Api.Controllers
{
	internal class TodosController : BaseController
	{
		public TodosController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost]
		public async Task<IActionResult> CreateTodo(CreateTodoDto model)
		{
			var request = new CreateTodoCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("item")]
		public async Task<IActionResult> CreateTodoItem(CreateTodoItemDto model)
		{
			var request = new CreateTodoItemCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllTodo([FromQuery] Filter filter)
		{
			var request = new GetAllTodoQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{todoId:guid}")]
		public async Task<IActionResult> GetDetailTodo(Guid todoId)
		{
			var request = new GetDetailTodoQuery(todoId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("{todoId:guid}")]
		public async Task<IActionResult> UpdateDetailTodo(Guid todoId, [FromBody] UpdateTodoDto model)
		{
			var request = new UpdateTodoCommand(todoId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("item/{todoItemId:guid}")]
		public async Task<IActionResult> UpdateTodoItem(Guid todoItemId, [FromBody] UpdateTodoItemDto model)
		{
			var request = new UpdateTodoItemCommand(todoItemId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("item/{todoItemId:guid}")]
		public async Task<IActionResult> GetDetailTodoItem(Guid todoItemId)
		{
			var request = new GetDetailTodoItemQuery(todoItemId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}