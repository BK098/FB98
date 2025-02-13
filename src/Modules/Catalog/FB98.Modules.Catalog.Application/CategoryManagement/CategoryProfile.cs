using FB98.Modules.Catalog.Application.CategoryManagement.Create;
using FB98.Modules.Catalog.Application.CategoryManagement.GetAll;
using FB98.Modules.Catalog.Application.CategoryManagement.GetDetail;
using FB98.Modules.Catalog.Application.CategoryManagement.Update;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.CategoryManagement
{
	internal class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<CreateCategoryDto, Category>();
			CreateMap<UpdateCategoryDto, Category>();
			CreateMap<Category, GetDetailCategoryResponse>();
			CreateMap<Category, GetAllCategoryResponse>();
		}
	}
}
