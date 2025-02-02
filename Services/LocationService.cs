using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SciencesTechnology.Services
{
    public class LocationService : ILocationService
    {
        private readonly HttpClient _httpClient;

        public LocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var countries = JsonConvert.DeserializeObject<List<Country>>(json);

            return countries.OrderBy(c => c.Name.Common).ToList();
        }

        public async Task<List<string>> GetStatesByCountryAsync(string country)
        {
            Console.WriteLine($"Fetching states for country: '{country}'");

            var response = await _httpClient.GetAsync("https://countriesnow.space/api/v0.1/countries/states");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch states. Status: {response.StatusCode}");
                return new List<string>();
            }

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<CountryStateResponse>(json);

            if (data?.Data == null)
            {
                Console.WriteLine("No data received.");
                return new List<string>();
            }

            var countryTrimmed = country.Trim();
            var countryData = data.Data.FirstOrDefault(c => c.Name.Trim().Equals(countryTrimmed, StringComparison.OrdinalIgnoreCase));

            if (countryData == null || countryData.States == null || !countryData.States.Any())
            {
                Console.WriteLine($"No states found for '{countryTrimmed}'.");
                return new List<string>();
            }

            return countryData.States.Select(s => s.Name).ToList();
        }
    }

    public class Country
    {
        public CountryName Name { get; set; }
    }

    public class CountryName
    {
        public string Common { get; set; }
    }

    public class CountryStateResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<CountryStateData> Data { get; set; }
    }

    public class CountryStateData
    {
        public string Name { get; set; }
        public List<State> States { get; set; }
    }

    public class State
    {
        public string Name { get; set; }
        public string StateCode { get; set; }
    }
}





