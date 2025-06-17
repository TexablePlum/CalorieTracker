# API Reference

Kompletna dokumentacja techniczna wszystkich modułów aplikacji CalorieTracker.

## 📋 Przegląd modułów

### 🌐 CalorieTracker.Api
**Warstwa prezentacji i kontrolery REST API**

Przeglądaj dokumentację w nawigacji po lewej stronie lub wyszukaj konkretne klasy:
- `AuthController` - Uwierzytelnianie i autoryzacja
- `ProductsController` - Zarządzanie produktami spożywczymi
- `RecipesController` - Operacje na przepisach kulinarnych
- `WeightMeasurementsController` - Pomiary masy ciała

**Zawiera:**
- **Controllers** - Endpointy HTTP
- **Models** - DTO dla żądań i odpowiedzi API
- **Validation** - Walidatory FluentValidation 
- **Mapping** - Profile AutoMapper
- **Attributes** - Custom atrybuty autoryzacji

### ⚙️ CalorieTracker.Application
**Logika biznesowa i wzorzec CQRS**

Główne przestrzenie nazw:
- `Auth.Handlers` - Obsługa uwierzytelniania
- `Products.Commands` - Zarządzanie produktami
- `Recipes.Commands` - Operacje na przepisach
- `WeightMeasurements.Commands` - Pomiary wagi

**Zawiera:**
- **Commands** - Operacje modyfikujące stan systemu
- **Queries** - Operacje odczytu danych
- **Handlers** - Implementacje logiki biznesowej
- **Interfaces** - Kontrakty dla warstwy Infrastructure

### 🏢 CalorieTracker.Domain
**Rdzeń biznesowy aplikacji**

Kluczowe klasy:
- `Product` - Produkt spożywczy z wartościami odżywczymi
- `Recipe` - Przepis kulinarny ze składnikami
- `WeightMeasurement` - Pomiar masy ciała użytkownika
- `RecipeNutritionCalculator` - Kalkulacja wartości odżywczych

**Zawiera:**
- **Entities** - Encje biznesowe
- **Enums** - Typy wyliczeniowe
- **Services** - Serwisy domenowe z logiką biznesową
- **ValueObjects** - Obiekty wartości

### 🔧 CalorieTracker.Infrastructure
**Implementacje techniczne i integracje**

Kluczowe klasy:
- `AppDbContext` - Główny kontekst Entity Framework
- `JwtGenerator` - Generator tokenów uwierzytelniających
- `EmailSender` - Serwis wysyłki emaili

**Zawiera:**
- **Data** - Entity Framework, kontekst bazy danych
- **Auth** - Generacja tokenów JWT
- **Email** - Wysyłka wiadomości email

### 🧪 CalorieTracker.Tests
**Testy jednostkowe i jakość kodu**

Kluczowe klasy testowe:
- `RecipeNutritionCalculatorTests` - Testy kalkulacji wartości odżywczych
- `WeightAnalysisServiceTests` - Testy analizy masy ciała

**Zawiera:**
- **Services** - Testy serwisów domenowych
- **Unit Tests** - Komprehensywne testy jednostkowe

## 🔍 Jak korzystać z dokumentacji

### Nawigacja
- **Lewa sidebar** - Przeglądaj moduły i przestrzenie nazw
- **Search box** - Wyszukaj konkretne klasy lub metody
- **Breadcrumbs** - Śledź swoją lokalizację w dokumentacji

### Przykłady użycia

```csharp
// Przykład: Kalkulacja wartości odżywczych przepisu
var calculator = new RecipeNutritionCalculator();
var nutrition = calculator.CalculateForRecipe(recipe);
Console.WriteLine($"Kalorie: {nutrition.Calories} kcal");
// Przykład: Analiza BMI
var weightService = new WeightAnalysisService();
var bmi = weightService.CalculateBMI(weightKg: 70, heightCm: 175);
Console.WriteLine($"BMI: {bmi}");
```

## 📚 Dodatkowe zasoby

- [Getting Started](../getting-started.md) - Przewodnik dla deweloperów
- [Architecture](../architecture.md) - Szczegóły architektury aplikacji

## 🔄 Aktualizacje

Ta dokumentacja jest generowana automatycznie z komentarzy XML w kodzie źródłowym.

---

**💡 Wskazówka:** Użyj nawigacji po lewej stronie, aby przeglądać szczegółową dokumentację każdego modułu, lub skorzystaj z wyszukiwarki, aby znaleźć konkretne klasy.