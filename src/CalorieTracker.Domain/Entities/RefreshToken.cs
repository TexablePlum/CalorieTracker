// Plik RefreshToken.cs - definicja encji tokena odświeżającego.
// Reprezentuje token używany do odświeżania tokenów dostępowych bez konieczności ponownego logowania.

using System;

namespace CalorieTracker.Domain.Entities;

/// &lt;summary>
/// Encja reprezentująca token odświeżający używany do utrzymania sesji użytkownika.
/// Służy do generowania nowych tokenów dostępowych po ich wygaśnięciu.
/// &lt;/summary>
public class RefreshToken
{
	/// &lt;summary>
	/// Unikalny identyfikator GUID tokena odświeżającego.
	/// &lt;/summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Ciąg znaków reprezentujący token odświeżający.
	/// Domyślnie generowany jako nowy GUID bez myślników.
	/// </summary>
	public string Token { get; set; } = Guid.NewGuid().ToString("N");

	/// <summary>
	/// Data i czas wygaśnięcia tokena odświeżającego.
	/// </summary>
	public DateTime ExpiresAt { get; set; }

	/// <summary>
	/// Wskazuje, czy token odświeżający został unieważniony.
	/// </summary>
	public bool Revoked { get; set; }

	/// <summary>
	/// Identyfikator użytkownika, do którego przypisany jest ten token odświeżający.
	/// Tworzy relację 1-N z <see cref="ApplicationUser"/>.
	/// </summary>
	public string UserId { get; set; } = null!;

	/// <summary>
	/// Właściwość nawigacyjna do powiązanego użytkownika <see cref="ApplicationUser"/>.
	/// </summary>
	public ApplicationUser User { get; set; } = null!;
}