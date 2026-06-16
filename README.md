# Unit Conversion API

A RESTful ASP.NET Core Web API for converting numerical values between units of measurement across multiple categories.

## Supported Categories

| Category    | Example Units                          |
|-------------|----------------------------------------|
| Length      | m, km, cm, mm, mi, yd, ft, in, nm     |
| Temperature | C, F, K                                |
| Weight/Mass | kg, g, mg, lb, oz, t, st              |
| Volume      | l, ml, m3, gal, qt, pt, cup, floz     |
| Speed       | mps, kph, mph, knot                    |
| Data        | b, kb, mb, gb, tb                      |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Running Locally

```bash
# Clone the repository
git clone <repo-url>
cd UnitConversionApi

# Run the API
dotnet run --project src/UnitConversionApi

# The API will be available at:
# http://localhost:5000  (or https://localhost:5001)
# Swagger UI opens at the root: http://localhost:5000
```

## Running Tests

```bash
dotnet test
```

## API Endpoints

### `POST /api/conversions`

Convert a value from one unit to another.

**Request body:**
```json
{
  "value": 100,
  "fromUnit": "km",
  "toUnit": "mi"
}
```

**Response:**
```json
{
  "inputValue": 100,
  "fromUnit": "KM",
  "outputValue": 62.13711922369,
  "toUnit": "MI",
  "category": "length"
}
```

Unit symbols are **case-insensitive** (`km`, `KM`, and `Km` all work).

### `GET /api/conversions/units`

Returns all supported units. Filter by category with `?category=length`.

### `GET /api/conversions/categories`

Returns all supported unit categories.

## Design Decisions

**Base-unit conversion model.** Each unit stores a single factor (and optional offset) to convert to/from a shared base unit per category (e.g., meters for length, Celsius for temperature). Converting between any two units is then a two-step operation: to-base then from-base. This keeps the unit registry O(n) in size rather than O(n²) — adding a new unit only requires defining its relationship to the base, not to every other unit.

**Temperature offsets.** Temperature scales are non-linear (they differ in both scale and zero point), so the `Unit` model carries a `ToBaseOffset` applied before the factor. This keeps all conversion logic in one place without special-casing temperature.

**Result\<T\> type.** The service layer returns a discriminated result type rather than throwing exceptions for invalid inputs. This avoids using exceptions for control flow and keeps the controller thin.

**Registry as a singleton.** Unit definitions are immutable at runtime, so the registry is registered as a singleton to avoid re-allocating the dictionary on each request.

**Extensibility.** Adding new units only requires adding entries to `UnitRegistry.BuildUnits()`. Persisting units to a database in the future would mean replacing the in-memory dictionary with a repository behind the same `IUnitRegistry` interface, with no changes to the service or controller layer.
