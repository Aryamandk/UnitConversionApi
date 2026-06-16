using UnitConversionApi.Models;

namespace UnitConversionApi.Configuration;

public interface IUnitRegistry
{
    bool TryGetUnit(string symbol, out Unit? unit);
    IEnumerable<Unit> GetAll();
    IEnumerable<string> GetCategories();
    IEnumerable<Unit> GetByCategory(string category);
}

public class UnitRegistry : IUnitRegistry
{
    private readonly Dictionary<string, Unit> _units;

    public UnitRegistry()
    {
        var units = BuildUnits();
        _units = units.ToDictionary(u => u.Symbol.ToLowerInvariant(), u => u);
    }

    public bool TryGetUnit(string symbol, out Unit? unit) =>
        _units.TryGetValue(symbol.ToLowerInvariant(), out unit);

    public IEnumerable<Unit> GetAll() => _units.Values.OrderBy(u => u.Category).ThenBy(u => u.Symbol);

    public IEnumerable<string> GetCategories() => _units.Values.Select(u => u.Category).Distinct().OrderBy(c => c);

    public IEnumerable<Unit> GetByCategory(string category) =>
        _units.Values.Where(u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<Unit> BuildUnits() =>
    [
        // Length (base: meters)
        new() { Symbol = "m",  Name = "Meter",      Category = "length", ToBaseFactor = 1 },
        new() { Symbol = "km", Name = "Kilometer",  Category = "length", ToBaseFactor = 1000 },
        new() { Symbol = "cm", Name = "Centimeter", Category = "length", ToBaseFactor = 0.01 },
        new() { Symbol = "mm", Name = "Millimeter", Category = "length", ToBaseFactor = 0.001 },
        new() { Symbol = "mi", Name = "Mile",       Category = "length", ToBaseFactor = 1609.344 },
        new() { Symbol = "yd", Name = "Yard",       Category = "length", ToBaseFactor = 0.9144 },
        new() { Symbol = "ft", Name = "Foot",       Category = "length", ToBaseFactor = 0.3048 },
        new() { Symbol = "in", Name = "Inch",       Category = "length", ToBaseFactor = 0.0254 },
        new() { Symbol = "nm", Name = "Nautical Mile", Category = "length", ToBaseFactor = 1852 },

        // Temperature — base: Celsius; offset applied before factor
        // For C: (value + 0) * 1 = C
        // For K: (value + (-273.15)) * 1 = C
        // For F: (value + (-32)) * (5.0/9) = C
        new() { Symbol = "c",  Name = "Celsius",    Category = "temperature", ToBaseFactor = 1,           ToBaseOffset = 0 },
        new() { Symbol = "f",  Name = "Fahrenheit", Category = "temperature", ToBaseFactor = 5.0 / 9.0,  ToBaseOffset = -32 },
        new() { Symbol = "k",  Name = "Kelvin",     Category = "temperature", ToBaseFactor = 1,           ToBaseOffset = -273.15 },

        // Weight/Mass (base: kilograms)
        new() { Symbol = "kg",  Name = "Kilogram",    Category = "weight", ToBaseFactor = 1 },
        new() { Symbol = "g",   Name = "Gram",        Category = "weight", ToBaseFactor = 0.001 },
        new() { Symbol = "mg",  Name = "Milligram",   Category = "weight", ToBaseFactor = 0.000001 },
        new() { Symbol = "lb",  Name = "Pound",       Category = "weight", ToBaseFactor = 0.45359237 },
        new() { Symbol = "oz",  Name = "Ounce",       Category = "weight", ToBaseFactor = 0.028349523 },
        new() { Symbol = "t",   Name = "Metric Ton",  Category = "weight", ToBaseFactor = 1000 },
        new() { Symbol = "st",  Name = "Stone",       Category = "weight", ToBaseFactor = 6.35029318 },

        // Volume (base: liters)
        new() { Symbol = "l",    Name = "Liter",         Category = "volume", ToBaseFactor = 1 },
        new() { Symbol = "ml",   Name = "Milliliter",    Category = "volume", ToBaseFactor = 0.001 },
        new() { Symbol = "m3",   Name = "Cubic Meter",   Category = "volume", ToBaseFactor = 1000 },
        new() { Symbol = "gal",  Name = "US Gallon",     Category = "volume", ToBaseFactor = 3.785411784 },
        new() { Symbol = "qt",   Name = "Quart",         Category = "volume", ToBaseFactor = 0.946352946 },
        new() { Symbol = "pt",   Name = "Pint",          Category = "volume", ToBaseFactor = 0.473176473 },
        new() { Symbol = "cup",  Name = "Cup",           Category = "volume", ToBaseFactor = 0.24 },
        new() { Symbol = "floz", Name = "Fluid Ounce",   Category = "volume", ToBaseFactor = 0.0295735296 },
        new() { Symbol = "tbsp", Name = "Tablespoon",    Category = "volume", ToBaseFactor = 0.0147867648 },
        new() { Symbol = "tsp",  Name = "Teaspoon",      Category = "volume", ToBaseFactor = 0.0049289216 },

        // Speed (base: meters per second)
        new() { Symbol = "mps",  Name = "Meters per Second",   Category = "speed", ToBaseFactor = 1 },
        new() { Symbol = "kph",  Name = "Kilometers per Hour", Category = "speed", ToBaseFactor = 1.0 / 3.6 },
        new() { Symbol = "mph",  Name = "Miles per Hour",      Category = "speed", ToBaseFactor = 0.44704 },
        new() { Symbol = "knot", Name = "Knot",                Category = "speed", ToBaseFactor = 0.514444 },

        // Data (base: bytes)
        new() { Symbol = "b",   Name = "Byte",     Category = "data", ToBaseFactor = 1 },
        new() { Symbol = "kb",  Name = "Kilobyte", Category = "data", ToBaseFactor = 1_024 },
        new() { Symbol = "mb",  Name = "Megabyte", Category = "data", ToBaseFactor = 1_048_576 },
        new() { Symbol = "gb",  Name = "Gigabyte", Category = "data", ToBaseFactor = 1_073_741_824 },
        new() { Symbol = "tb",  Name = "Terabyte", Category = "data", ToBaseFactor = 1_099_511_627_776 },
    ];
}