using AutoMapper;
using FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodo;
using FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetAll;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetail;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem;
using FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodo;
using FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodoItem;
using FB98.Modules.ShoppingList.Domain.Entites;

namespace FB98.Modules.ShoppingList.Application.TodoManagement
{
	internal class TodoProfile : Profile
	{
		public TodoProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateTodoDto, Todo>();
			CreateMap<UpdateTodoDto, Todo>();
			CreateMap<UpdateTodoItemDto, TodoItem>();
			CreateMap<CreateTodoItemDto, TodoItem>();
			CreateMap<Todo, GetAllTodoResponse>();
			CreateMap<Todo, GetDetailTodoResponse>()
				.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
			CreateMap<TodoItem, GetDetailTodoItemResponse>();
		}
	}
}