using FB98.Modules.Catalog.Application.ComboManagement.Create;
using FB98.Modules.Catalog.Application.ComboManagement.GetAll;
using FB98.Modules.Catalog.Application.ComboManagement.GetDetail;
using FB98.Modules.Catalog.Application.ComboManagement.Update;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.ComboManagement
{
	internal class ComboProfile : Profile
	{
		public ComboProfile()
		{
			Init();
		}

		private void Init()
		{
			CreateMap<CreateComboDto, Combo>()
				.ForMember(dest => dest.ComboProducts, opt => opt.MapFrom(src => src.Products));

			CreateMap<CreateComboProductDto, ComboProduct>()
				//.ForMember(src => src.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Combo, opt => opt.Ignore())
				.ForMember(dest => dest.ComboId, opt => opt.Ignore());

			CreateMap<UpdateComboDto, Combo>()
				.ForMember(dest => dest.ComboProducts, opt => opt.MapFrom(src => src.Products));

			CreateMap<UpdateComboProductDto, ComboProduct>()
				//.ForMember(src => src.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(src => src.Combo, opt => opt.Ignore())
				.ForMember(dest => dest.ComboId, opt => opt.Ignore());


			CreateMap<Combo, GetAllComboResponse>();
			CreateMap<Combo, GetDetailComboResponse>()
				.ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ComboProducts));
			CreateMap<ComboProduct, GetDetailComboProductResponse>()
				.ForMember(src => src.Id, opt => opt.MapFrom(src => src.ProductId))
				.ForMember(src => src.Name, opt => opt.MapFrom(src => src.Product.Name))
				.ForMember(src => src.Image, opt => opt.MapFrom(src => src.Product.Image))
				.ForMember(src => src.Price, opt => opt.MapFrom(src => src.Product.Price));
		}
	}
}