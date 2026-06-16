using UnitConversionApi.Configuration;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IConversionService
{
    Result<ConversionResult> Convert(ConversionRequest request);
}

public class ConversionService(IUnitRegistry registry) : IConversionService
{
    public Result<ConversionResult> Convert(ConversionRequest request)
    {
        if (!registry.TryGetUnit(request.FromUnit, out var fromUnit) || fromUnit is null)
            return Result<ConversionResult>.Fail($"Unknown unit: '{request.FromUnit}'.");

        if (!registry.TryGetUnit(request.ToUnit, out var toUnit) || toUnit is null)
            return Result<ConversionResult>.Fail($"Unknown unit: '{request.ToUnit}'.");

        if (!fromUnit.Category.Equals(toUnit.Category, StringComparison.OrdinalIgnoreCase))
            return Result<ConversionResult>.Fail(
                $"Cannot convert between '{fromUnit.Category}' and '{toUnit.Category}'. Units must belong to the same category.");

        double baseValue = fromUnit.ToBase(request.Value);
        double result = toUnit.FromBase(baseValue);

        return Result<ConversionResult>.Ok(new ConversionResult(
            InputValue: request.Value,
            FromUnit: fromUnit.Symbol.ToUpperInvariant(),
            OutputValue: result,
            ToUnit: toUnit.Symbol.ToUpperInvariant(),
            Category: fromUnit.Category
        ));
    }
}

public class Result<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public string? Error { get; private init; }

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}