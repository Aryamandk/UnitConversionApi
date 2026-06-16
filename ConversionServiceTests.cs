using FluentAssertions;
using UnitConversionApi.Configuration;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Tests;

public class ConversionServiceTests
{
    private readonly IConversionService _sut;

    public ConversionServiceTests()
    {
        var registry = new UnitRegistry();
        _sut = new ConversionService(registry);
    }

    [Theory]
    [InlineData(1, "km", "m", 1000)]
    [InlineData(1, "mi", "km", 1.609344)]
    [InlineData(12, "in", "cm", 30.48)]
    [InlineData(1, "ft", "m", 0.3048)]
    public void Convert_Length_ReturnsCorrectResult(double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest(value, from, to));

        result.IsSuccess.Should().BeTrue();
        result.Value!.OutputValue.Should().BeApproximately(expected, 0.0001);
    }

    [Theory]
    [InlineData(0, "c", "f", 32)]
    [InlineData(100, "c", "f", 212)]
    [InlineData(0, "c", "k", 273.15)]
    [InlineData(32, "f", "c", 0)]
    [InlineData(212, "f", "c", 100)]
    public void Convert_Temperature_ReturnsCorrectResult(double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest(value, from, to));

        result.IsSuccess.Should().BeTrue();
        result.Value!.OutputValue.Should().BeApproximately(expected, 0.001);
    }

    [Theory]
    [InlineData(1, "kg", "lb", 2.20462)]
    [InlineData(1, "lb", "kg", 0.453592)]
    [InlineData(1000, "g", "kg", 1)]
    public void Convert_Weight_ReturnsCorrectResult(double value, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest(value, from, to));

        result.IsSuccess.Should().BeTrue();
        result.Value!.OutputValue.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsInputValue()
    {
        var result = _sut.Convert(new ConversionRequest(42, "m", "m"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.OutputValue.Should().Be(42);
    }

    [Fact]
    public void Convert_UnknownFromUnit_ReturnsFail()
    {
        var result = _sut.Convert(new ConversionRequest(1, "xyz", "m"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("xyz");
    }

    [Fact]
    public void Convert_UnknownToUnit_ReturnsFail()
    {
        var result = _sut.Convert(new ConversionRequest(1, "m", "xyz"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("xyz");
    }

    [Fact]
    public void Convert_CrossCategoryConversion_ReturnsFail()
    {
        var result = _sut.Convert(new ConversionRequest(1, "m", "kg"));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("length").And.Contain("weight");
    }

    [Fact]
    public void Convert_CaseInsensitiveUnits_Succeeds()
    {
        var result = _sut.Convert(new ConversionRequest(1, "KM", "M"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.OutputValue.Should().BeApproximately(1000, 0.001);
    }
}