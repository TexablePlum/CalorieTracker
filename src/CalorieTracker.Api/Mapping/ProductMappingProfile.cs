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
	/// </summary>
	public class ProductMappingProfile : Profile
	{
		/// <summary>
		/// Inicjalizuje nową instancję profilu mapowania dla produktów.
		/// Konfiguruje wszystkie mapowania wymagane do przetwarzania operacji na produktach.
		/// </summary>
		public ProductMappingProfile()
		{
			/// <summary>
			/// Mapowanie z <see cref="CreateProductRequest"/> na <see cref="CreateProductCommand"/>.
			/// Transformuje model żądania utworzenia produktu na odpowiednią komendę.
			/// Automatycznie konwertuje stringowe wartości Category i Unit na odpowiednie typy enum.
			/// </summary>
			CreateMap<CreateProductRequest, CreateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));

			/// <summary>
			/// Mapowanie z <see cref="UpdateProductRequest"/> na <see cref="UpdateProductCommand"/>.
			/// Transformuje model żądania aktualizacji produktu na odpowiednią komendę.
			/// Automatycznie konwertuje stringowe wartości Category i Unit na odpowiednie typy enum.
			/// </summary>
			CreateMap<UpdateProductRequest, UpdateProductCommand>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					Enum.Parse<ProductCategory>(src.Category, true)))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src =>
					Enum.Parse<ProductUnit>(src.Unit, true)));

			/// <summary>
			/// Mapowanie z <see cref="SearchProductsRequest"/> na <see cref="SearchProductsQuery"/>.
			/// Transformuje parametry wyszukiwania produktów na odpowiednie zapytanie.
			/// Obsługuje konwersję opcjonalnego parametru kategorii (może być null).
			/// </summary>
			CreateMap<SearchProductsRequest, SearchProductsQuery>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
					string.IsNullOrEmpty(src.Category) ? (ProductCategory?)null :
					Enum.Parse<ProductCategory>(src.Category, true)));

			/// <summary>
			/// Mapowanie z encji <see cref="Product"/> na model <see cref="ProductDto"/>.
			/// Transformuje pełne dane produktu na format odpowiedzi API.
			/// Konwertuje enumy Category i Unit na ich stringowe reprezentacje.
			/// </summary>
			CreateMap<Product, ProductDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));

			/// <summary>
			/// Mapowanie z encji <see cref="Product"/> na model <see cref="ProductSummaryDto"/>.
			/// Transformuje podstawowe dane produktu na uproszczony format odpowiedzi API.
			/// Konwertuje enumy Category i Unit na ich stringowe reprezentacje.
			/// </summary>
			CreateMap<Product, ProductSummaryDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
				.ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit.ToString()));
		}
	}
}