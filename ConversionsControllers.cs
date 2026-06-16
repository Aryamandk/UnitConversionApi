using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Configuration;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>Endpoints for converting values between units of measurement.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConversionsController(IConversionService conversionService, IUnitRegistry unitRegistry) : ControllerBase
{
    /// <summary>Convert a value from one unit to another.</summary>
    /// <remarks>
    /// Example request:
    ///
    ///     POST /api/conversions
    ///     {
    ///         "value": 100,
    ///         "fromUnit": "km",
    ///         "toUnit": "mi"
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ConversionError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConversionError), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Convert([FromBody] ConversionRequest request)
    {
        if (request is null)
            return BadRequest(new ConversionError("Request body is required."));

        if (string.IsNullOrWhiteSpace(request.FromUnit))
            return BadRequest(new ConversionError("'fromUnit' is required."));

        if (string.IsNullOrWhiteSpace(request.ToUnit))
            return BadRequest(new ConversionError("'toUnit' is required."));

        var result = conversionService.Convert(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : UnprocessableEntity(new ConversionError(result.Error!));
    }

    /// <summary>List all supported units, optionally filtered by category.</summary>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IEnumerable<UnitInfo>), StatusCodes.Status200OK)]
    public IActionResult GetUnits([FromQuery] string? category = null)
    {
        var units = string.IsNullOrWhiteSpace(category)
            ? unitRegistry.GetAll()
            : unitRegistry.GetByCategory(category);

        var result = units.Select(u => new UnitInfo(u.Symbol.ToUpperInvariant(), u.Name, u.Category));
        return Ok(result);
    }

    /// <summary>List all supported unit categories.</summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetCategories() => Ok(unitRegistry.GetCategories());
}