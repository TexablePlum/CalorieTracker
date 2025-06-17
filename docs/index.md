# CalorieTracker

Kompleksowa aplikacja do zarządzania dietą i monitorowania masy ciała.

## 🚀 Funkcjonalności

- **Zarządzanie produktami** - Dodawanie i edycja produktów spożywczych z pełnymi wartościami odżywczymi
- **Przepisy kulinarne** - Tworzenie przepisów z automatyczną kalkulacją wartości odżywczych
- **Monitoring wagi** - Śledzenie masy ciała z analizą BMI i postępu
- **Profile użytkowników** - Personalizowane cele i preferencje żywieniowe
- **API REST** - Pełne API do integracji z aplikacjami klienckimi

## 🏗️ Architektura

Aplikacja wykorzystuje **Clean Architecture** z podziałem na warstwy:

- **Domain** - Encje biznesowe, value objects, serwisy domenowe
- **Application** - Logika biznesowa, CQRS, handlers
- **Infrastructure** - Implementacje zewnętrzne (baza danych, email, JWT)
- **API** - Kontrolery REST, walidacja, mapowanie

## 🧪 Jakość kodu

- **Testy jednostkowe** - Pokrycie kluczowej logiki biznesowej
- **Wzorce projektowe** - Repository, CQRS, Dependency Injection
- **Walidacja** - FluentValidation dla wszystkich endpoint-ów
- **Dokumentacja** - Automatyczna generacja dokumentacji API

## 📖 Dokumentacja

- [API Reference](api/index.md) - Szczegółowa dokumentacja wszystkich klas i metod
- [Architektura](architecture.md) - Opis architektury i wzorców
- [Getting Started](getting-started.md) - Przewodnik dla deweloperów

## 🛠️ Technologie

- **.NET 9** - Framework aplikacji
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **Identity** - Uwierzytelnianie i autoryzacja
- **JWT** - Tokeny dostępu
- **xUnit** - Testy jednostkowe
- **FluentValidation** - Walidacja danych
- **AutoMapper** - Mapowanie obiektów

## 🚀 Szybki start

```bash
# Klonowanie repozytorium
git clone <repository-url>
cd CalorieTracker

# Konfiguracja bazy danych
cd src/CalorieTracker.Api
dotnet ef database update

# Uruchomienie aplikacji
dotnet run