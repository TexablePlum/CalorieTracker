
namespace CalorieTracker.Application.Auth.Queries
{
	/// <summary>
	/// Query do pobrania produktu po kodzie kreskowym
	/// </summary>
	public record GetProductByBarcodeQuery(string Barcode);
}
