// Plik ProductMappingProfile.cs - konfiguracja mapowania obiektów dla modułu produktów.
// Odpowiada za transformację między modelami API, komendami/zapytaniami aplikacji a encjami domenowymi.
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
	/// Klasa profilu AutoMappera dla operacji związanych z produktami.
	/// Definiuje mapowania między modelami API a obiektami komend, zapytań i encji domenowych.
	/// Obejmuje konwersje enum-string oraz transformacje między różnymi reprezentacjami produktów.
	/// </summary>
	public class ProductMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla produktów.
		/// Konfiguruje mapowania dla:
		/// - CreateProductRequest -> CreateProductCommand (z konwersją enum)
		/// - UpdateProductRequest -> UpdateProductCommand (z konwersją enum)
		/// - SearchProductsRequest -> SearchProductsQuery (z obsługą null)
		/// - Product -> ProductDto (enum na string)
		/// - Product -> ProductSummaryDto (enum na string)
		/// </summary>
		public ProductMappingProfile()
		{
			ConfigureCreateProductMapping();
			ConfigureUpdateProductMapping();
			ConfigureSearchProductMapping();
			ConfigureProductToDto();
			ConfigureProductToSummaryDto();
		}

		/// <summary>
		/// Konfiguruje mapowanie z CreateProductRequest na CreateProductCommand.
		/// Automatycznie konwertuje stringowe wartości Category i Unit na odpowiednie typy enum.
		/// </summary>
		private void ConfigureCreateProductMapping()
		{
			CreateMap<CreateProductRequest, CreateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));
		}

		/// <summary>
		/// Konfiguruje mapowanie z UpdateProductRequest na UpdateProductCommand.
		/// Automatycznie konwertuje stringowe wartości Category i Unit na odpowiednie typy enum.
		/// </summary>
		private void ConfigureUpdateProductMapping()
		{
			CreateMap<UpdateProductRequest, UpdateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));
		}

		/// <summary>
		/// Konfiguruje mapowanie z SearchProductsRequest na SearchProductsQuery.
		/// Obsługuje konwersję opcjonalnego parametru kategorii (może być null).
		/// </summary>
		private void ConfigureSearchProductMapping()
		{
			CreateMap<SearchProductsRequest, SearchProductsQuery>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Category) ? (ProductCategory?)null :
					Enum.Parse<ProductCategory>(src.Category, true)));
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji Product na model ProductDto.
		/// Konwertuje enumy Category i Unit na ich stringowe reprezentacje.
		/// </summary>
		private void ConfigureProductToDto()
		{
			CreateMap<Product, ProductDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));
		}

		/// <summary>
		/// Konfiguruje mapowanie z encji Product na model ProductSummaryDto.
		/// Konwertuje enumy Category i Unit na ich stringowe reprezentacje dla uproszczonej reprezentacji.
		/// </summary>
		private void ConfigureProductToSummaryDto()
		{
			CreateMap<Product, ProductSummaryDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));
		}
	}
}