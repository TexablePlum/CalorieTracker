using AutoMapper;
using CalorieTracker.Api.Attributes;
using CalorieTracker.Api.Models.Products;
using CalorieTracker.Application.Auth.Commands;
using CalorieTracker.Application.Auth.Handlers;
using CalorieTracker.Application.Auth.Queries;
using CalorieTracker.Application.Products.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CalorieTracker.Api.Controllers
{
	/// <summary>
	/// Kontroler do zarządzania produktami spożywczymi
	/// </summary>
	[Authorize]
	[RequireCompleteProfile]
	[ApiController]
	[Route("api/[controller]")]
	public class ProductsController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly CreateProductHandler _createHandler;
		private readonly UpdateProductHandler _updateHandler;
		private readonly DeleteProductHandler _deleteHandler;
		private readonly SearchProductsHandler _searchHandler;
		private readonly GetProductByIdHandler _getByIdHandler;
		private readonly GetProductByBarcodeHandler _getBarcodeHandler;
		private readonly GetUserProductsHandler _getUserProductsHandler;

		public ProductsController(
			IMapper mapper,
			CreateProductHandler createHandler,
			UpdateProductHandler updateHandler,
			DeleteProductHandler deleteHandler,
			SearchProductsHandler searchHandler,
			GetProductByIdHandler getByIdHandler,
			GetProductByBarcodeHandler getBarcodeHandler,
			GetUserProductsHandler getUserProductsHandler)
		{
			_mapper = mapper;
			_createHandler = createHandler;
			_updateHandler = updateHandler;
			_deleteHandler = deleteHandler;
			_searchHandler = searchHandler;
			_getByIdHandler = getByIdHandler;
			_getBarcodeHandler = getBarcodeHandler;
			_getUserProductsHandler = getUserProductsHandler;
		}

		/// <summary>
		/// Wyszukuje produkty po nazwie z opcjonalnym filtrowaniem po kategorii
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<SearchProductsResponse>> SearchProducts([FromQuery] SearchProductsRequest request)
		{
			var query = _mapper.Map<SearchProductsQuery>(request);
			var products = await _searchHandler.Handle(query);

			var productDtos = _mapper.Map<List<ProductSummaryDto>>(products);

			return Ok(new SearchProductsResponse
			{
				Products = productDtos,
				TotalCount = productDtos.Count,
				HasMore = productDtos.Count == request.Take
			});
		}

		/// <summary>
		/// Pobiera szczegóły produktu po ID
		/// </summary>
		[HttpGet("{id:guid}")]
		public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
		{
			var product = await _getByIdHandler.Handle(new GetProductByIdQuery(id));

			if (product is null)
				return NotFound("Produkt nie został znaleziony");

			var productDto = _mapper.Map<ProductDto>(product);
			return Ok(productDto);
		}

		/// <summary>
		/// Pobiera produkt po kodzie kreskowym
		/// </summary>
		[HttpGet("barcode/{barcode}")]
		public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
		{
			var product = await _getBarcodeHandler.Handle(new GetProductByBarcodeQuery(barcode));

			if (product is null)
				return NotFound("Produkt o tym kodzie kreskowym nie został znaleziony");

			var productDto = _mapper.Map<ProductDto>(product);
			return Ok(productDto);
		}

		/// <summary>
		/// Pobiera produkty utworzone przez aktualnie zalogowanego użytkownika
		/// </summary>
		[HttpGet("my-products")]
		public async Task<ActionResult<List<ProductSummaryDto>>> GetMyProducts([FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var query = new GetUserProductsQuery { UserId = userId, Skip = skip, Take = take };
			var products = await _getUserProductsHandler.Handle(query);

			var productDtos = _mapper.Map<List<ProductSummaryDto>>(products);
			return Ok(productDtos);
		}

		/// <summary>
		/// Tworzy nowy produkt
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<CreateProductCommand>(request);
			command = command with { CreatedByUserId = userId };

			var productId = await _createHandler.Handle(command);

			return CreatedAtAction(
				nameof(GetProduct),
				new { id = productId },
				new { id = productId });
		}

		/// <summary>
		/// Aktualizuje istniejący produkt (tylko właściciel może edytować)
		/// </summary>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var command = _mapper.Map<UpdateProductCommand>(request);
			command = command with { Id = id, UpdatedByUserId = userId };

			var success = await _updateHandler.Handle(command);

			if (!success)
				return NotFound("Produkt nie został znaleziony lub nie masz uprawnień do jego edycji");

			return NoContent();
		}

		/// <summary>
		/// Usuwa produkt (tylko właściciel może usunąć)
		/// </summary>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> DeleteProduct(Guid id)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId is null) return Unauthorized();

			var success = await _deleteHandler.Handle(new DeleteProductCommand(id, userId));

			if (!success)
				return NotFound("Produkt nie został znaleziony lub nie masz uprawnień do jego usunięcia");

			return NoContent();
		}
	}
}