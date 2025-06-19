// Plik MealLogEntry.cs - encja wpisu posiłku w dzienniku żywieniowym.
// Reprezentuje pojedynczy zalogowany posiłek z powiązaniem do produktu lub przepisu.

using CalorieTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CalorieTracker.Domain.Entities
{
	/// <summary>
	/// Encja reprezentująca pojedynczy wpis posiłku w dzienniku żywieniowym użytkownika.
	/// Może być powiązana z produktem lub przepisem, zawiera informacje o ilości i czasie spożycia.
	/// Stanowi podstawę do kalkulacji dziennego spożycia wartości odżywczych.
	/// </summary>
	public class MealLogEntry
	{
		/// <summary>
		/// Unikalny identyfikator GUID wpisu posiłku.
		/// Jest kluczem głównym encji.
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Identyfikator użytkownika, którego dotyczy ten wpis posiłku.
		/// Pole wymagane.
		/// </summary>
		[Required]
		public string UserId { get; set; } = null!;

		/// <summary>
		/// Identyfikator produktu spożywczego (opcjonalny).
		/// Pole wzajemnie wykluczające się z RecipeId - jeden z nich musi być wypełniony.
		/// </summary>
		public Guid? ProductId { get; set; }

		/// <summary>
		/// Identyfikator przepisu kulinarnego (opcjonalny).
		/// Pole wzajemnie wykluczające się z ProductId - jeden z nich musi być wypełniony.
		/// </summary>
		public Guid? RecipeId { get; set; }

		/// <summary>
		/// Ilość spożytego produktu/przepisu w jednostkach bazowych.
		/// Dla produktów: gramy/mililitry/sztuki zgodnie z Product.Unit.
		/// Dla przepisów: zawsze gramy (waga całkowitej porcji).
		/// </summary>
		[Required]
		[Range(0.1, 10000)]
		public float Quantity { get; set; }

		/// <summary>
		/// Typ posiłku określający kategorię czasową spożycia.
		/// </summary>
		[Required]
		public MealType MealType { get; set; }

		/// <summary>
		/// Data i czas rzeczywistego spożycia posiłku.
		/// Domyślnie ustawiane na czas utworzenia wpisu.
		/// </summary>
		[Required]
		public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Opcjonalne notatki użytkownika dotyczące posiłku.
		/// Może zawierać informacje o sposobie przygotowania, smaku, okolicznościach spożycia.
		/// </summary>
		[MaxLength(500)]
		public string? Notes { get; set; }

		// Kalkulowane wartości odżywcze (przechowywane dla wydajności)

		/// <summary>
		/// Kalkulowana liczba kalorii dla spożytej ilości.
		/// Obliczana przy zapisie na podstawie produktu/przepisu i ilości.
		/// </summary>
		public float CalculatedCalories { get; set; }

		/// <summary>
		/// Kalkulowana ilość białka w gramach dla spożytej ilości.
		/// </summary>
		public float CalculatedProtein { get; set; }

		/// <summary>
		/// Kalkulowana ilość tłuszczu w gramach dla spożytej ilości.
		/// </summary>
		public float CalculatedFat { get; set; }

		/// <summary>
		/// Kalkulowana ilość węglowodanów w gramach dla spożytej ilości.
		/// </summary>
		public float CalculatedCarbohydrates { get; set; }

		/// <summary>
		/// Kalkulowana ilość błonnika w gramach dla spożytej ilości (opcjonalna).
		/// </summary>
		public float? CalculatedFiber { get; set; }

		/// <summary>
		/// Kalkulowana ilość cukru w gramach dla spożytej ilości (opcjonalna).
		/// </summary>
		public float? CalculatedSugar { get; set; }

		/// <summary>
		/// Kalkulowana ilość sodu w miligramach dla spożytej ilości (opcjonalna).
		/// </summary>
		public float? CalculatedSodium { get; set; }

		// Metadane

		/// <summary>
		/// Data i czas utworzenia wpisu w systemie.
		/// </summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// Data i czas ostatniej modyfikacji wpisu.
		/// </summary>
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		// Właściwości nawigacyjne

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego użytkownika.
		/// </summary>
		public ApplicationUser User { get; set; } = null!;

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego produktu (jeśli ProductId nie jest null).
		/// </summary>
		public Product? Product { get; set; }

		/// <summary>
		/// Właściwość nawigacyjna do powiązanego przepisu (jeśli RecipeId nie jest null).
		/// </summary>
		public Recipe? Recipe { get; set; }

		/// <summary>
		/// Sprawdza czy wpis posiłku jest powiązany z produktem.
		/// </summary>
		public bool IsProductBased => ProductId.HasValue;

		/// <summary>
		/// Sprawdza czy wpis posiłku jest powiązany z przepisem.
		/// </summary>
		public bool IsRecipeBased => RecipeId.HasValue;

		/// <summary>
		/// Zwraca nazwę produktu lub przepisu dla wyświetlania.
		/// </summary>
		public string GetDisplayName()
		{
			if (IsProductBased && Product != null)
				return Product.Name;

			if (IsRecipeBased && Recipe != null)
				return Recipe.Name;

			return "Nieznany posiłek";
		}

		/// <summary>
		/// Zwraca jednostkę miary dla spożytej ilości.
		/// </summary>
		public string GetQuantityUnit()
		{
			if (IsProductBased && Product != null)
			{
				return Product.Unit switch
				{
					ProductUnit.Gram => "g",
					ProductUnit.Milliliter => "ml",
					ProductUnit.Piece => "szt",
					_ => "jednostek"
				};
			}

			return "g"; // Przepisy zawsze w gramach
		}
	}
}