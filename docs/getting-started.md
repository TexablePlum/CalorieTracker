# Getting Started

## Wymagania

- .NET 9 SDK
- SQL Server (LocalDB lub pełna wersja)
- Visual Studio 2022 lub VS Code

## Konfiguracja projektu

### 1. Klonowanie repozytorium

```bash
git clone <repository-url>
cd CalorieTracker
```

### 2. Konfiguracja bazy danych

Zaktualizuj connection string w `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CalorieTracker;Trusted_Connection=true;"
  }
}
```

### 3. Migracje bazy danych

```bash
cd src/CalorieTracker.Api
dotnet ef database update
```

### 4. Konfiguracja email (opcjonalna)

```json
{
  "Email": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your-email@gmail.com",
    "Password": "your-password",
    "EnableSsl": true,
    "From": "your-email@gmail.com"
  }
}
```

### 5. Konfiguracja JWT

```json
{
  "Jwt": {
    "Key": "your-super-secret-key-min-32-characters",
    "Issuer": "CalorieTracker",
    "Audience": "CalorieTracker-Users"
  }
}
```

## Uruchomienie aplikacji

```bash
cd src/CalorieTracker.Api
dotnet run
```

Aplikacja będzie dostępna pod adresem: `https://localhost:5001`

## Swagger UI

Dokumentacja API dostępna pod: `https://localhost:5001/swagger`

## Uruchamianie testów

```bash
# Wszystkie testy
dotnet test

# Testy z pokryciem kodu
dotnet test --collect:"XPlat Code Coverage"

# Konkretna klasa testów
dotnet test --filter "RecipeNutritionCalculatorTests"
```

## Struktura projektu

```
src/
├── CalorieTracker.Api/          # REST API controllers
├── CalorieTracker.Application/  # Business logic (CQRS)
├── CalorieTracker.Domain/       # Core business entities
├── CalorieTracker.Infrastructure/ # External concerns
└── CalorieTracker.Tests/        # Unit tests
```

## Pierwsze kroki

1. **Zarejestruj użytkownika** - `POST /api/auth/register`
2. **Potwierdź email** - `POST /api/auth/confirm`
3. **Zaloguj się** - `POST /api/auth/login`
4. **Uzupełnij profil** - `PUT /api/profile`
5. **Dodaj produkty** - `POST /api/products`
6. **Utwórz przepisy** - `POST /api/recipes`

## Użyteczne komendy

```bash
# Build całego solution
dotnet build

# Uruchom z watch mode
dotnet watch run

# Generuj migracje
dotnet ef migrations add <MigrationName>

# Aktualizuj bazę danych
dotnet ef database update

# Uruchom tylko API
cd src/CalorieTracker.Api && dotnet run
```

## Rozwiązywanie problemów

### Błąd połączenia z bazą danych
- Sprawdź czy SQL Server jest uruchomiony
- Zweryfikuj connection string
- Uruchom migracje: `dotnet ef database update`

### Błędy JWT
- Sprawdź konfigurację klucza JWT (min. 32 znaki)
- Upewnij się, że Issuer i Audience są poprawne

### Problemy z emailem
- Sprawdź ustawienia SMTP
- Dla Gmail użyj App Password zamiast zwykłego hasła