# CalorieTracker API

A REST backend for nutrition tracking and body-weight management, built with ASP.NET Core 9 on a clean, layered architecture. It derives a user's daily energy and macronutrient targets from their profile and tracks meals, water, weight, products, and recipes against those targets with all dietary math implemented as dependency-free domain logic.

![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT_%2B_Identity-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)
![Swagger](https://img.shields.io/badge/API-OpenAPI-85EA2D?style=flat-square&logo=swagger&logoColor=black)
![xUnit](https://img.shields.io/badge/Tests-xUnit-512BD4?style=flat-square)
![Azure](https://img.shields.io/badge/Deploy-Azure_App_Service-0078D4?style=flat-square&logo=microsoftazure&logoColor=white)
![License: MIT](https://img.shields.io/badge/License-MIT-green?style=flat-square)

**[Live API & Swagger UI](https://ct-backend-texableplum.azurewebsites.net/index.html)** · **[API Reference (DocFX)](https://texableplum.github.io/CalorieTracker/)**

---

## Overview

CalorieTracker turns a user profile into a personalized nutrition plan and then tracks progress against it:

1. **Register** and confirm the account with a one-time email code.
2. **Complete the profile** - age, gender, height, weight, target weight, activity level, goal, weekly pace, and a meal plan.
3. From that profile the API computes **daily calorie, macronutrient, and water targets**.
4. **Log** meals, water, and weight; query daily progress, history, and trends.
5. Browse a shared **product catalog**, add custom products, and compose **recipes** whose nutrition is calculated from their ingredients.

Most endpoints sit behind a `RequireCompleteProfile` filter: until the profile holds everything the nutrition formulas need, the API answers `403 ProfileIncomplete`. This keeps the calculation layer total - it never has to defend against partial input at request time.

## Features

- **Authentication & accounts:** ASP.NET Core Identity with JWT access tokens and rotating refresh tokens, email confirmation, password reset, resend throttling, account lockout (5 attempts / 15 min), and an enforced password policy.
- **Nutrition engine:** BMR, TDEE, goal-adjusted calories, protein/fat/carbohydrate ranges, and a hydration target, derived purely from the profile.
- **Meal & water logging:** log products or recipes per meal type, with per-entry macro snapshots; daily progress and historical queries.
- **Weight tracking:** measurements enriched with BMI, change since the previous entry, and progress toward the target weight.
- **Products:** searchable shared catalog with barcode lookup, per-100 g nutrition, categories and units, plus user-created products.
- **Recipes:** multi-ingredient recipes with nutrition computed from ingredient products and serving size.
- **Validation & mapping:** FluentValidation on inbound requests, AutoMapper between API models and domain entities.
- **Docs & ops:** OpenAPI/Swagger UI, DocFX reference site, and CI that deploys to Azure and publishes the docs.

## Architecture

The solution follows Clean Architecture: dependencies point inward, and the domain has no framework or infrastructure references.

| Project | Responsibility | Key dependencies |
| :--- | :--- | :--- |
| `CalorieTracker.Domain` | Entities, enums, value objects, and pure calculation services (nutrition, weight, recipe). No I/O. | Identity types only |
| `CalorieTracker.Application` | Command/query handlers, application interfaces (`IAppDbContext`, `IJwtGenerator`). Orchestrates the domain. | Domain |
| `CalorieTracker.Infrastructure` | EF Core `AppDbContext` + SQL Server, Identity stores, migrations, JWT generation, SMTP email. | EF Core, MailKit, RazorLight |
| `CalorieTracker.Api` | Controllers, request/response models, validators, AutoMapper profiles, auth & Swagger wiring, the `RequireCompleteProfile` filter. | Application, Infrastructure |
| `CalorieTracker.Tests` | xUnit + Moq unit tests for the domain calculation services. | all of the above |

```
API  ─▶  Application  ─▶  Domain
 │            │
 └────────────┴────────▶  Infrastructure  ─▶  Domain
```

**On the handler layer:** commands and queries are dispatched through small, single-purpose handler classes such as `LogMealHandler` and `GetDailyNutritionProgressHandler`, each registered explicitly in DI. It is a CQRS-style split implemented by hand rather than through a mediator library. The wiring is explicit and easy to trace.

## Nutrition model

The heart of the project is `NutritionCalculationService`, a self-contained domain service with no external dependencies, covered directly by unit tests.

| Quantity | Method |
| :--- | :--- |
| **BMR** | Mifflin–St Jeor equation, sex-specific |
| **TDEE** | BMR × activity multiplier (1.2 sedentary → 1.9 extremely active) |
| **Target calories** | TDEE adjusted by the weekly goal at 7,700 kcal/kg, clamped to safe floors/ceilings per goal |
| **Protein** | g/kg body weight, banded by goal (lose / maintain / gain) |
| **Fat** | percentage of target calories, banded by goal, at 9 kcal/g |
| **Carbohydrates** | remainder of the calorie budget after protein and fat ("carbs by difference") |
| **Water** | 30 ml/kg plus an activity bonus, clamped to 2.0–3.5 L (IOM guidance) |

`WeightAnalysisService` computes BMI, change since the last measurement, and distance to goal, and back-fills those derived fields on each new measurement.

## Tech stack

- **Runtime:** .NET 9, ASP.NET Core Web API
- **Persistence:** Entity Framework Core 9, SQL Server (code-first, 30 migrations)
- **Identity & auth:** ASP.NET Core Identity, JWT bearer (`System.IdentityModel.Tokens.Jwt`), refresh tokens
- **Validation & mapping:** FluentValidation, AutoMapper
- **Email:** MailKit (SMTP) with RazorLight HTML templates
- **Docs:** Swashbuckle / Swagger, DocFX
- **Testing:** xUnit, Moq
- **CI/CD:** GitHub Actions → Azure App Service; DocFX → GitHub Pages

## API reference

All routes are prefixed with `/api`. Endpoints are JWT-protected unless marked *public*; those tagged **profile** additionally require a complete profile.

**Auth:** `/api/auth`
| Method | Route | Description |
| :-- | :-- | :-- |
| POST | `/register` | Create an account *(public)* |
| POST | `/login` | Obtain access + refresh tokens *(public)* |
| POST | `/confirm` | Confirm email with a one-time code *(public)* |
| POST | `/resend-code` | Resend the confirmation code *(public)* |
| POST | `/refresh` | Exchange a refresh token *(public)* |
| POST | `/forgot-password` · `/reset-password` | Password reset flow *(public)* |
| POST | `/logout` | Revoke the current refresh token |
| GET | `/me` | Current user details |

**Profile:** `/api/profile`
| Method | Route | Description |
| :-- | :-- | :-- |
| GET | `/` | Read the profile |
| PUT | `/` | Create or update the profile that drives all targets |

**Products:** `/api/products` · *profile*
| Method | Route | Description |
| :-- | :-- | :-- |
| GET | `/search` · `/{id}` · `/barcode/{barcode}` · `/my-products` | Look up products |
| POST · PUT · DELETE | `/` · `/{id}` | Manage user products |

**Recipes:** `/api/recipes` · *profile*
| Method | Route | Description |
| :-- | :-- | :-- |
| GET | `/` · `/search` · `/my` · `/{id}` | Browse recipes |
| POST · PUT · DELETE | `/` · `/{id}` | Manage recipes |

**Nutrition tracking:** `/api/nutrition-tracking` · *profile*
| Method | Route | Description |
| :-- | :-- | :-- |
| POST · PUT · DELETE | `/log-meal` · `/meals/{id}` | Log and edit meals |
| GET | `/daily-progress` · `/meal-history` | Progress against targets and history |
| POST · PUT · DELETE | `/log-water` · `/water/{id}` | Log and edit water intake |
| GET | `/water/quick-options` | Preset water amounts |

**Weight measurements:** `/api/weightmeasurements` · *profile*
| Method | Route | Description |
| :-- | :-- | :-- |
| GET | `/` · `/latest` · `/{id}` | Read measurements |
| POST · PUT · DELETE | `/` · `/{id}` | Manage measurements |

The full request/response schemas are available in the [live Swagger UI](https://ct-backend-texableplum.azurewebsites.net/index.html).

## Data model

```
ApplicationUser 1───1 UserProfile          (age, sex, height, weight, target, activity, goal, meal plan)
ApplicationUser 1───* MealLogEntry         (→ Product or Recipe, meal type, calculated macros snapshot)
ApplicationUser 1───* WaterIntakeLogEntry  (amount, timestamp)
ApplicationUser 1───* WeightMeasurement    (weight, BMI, change, date)
Recipe          1───* RecipeIngredient ───* Product
```

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or a full instance)
- An SMTP account for confirmation and reset emails

### Configuration

`appsettings.json` ships with empty values, so supply secrets via [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) in development or environment variables in production. The double-underscore form maps to the nested keys below.

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=CalorieTracker;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "<at least 32 characters>",
    "Issuer": "CalorieTracker",
    "Audience": "CalorieTrackerClient"
  },
  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "User": "no-reply@example.com",
    "Password": "<smtp-password>",
    "EnableSsl": true,
    "From": "no-reply@example.com"
  }
}
```

### Database

Automatic migration on startup is intentionally disabled; apply migrations explicitly, and the CI pipeline runs the same command on deploy:

```bash
dotnet ef database update \
  --project src/CalorieTracker.Infrastructure \
  --startup-project src/CalorieTracker.Api
```

### Run

```bash
dotnet restore
dotnet run --project src/CalorieTracker.Api
```

Swagger UI is served at the application root: `http://localhost:5260` or `https://localhost:7095`.

### Test

```bash
dotnet test
```

## Project structure

```
src/
├── CalorieTracker.Domain          # entities, enums, value objects, calculation services
├── CalorieTracker.Application     # command/query handlers, interfaces
├── CalorieTracker.Infrastructure  # EF Core, Identity, migrations, JWT, email
├── CalorieTracker.Api             # controllers, models, validators, mapping, startup
└── CalorieTracker.Tests           # xUnit tests for domain services
docs/                              # DocFX configuration and generated API metadata
```

## Continuous integration

- **`deploy.yml`:** on push to `main`: build, publish, apply EF Core migrations, and deploy to Azure App Service.
- **`documentation.yml`:** build the DocFX site and publish it to GitHub Pages.

## Mobile client

The API is consumed by a companion cross-platform mobile app built with Flutter and Dart.

## A note on language

Type and member names are in English; the inline XML-doc comments and some API response messages are in Polish, as this was an academic project with Polish-language documentation required by the university. The public contract of routes, models, and Swagger is language-neutral.

## License

Released under the [MIT License](LICENSE).
