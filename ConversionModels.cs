namespace UnitConversionApi.Models;

public record ConversionRequest(
    double Value,
    string FromUnit,
    string ToUnit
);

public record ConversionResult(
    double InputValue,
    string FromUnit,
    double OutputValue,
    string ToUnit,
    string Category
);

public record UnitInfo(
    string Symbol,
    string Name,
    string Category
);

public record ConversionError(string Message, string? Details = null);

public class Unit
{
    public string Symbol { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    /// <summary>Factor to convert this unit to the base unit of its category.</summary>
    public double ToBaseFactor { get; init; }
    /// <summary>Offset applied before multiplying (used for temperature scales).</summary>
    public double ToBaseOffset { get; init; }
    /// <summary>Converts a value in this unit to the category base unit.</summary>
    public double ToBase(double value) => (value + ToBaseOffset) * ToBaseFactor;
    /// <summary>Converts a value from the category base unit to this unit.</summary>
    public double FromBase(double baseValue) => (baseValue / ToBaseFactor) - ToBaseOffset;
}