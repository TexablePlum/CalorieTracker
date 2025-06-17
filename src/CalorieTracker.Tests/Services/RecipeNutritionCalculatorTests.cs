using CalorieTracker.Domain.Entities;
using CalorieTracker.Domain.Enums;
using CalorieTracker.Domain.Services;
using Xunit;

namespace CalorieTracker.Tests.Services
{
	/// <summary>
	/// Klasa testowa weryfikująca poprawność działania serwisu <see cref="RecipeNutritionCalculator"/>.
	/// Zawiera testy jednostkowe dla wszystkich scenariuszy kalkulacji wartości odżywczych przepisów kulinarnych,
	/// włączając różne jednostki miary produktów, wieloskładnikowe receptury i przypadki graniczne.
	/// </summary>
	/// <remarks>
	/// <para><strong>Testowane algorytmy kalkulacyjne:</strong></para>
	/// <list type="bullet">
	/// <item>Konwersja jednostek miary (gram, milliliter, sztuka) na wspólną podstawę gramową</item>
	/// <item>Proporcjonalne przeliczanie wartości odżywczych z "na 100g" na faktyczne ilości składników</item>
	/// <item>Sumowanie makroskładników i mikroelementów z wielu produktów w przepisie</item>
	/// <item>Obsługa opcjonalnych składników odżywczych (błonnik, cukier, sód) z wartościami null</item>
	/// <item>Walidacja przypadków granicznych (ilości ułamkowe, puste przepisy, skrajne wartości)</item>
	/// </list>
	/// <para><strong>Logika biznesowa testowana:</strong></para>
	/// <list type="bullet">
	/// <item>Wszystkie wartości odżywcze produktów przechowywane są w systemie "na 100g/100ml"</item>
	/// <item>Rzeczywiste kalkulacje wymagają proporcjonalnego przeliczania według faktycznych ilości</item>
	/// <item>Różne jednostki miary wymagają unifikacji do gramów przed obliczeniami</item>
	/// <item>Wartości opcjonalne muszą być propagowane prawidłowo (null + wartość = wartość)</item>
	/// </list>
	/// <para><strong>Zastosowane wzorce testowe:</strong> AAA Pattern, Builder Pattern, Theory/InlineData, Factory Methods</para>
	/// </remarks>
	public class RecipeNutritionCalculatorTests
	{
		/// <summary>
		/// Instancja testowanego kalkulatora wartości odżywczych przepisów.
		/// Serwis domenowy bezstanowy, inicjalizowany raz w konstruktorze dla wszystkich testów.
		/// Implementuje logikę biznesową konwersji jednostek i sumowania składników odżywczych.
		/// </summary>
		private readonly RecipeNutritionCalculator _calculator;

		/// <summary>
		/// Inicjalizuje nową instancję klasy testowej.
		/// Tworzy czystą instancję <see cref="RecipeNutritionCalculator"/> dla każdego zestawu testów.
		/// </summary>
		public RecipeNutritionCalculatorTests()
		{
			_calculator = new RecipeNutritionCalculator();
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji wartości odżywczych dla produktów mierzonych w gramach.
		/// Test podstawowego scenariusza z najprostszą jednostką miary - bezpośrednia konwersja 1:1.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Produkt</term>
		/// <description>Masło - 717 kcal/100g, 0.9g białka/100g, 81g tłuszczu/100g, 0.1g węglowodanów/100g</description>
		/// </item>
		/// <item>
		/// <term>Ilość użyta</term>
		/// <description>50g (połowa porcji referencyjnej)</description>
		/// </item>
		/// <item>
		/// <term>Oczekiwane kalorie</term>
		/// <description>358.5 kcal (50g × 717 kcal/100g = 358.5)</description>
		/// </item>
		/// </list>
		/// <para><strong>Weryfikowane aspekty kalkulacyjne:</strong></para>
		/// <list type="bullet">
		/// <item>Proporcjonalne przeliczanie wszystkich makroskładników</item>
		/// <item>Zachowanie precyzji obliczeń (dokładność do miejsc po przecinku)</item>
		/// <item>Poprawność wzoru: (ilość_składnika / 100g) × wartość_na_100g</item>
		/// </list>
		/// <para><strong>Reprezentatywność:</strong> Test stanowi wzorzec dla wszystkich produktów gramowych,
		/// które stanowią większość składników stałych w przepisach kulinarnych.</para>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithGramBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange - przygotowanie produktu gramowego
			var maslo = CreateProduct("Masło", ProductUnit.Gram, 717f, 0.9f, 81f, 0.1f, servingSize: 100f);
			var recipe = CreateRecipe("Test Recipe");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, maslo, 50f)); // 50g masła

			// Act - wykonanie kalkulacji wartości odżywczych
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - weryfikacja proporcjonalnych obliczeń
			Assert.Equal(358.5f, result.Calories, 1); // 50g * 717 kcal/100g = 358.5
			Assert.Equal(0.45f, result.Protein, 2);   // 50g * 0.9g/100g = 0.45g
			Assert.Equal(40.5f, result.Fat, 1);       // 50g * 81g/100g = 40.5g
			Assert.Equal(0.05f, result.Carbohydrates, 2); // 50g * 0.1g/100g = 0.05g
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji dla produktów mierzonych w sztukach.
		/// Test konwersji jednostek sztukowych na gramy przez ServingSize produktu.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Produkt</term>
		/// <description>Jajko - 155 kcal/100g, 13g białka/100g, 11g tłuszczu/100g, 1.1g węglowodanów/100g</description>
		/// </item>
		/// <item>
		/// <term>Jednostka i wielkość</term>
		/// <description>1 sztuka = 50g (ServingSize)</description>
		/// </item>
		/// <item>
		/// <term>Ilość użyta</term>
		/// <description>2 jajka = 2 × 50g = 100g składnika</description>
		/// </item>
		/// </list>
		/// <para><strong>Logika konwersji testowana:</strong></para>
		/// <list type="bullet">
		/// <item>quantity × ServingSize = masa_w_gramach</item>
		/// <item>2 sztuki × 50g/sztuka = 100g</item>
		/// <item>100g = dokładnie porcja referencyjna → pełne wartości odżywcze</item>
		/// </list>
		/// <para><strong>Przypadki użycia:</strong> Jajka, bułki, owoce, warzywa - produkty naturalnie liczone w sztukach.
		/// Kluczowe dla intuicyjnego użytkowania aplikacji przez kucharzy.</para>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithPieceBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange - jajko: 1 sztuka = 50g, 155 kcal/100g
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var recipe = CreateRecipe("Jajecznica");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 2f)); // 2 jajka

			// Act - kalkulacja z konwersją sztuk na gramy
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 2 jajka * 50g = 100g = 1 * wartości per 100g
			Assert.Equal(155f, result.Calories);
			Assert.Equal(13f, result.Protein);
			Assert.Equal(11f, result.Fat);
			Assert.Equal(1.1f, result.Carbohydrates);
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji dla produktów płynnych mierzonych w mililitrach.
		/// Test konwersji objętości na masę z założeniem gęstości zbliżonej do wody (1ml ≈ 1g).
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Produkt</term>
		/// <description>Mleko - 42 kcal/100ml, 3.4g białka/100ml, 1g tłuszczu/100ml, 4.8g węglowodanów/100ml</description>
		/// </item>
		/// <item>
		/// <term>Objętość użyta</term>
		/// <description>250ml (2.5 × porcja referencyjna 100ml)</description>
		/// </item>
		/// <item>
		/// <term>Konwersja gęstości</term>
		/// <description>250ml ≈ 250g (założenie 1:1 dla większości płynów spożywczych)</description>
		/// </item>
		/// </list>
		/// <para><strong>Weryfikowane obliczenia:</strong></para>
		/// <list type="bullet">
		/// <item>Kalorie: 250ml × 42 kcal/100ml = 105 kcal</item>
		/// <item>Białko: 250ml × 3.4g/100ml = 8.5g</item>
		/// <item>Tłuszcz: 250ml × 1g/100ml = 2.5g</item>
		/// <item>Węglowodany: 250ml × 4.8g/100ml = 12g</item>
		/// </list>
		/// <para><strong>Przypadki użycia:</strong> Mleko, soki, oleje, octy, buliony - wszystkie płyny kuchenne.
		/// Założenie gęstości 1:1 jest wystarczająco precyzyjne dla celów kulinarnych.</para>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithMilliliterBasedProduct_ShouldCalculateCorrectly()
		{
			// Arrange - mleko: 100ml, 42 kcal/100ml
			var mleko = CreateProduct("Mleko", ProductUnit.Milliliter, 42f, 3.4f, 1f, 4.8f, servingSize: 100f);
			var recipe = CreateRecipe("Koktajl");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, mleko, 250f)); // 250ml mleka

			// Act - kalkulacja z konwersją mililitrów
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 250ml * 42 kcal/100ml = 105 kcal
			Assert.Equal(105f, result.Calories);
			Assert.Equal(8.5f, result.Protein, 1);   // 250 * 3.4/100 = 8.5
			Assert.Equal(2.5f, result.Fat, 1);       // 250 * 1/100 = 2.5
			Assert.Equal(12f, result.Carbohydrates); // 250 * 4.8/100 = 12
		}

		/// <summary>
		/// Weryfikuje poprawność kalkulacji dla ułamkowych ilości składników.
		/// Test precyzji obliczeń przy niestandardowych proporcjach używanych w kuchni.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Produkt</term>
		/// <description>Jajko (jak w poprzednich testach)</description>
		/// </item>
		/// <item>
		/// <term>Ilość ułamkowa</term>
		/// <description>0.5 jajka (pół jajka - częsty przypadek w przepisach)</description>
		/// </item>
		/// <item>
		/// <term>Masa składnika</term>
		/// <description>0.5 × 50g = 25g = 0.25 porcji referencyjnej</description>
		/// </item>
		/// </list>
		/// <para><strong>Weryfikowane obliczenia ułamkowe:</strong></para>
		/// <list type="bullet">
		/// <item>Kalorie: 25g × 155 kcal/100g = 38.75 kcal</item>
		/// <item>Białko: 25g × 13g/100g = 3.25g</item>
		/// <item>Tłuszcz: 25g × 11g/100g = 2.75g</item>
		/// <item>Węglowodany: 25g × 1.1g/100g = 0.275g</item>
		/// </list>
		/// <para><strong>Znaczenie praktyczne:</strong> Przepisy często wymagają ułamków składników
		/// (pół jajka, ćwierć cebuli, 1.5 łyżki oleju). Test zapewnia precyzję dla takich przypadków.</para>
		/// <para><strong>Precyzja:</strong> Różne tolerancje dla różnych składników odpowiadają rzeczywistej
		/// dokładności pomiarów w kuchni (2-3 miejsca po przecinku).</para>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithFractionalQuantity_ShouldCalculateCorrectly()
		{
			// Arrange - pół jajka
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var recipe = CreateRecipe("Mini omlet");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 0.5f)); // 0.5 jajka

			// Act - kalkulacja ułamkowej ilości
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - 0.5 jajka * 50g = 25g = 0.25 * wartości per 100g
			Assert.Equal(38.75f, result.Calories, 2); // 25g * 155/100 = 38.75
			Assert.Equal(3.25f, result.Protein, 2);   // 25g * 13/100 = 3.25
			Assert.Equal(2.75f, result.Fat, 2);       // 25g * 11/100 = 2.75
			Assert.Equal(0.275f, result.Carbohydrates, 3); // 25g * 1.1/100 = 0.275
		}

		/// <summary>
		/// Weryfikuje poprawność sumowania wartości odżywczych z wielu różnorodnych składników.
		/// Test integracyjny sprawdzający współpracę wszystkich algorytmów konwersji i sumowania.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy - Omlet trzuskładnikowy:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Składnik 1</term>
		/// <description>2 jajka (sztuki): 2 × 50g = 100g → 155 kcal, 13g białka, 11g tłuszczu, 1.1g węglowodanów</description>
		/// </item>
		/// <item>
		/// <term>Składnik 2</term>
		/// <description>10g masła (gramy): 10g → 71.7 kcal, 0.09g białka, 8.1g tłuszczu, 0.01g węglowodanów</description>
		/// </item>
		/// <item>
		/// <term>Składnik 3</term>
		/// <description>50ml mleka (mililitry): 50ml → 21 kcal, 1.7g białka, 0.5g tłuszczu, 2.4g węglowodanów</description>
		/// </item>
		/// </list>
		/// <para><strong>Weryfikowane sumy składników:</strong></para>
		/// <list type="bullet">
		/// <item>Kalorie całkowite: 155 + 71.7 + 21 = 247.7 kcal</item>
		/// <item>Białko całkowite: 13 + 0.09 + 1.7 = 14.79g</item>
		/// <item>Tłuszcze całkowite: 11 + 8.1 + 0.5 = 19.6g</item>
		/// <item>Węglowodany całkowite: 1.1 + 0.01 + 2.4 = 3.51g</item>
		/// </list>
		/// <para><strong>Testowane aspekty integracyjne:</strong></para>
		/// <list type="bullet">
		/// <item>Równoczesna konwersja trzech różnych jednostek miary</item>
		/// <item>Prawidłowe sumowanie wszystkich makroskładników</item>
		/// <item>Zachowanie precyzji przy akumulacji błędów zaokrągleń</item>
		/// <item>Reprezentatywność rzeczywistych przepisów kulinarnych</item>
		/// </list>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithMultipleIngredients_ShouldSumCorrectly()
		{
			// Arrange - trzuskładnikowy omlet
			var jajko = CreateProduct("Jajko", ProductUnit.Piece, 155f, 13f, 11f, 1.1f, servingSize: 50f);
			var maslo = CreateProduct("Masło", ProductUnit.Gram, 717f, 0.9f, 81f, 0.1f, servingSize: 100f);
			var mleko = CreateProduct("Mleko", ProductUnit.Milliliter, 42f, 3.4f, 1f, 4.8f, servingSize: 100f);

			var recipe = CreateRecipe("Omlet");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, jajko, 2f));  // 2 jajka (100g)
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, maslo, 10f)); // 10g masła
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, mleko, 50f)); // 50ml mleka

			// Act - kalkulacja wieloskładnikowa
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - weryfikacja sum wszystkich składników
			// 2 jajka: 155 kcal + 10g masła: 71.7 kcal + 50ml mleka: 21 kcal = 247.7 kcal
			Assert.Equal(247.7f, result.Calories, 1);

			// Białko: 13g + 0.09g + 1.7g = 14.79g
			Assert.Equal(14.79f, result.Protein, 2);

			// Tłuszcze: 11g + 8.1g + 0.5g = 19.6g
			Assert.Equal(19.6f, result.Fat, 1);

			// Węglowodany: 1.1g + 0.01g + 2.4g = 3.51g
			Assert.Equal(3.51f, result.Carbohydrates, 2);
		}

		/// <summary>
		/// Weryfikuje prawidłową obsługę opcjonalnych składników odżywczych z wartościami null.
		/// Test sprawdzający logikę propagacji wartości null w sumowaniu składników.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy - Produkt z częściowymi danymi:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Składniki podstawowe</term>
		/// <description>Kalorie: 100/100g, Białko: 10g/100g, Tłuszcz: 5g/100g, Węglowodany: 15g/100g</description>
		/// </item>
		/// <item>
		/// <term>Składniki opcjonalne</term>
		/// <description>Błonnik: 2g/100g, Cukry: null, Sód: 500mg/100g</description>
		/// </item>
		/// <item>
		/// <term>Ilość użyta</term>
		/// <description>50g (połowa porcji referencyjnej)</description>
		/// </item>
		/// </list>
		/// <para><strong>Weryfikowana logika null-safety:</strong></para>
		/// <list type="bullet">
		/// <item>Wartości non-null: obliczane proporcjonalnie (błonnik: 50g × 2g/100g = 1g)</item>
		/// <item>Wartości null: pozostają null (cukry: null → null)</item>
		/// <item>Brak propagacji null na inne składniki (sód: 50g × 500mg/100g = 250mg)</item>
		/// </list>
		/// <para><strong>Przypadki użycia:</strong> Produkty z niepełnymi etykietami, składniki domowe,
		/// produkty regionalne bez pełnej analizy laboratoryjnej. System musi pozostać funkcjonalny
		/// przy częściowych danych odżywczych.</para>
		/// <para><strong>Logika biznesowa:</strong> Null oznacza "nieznane", nie "zero". 
		/// Umożliwia stopniowe uzupełnianie bazy danych produktów.</para>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithOptionalNutrients_ShouldHandleNullValues()
		{
			// Arrange - produkt z niektórymi wartościami null
			var produkt = new Product
			{
				Id = Guid.NewGuid(),
				Name = "Test Product",
				Unit = ProductUnit.Gram,
				ServingSize = 100f,
				CaloriesPer100g = 100f,
				ProteinPer100g = 10f,
				FatPer100g = 5f,
				CarbohydratesPer100g = 15f,
				FiberPer100g = 2f,
				SugarsPer100g = null,    // Testowanie wartości null
				SodiumPer100g = 500f
			};

			var recipe = CreateRecipe("Test Recipe");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, produkt, 50f)); // 50g

			// Act - kalkulacja z wartościami null
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - weryfikacja obsługi null values
			Assert.Equal(50f, result.Calories);      // 50g * 100 kcal/100g = 50 kcal
			Assert.Equal(5f, result.Protein);        // 50g * 10g/100g = 5g
			Assert.Equal(2.5f, result.Fat);          // 50g * 5g/100g = 2.5g
			Assert.Equal(7.5f, result.Carbohydrates); // 50g * 15g/100g = 7.5g
			Assert.Equal(1f, result.Fiber);          // 50g * 2g/100g = 1g
			Assert.Null(result.Sugar);               // Powinno zostać null
			Assert.Equal(250f, result.Sodium);       // 50g * 500mg/100g = 250mg
		}

		/// <summary>
		/// Weryfikuje obsługę przepisu bez składników (edge case).
		/// Test sprawdzający inicjalizację wartości domyślnych i stabilność algorytmu.
		/// </summary>
		/// <remarks>
		/// <para><strong>Scenariusz testowy:</strong></para>
		/// <list type="bullet">
		/// <item>Przepis utworzony, ale bez dodanych składników</item>
		/// <item>Lista składników pusta (Ingredients.Count = 0)</item>
		/// <item>Oczekiwane zachowanie: zwrot zerowych wartości, nie błąd</item>
		/// </list>
		/// <para><strong>Weryfikowane aspekty stabilności:</strong></para>
		/// <list type="bullet">
		/// <item>Brak rzucania wyjątków przy pustej kolekcji</item>
		/// <item>Poprawna inicjalizacja wszystkich składników odżywczych na zero</item>
		/// <item>Wartości opcjonalne ustawione na null (nie zero)</item>
		/// <item>Gotowość do dodawania składników w przyszłości</item>
		/// </list>
		/// <para><strong>Przypadki użycia w UI:</strong></para>
		/// <list type="bullet">
		/// <item>Nowy przepis w trakcie tworzenia (draft state)</item>
		/// <item>Przepis po usunięciu wszystkich składników</item>
		/// <item>Szablon przepisu do wypełnienia</item>
		/// <item>Błędne stany aplikacji (network issues, data corruption)</item>
		/// </list>
		/// </remarks>
		[Fact]
		public void CalculateForRecipe_WithEmptyIngredients_ShouldReturnZeroValues()
		{
			// Arrange - przepis bez składników
			var recipe = CreateRecipe("Empty Recipe");
			// Brak składników - lista pozostaje pusta

			// Act - kalkulacja pustego przepisu
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - weryfikacja wartości zerowych
			Assert.Equal(0f, result.Calories);
			Assert.Equal(0f, result.Protein);
			Assert.Equal(0f, result.Fat);
			Assert.Equal(0f, result.Carbohydrates);
			Assert.Null(result.Fiber);    // Opcjonalne składniki = null
			Assert.Null(result.Sugar);    // Opcjonalne składniki = null
			Assert.Null(result.Sodium);   // Opcjonalne składniki = null
		}

		/// <summary>
		/// Weryfikuje poprawność konwersji różnych jednostek miary na gramy.
		/// Test parametryzowany sprawdzający wszystkie obsługiwane jednostki produktów oraz logikę konwersji.
		/// </summary>
		/// <param name="unit">Jednostka miary produktu testowego.</param>
		/// <param name="quantity">Ilość produktu w jego naturalnej jednostce.</param>
		/// <param name="expectedGrams">Oczekiwana masa w gramach po konwersji.</param>
		/// <remarks>
		/// <para><strong>Testowane algorytmy konwersji:</strong></para>
		/// <list type="table">
		/// <item>
		/// <term>Gram → Gram</term>
		/// <description>100g = 100g (konwersja 1:1, brak transformacji)</description>
		/// </item>
		/// <item>
		/// <term>Milliliter → Gram</term>
		/// <description>200ml = 200g (założenie gęstości ρ ≈ 1g/ml)</description>
		/// </item>
		/// <item>
		/// <term>Piece → Gram</term>
		/// <description>3 sztuki × 50g/sztuka = 150g (quantity × ServingSize)</description>
		/// </item>
		/// </list>
		/// <para><strong>Logika biznesowa konwersji:</strong></para>
		/// <list type="bullet">
		/// <item>Wszystkie kalkulacje wewnętrznie operują na gramach dla spójności</item>
		/// <item>ServingSize definiuje masę jednej sztuki dla produktów liczonych</item>
		/// <item>Gęstość 1:1 jest przybliżeniem wystarczającym dla większości płynów kuchennych</item>
		/// <item>Konwersja musi być deterministyczna i odwracalna</item>
		/// </list>
		/// <para><strong>Walidacja przez kalorie:</strong> Test sprawdza konwersję pośrednio przez kalkulację kalorii,
		/// która jest proporcjonalna do masy (expectedGrams × 100 kcal/100g = expectedGrams kcal).</para>
		/// </remarks>
		[Theory]
		[InlineData(ProductUnit.Gram, 100f, 100f)]    // 100g produktu = 100g
		[InlineData(ProductUnit.Milliliter, 200f, 200f)] // 200ml produktu = 200g (1:1)
		[InlineData(ProductUnit.Piece, 3f, 150f)]     // 3 sztuki po 50g = 150g
		public void ConvertToGrams_ShouldCalculateCorrectWeight(ProductUnit unit, float quantity, float expectedGrams)
		{
			// Arrange - produkt testowy z zadaną jednostką
			var product = CreateProduct("Test", unit, 100f, 10f, 5f, 15f, servingSize: 50f);
			var recipe = CreateRecipe("Test");
			recipe.Ingredients.Add(CreateIngredient(recipe.Id, product, quantity));

			// Act - kalkulacja z konwersją jednostek
			var result = _calculator.CalculateForRecipe(recipe);

			// Assert - sprawdza czy kalorie są proporcjonalne do masy
			var expectedCalories = expectedGrams * 100f / 100f; // 100 kcal/100g
			Assert.Equal(expectedCalories, result.Calories);
		}

		#region Helper Methods

		/// <summary>
		/// Factory method tworząca instancję produktu spożywczego do celów testowych.
		/// Generuje kompletny obiekt <see cref="Product"/> z określonymi wartościami odżywczymi.
		/// </summary>
		/// <param name="name">Nazwa produktu wyświetlana w testach.</param>
		/// <param name="unit">Jednostka miary produktu (<see cref="ProductUnit"/>).</param>
		/// <param name="calories">Kalorie na 100g/100ml produktu.</param>
		/// <param name="protein">Białko w gramach na 100g/100ml produktu.</param>
		/// <param name="fat">Tłuszcz w gramach na 100g/100ml produktu.</param>
		/// <param name="carbs">Węglowodany w gramach na 100g/100ml produktu.</param>
		/// <param name="servingSize">Wielkość porcji referencyjnej (domyślnie 100g).</param>
		/// <returns>Instancja <see cref="Product"/> gotowa do użycia w testach.</returns>
		/// <remarks>
		/// <para><strong>Automatycznie generowane wartości:</strong></para>
		/// <list type="bullet">
		/// <item>ID: Nowy GUID dla każdego wywołania</item>
		/// <item>ServingSize: Kluczowa dla konwersji jednostek sztukowych</item>
		/// <item>Opcjonalne składniki: Domyślnie null (nie ustawiane)</item>
		/// </list>
		/// <para><strong>Zastosowanie w testach:</strong> Umożliwia szybkie tworzenie produktów testowych
		/// z kontrolowanymi wartościami odżywczymi, zapewniając przewidywalne wyniki kalkulacji.</para>
		/// </remarks>
		private Product CreateProduct(string name, ProductUnit unit, float calories, float protein,
			float fat, float carbs, float servingSize = 100f)
		{
			return new Product
			{
				Id = Guid.NewGuid(),
				Name = name,
				Unit = unit,
				ServingSize = servingSize,
				CaloriesPer100g = calories,
				ProteinPer100g = protein,
				FatPer100g = fat,
				CarbohydratesPer100g = carbs
			};
		}

		/// <summary>
		/// Factory method tworząca instancję przepisu kulinarnego do celów testowych.
		/// Generuje minimalny obiekt <see cref="Recipe"/> z domyślnymi wartościami testowymi.
		/// </summary>
		/// <param name="name">Nazwa przepisu wyświetlana w testach.</param>
		/// <returns>Instancja <see cref="Recipe"/> z pustą listą składników gotową do testów.</returns>
		/// <remarks>
		/// <para><strong>Domyślne wartości testowe:</strong></para>
		/// <list type="bullet">
		/// <item>ID: Nowy GUID dla każdego wywołania</item>
		/// <item>Instructions: "Test instructions" (generyczne dla testów)</item>
		/// <item>ServingsCount: 1 (pojedyncza porcja dla prostoty kalkulacji)</item>
		/// <item>TotalWeightGrams: 100g (round number dla łatwych obliczeń)</item>
		/// <item>PreparationTimeMinutes: 10 min (wartość neutralna)</item>
		/// <item>CreatedByUserId: "test-user" (stały identyfikator testowy)</item>
		/// <item>Ingredients: Pusta lista (wypełniana w testach)</item>
		/// </list>
		/// <para><strong>Pattern Builder:</strong> Metoda implementuje wzorzec Builder dla obiektów testowych,
		/// umożliwiając łatwe tworzenie przepisów z minimalnymi danymi wymaganymi przez testy.</para>
		/// </remarks>
		private Recipe CreateRecipe(string name)
		{
			return new Recipe
			{
				Id = Guid.NewGuid(),
				Name = name,
				Instructions = "Test instructions",
				ServingsCount = 1,
				TotalWeightGrams = 100f,
				PreparationTimeMinutes = 10,
				CreatedByUserId = "test-user",
				Ingredients = new List<RecipeIngredient>()
			};
		}

		/// <summary>
		/// Factory method tworząca instancję składnika przepisu do celów testowych.
		/// Łączy produkt z przepisem poprzez określenie ilości użytego składnika.
		/// </summary>
		/// <param name="recipeId">Identyfikator przepisu, do którego należy składnik.</param>
		/// <param name="product">Obiekt produktu użytego jako składnik.</param>
		/// <param name="quantity">Ilość produktu w jego naturalnej jednostce miary.</param>
		/// <returns>Instancja <see cref="RecipeIngredient"/> łącząca przepis z produktem.</returns>
		/// <remarks>
		/// <para><strong>Relacje encji:</strong></para>
		/// <list type="bullet">
		/// <item>RecipeId: Foreign Key do tabeli Recipes</item>
		/// <item>ProductId: Foreign Key do tabeli Products</item>
		/// <item>Product: Navigation Property dla łatwego dostępu w testach</item>
		/// <item>Quantity: Kluczowa wartość dla kalkulacji proporcjonalnych</item>
		/// </list>
		/// <para><strong>Zastosowanie w testach:</strong> Umożliwia szybkie tworzenie powiązań
		/// produkt-przepis z kontrolowanymi ilościami, co jest kluczowe dla testowania
		/// algorytmów kalkulacji wartości odżywczych.</para>
		/// <para><strong>Wzorzec Association Object:</strong> RecipeIngredient implementuje wzorzec
		/// Association Object, przechowując dodatkowe informacje (quantity) o relacji Many-to-Many.</para>
		/// </remarks>
		private RecipeIngredient CreateIngredient(Guid recipeId, Product product, float quantity)
		{
			return new RecipeIngredient
			{
				Id = Guid.NewGuid(),
				RecipeId = recipeId,
				ProductId = product.Id,
				Quantity = quantity,
				Product = product
			};
		}

		#endregion
	}
}
