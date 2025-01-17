using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SciencesTechnology.Services
{
    public class LocationService : ILocationService
    {
        public Task<List<Country>> GetCountriesAsync()
        {
            var countries = new List<Country>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    // Skip invalid cultures
                    if (culture.LCID == 0x7F) // Neutral Culture
                        continue;

                    var region = new RegionInfo(culture.Name);

                    if (!countries.Any(c => c.Name == region.EnglishName))
                    {
                        countries.Add(new Country { Name = region.EnglishName });
                    }
                }
                catch (CultureNotFoundException)
                {
                    // Skip unsupported cultures
                }
            }

            countries = countries.OrderBy(c => c.Name).ToList();
            return Task.FromResult(countries);
        }
    }

    public class Country
    {
        public string? Name { get; set; }
    }
}




