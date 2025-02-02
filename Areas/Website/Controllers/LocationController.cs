using Microsoft.AspNetCore.Mvc;
using SciencesTechnology.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/location")]
[ApiController]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _locationService.GetCountriesAsync();
        return Ok(countries);
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetStates([FromQuery] string country)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            return BadRequest("Country name is required.");
        }

        var states = await _locationService.GetStatesByCountryAsync(country);
        if (states == null || states.Count == 0)
        {
            return NotFound($"No states found for {country}.");
        }

        return Ok(states);
    }
}

