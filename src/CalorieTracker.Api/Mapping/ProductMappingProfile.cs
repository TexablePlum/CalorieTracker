using AutoMapper;
using CalorieTracker.Api.Models.Products;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Products.Commands;
using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;

namespace CalorieTracker.Api.Mapping
{
	/// <summary>
	/// Profil AutoMapper dla mapowania obiektów związanych z produktami
	/// </summary>
	public class ProductMappingProfile : Profile
	{
		public ProductMappingProfile()
		{
			// Request -> Command mappings
			CreateMap<CreateProductRequest, CreateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));

			CreateMap<UpdateProductRequest, UpdateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));

			CreateMap<SearchProductsRequest, SearchProductsQuery>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Category) ? (ProductCategory?)null :
					Enum.Parse<ProductCategory>(src.Category, true)));

			// Entity -> DTO mappings
			CreateMap<Product, ProductDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));

			CreateMap<Product, ProductSummaryDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));
		}
	}
}