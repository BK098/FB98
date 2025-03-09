using FB98.Modules.Catalog.Application.ProductManagement.Create;
using FB98.Modules.Catalog.Application.ProductManagement.GetAll;
using FB98.Modules.Catalog.Application.ProductManagement.GetDetail;
using FB98.Modules.Catalog.Application.ProductManagement.Update;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.ProductManagement
{
	public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateProductDto, Product>()
				.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl));

			CreateMap<UpdateProductDto, Product>()
				.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl));

			CreateMap<Product, GetAllProductResponse>();

			CreateMap<Product, GetDetailProductResponse>()
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
		}
	}
}