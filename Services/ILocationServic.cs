using System.Collections.Generic;
using System.Threading.Tasks;

namespace SciencesTechnology.Services
{
    public interface ILocationService
    {
        Task<List<Country>> GetCountriesAsync();
        Task<List<string>> GetStatesByCountryAsync(string country);
    }
}




