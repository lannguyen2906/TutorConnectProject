using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Helper
{
    public class APIAddress
    {
        private readonly HttpClient _httpClient;

        public APIAddress(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetFullAddressByAddressIdAsync(string addressId)
        {
            var apiUrl = $"https://esgoo.net/api-tinhthanh/5/{addressId}.htm";

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                var response = await _httpClient.GetAsync(apiUrl, cts.Token);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cts.Token);
                    var addressData = JsonConvert.DeserializeObject<AddressApiResponse>(json);

                    if (addressData != null && addressData.error == 0)
                    {
                        return addressData.data.full_name;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return "";
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve address information.");
            }

            return "";
        }

        public async Task<string> GetFullAddressByAddressesIdAsync(string cityId, string districtId, string wardId)
        {

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                var cityName = await GetCityNameByIdAsync(cityId);

                var districtName = await GetDistrictNameByIdAsync(districtId);

                var wardName = await GetWardNameByIdAsync(wardId);

                return $"{wardName}, {districtName}, {cityName}";
            }
            catch (OperationCanceledException)
            {
                return "";
            }
            catch (Exception)
            {
                throw new Exception("Unable to retrieve address information.");
            }

            return "";
        }

        public async Task<string> GetDistrictNameByIdAsync(string districtId)
        {
            var response = await _httpClient.GetStringAsync($"https://provinces.open-api.vn/api/d/{districtId}");

            // Deserialize JSON response to get "data_name" or handle error
            var jsonResponse = JsonConvert.DeserializeObject<DistrictResponse>(response);

            // Return district name if found
            return jsonResponse?.name ?? "District not found";
        }

        public async Task<string> GetCityNameByIdAsync(string cityId)
        {
            var response = await _httpClient.GetStringAsync($"https://provinces.open-api.vn/api/p/{cityId}");

            // Deserialize JSON response to get "data_name" or handle error
            var jsonResponse = JsonConvert.DeserializeObject<CityResponse>(response);

            if (jsonResponse?.error == 1)
            {
                // Return error text if available, or a default message
                return jsonResponse.error_text ?? "Error fetching city data";
            }

            // Return city name if found
            return jsonResponse?.name ?? "City not found";
        }

        public async Task<string> GetWardNameByIdAsync(string wardId)
        {
            var response = await _httpClient.GetStringAsync($"https://provinces.open-api.vn/api/w/{wardId}");

            // Deserialize JSON response to get "data_name" or handle error
            var jsonResponse = JsonConvert.DeserializeObject<CityResponse>(response);

            if (jsonResponse?.error == 1)
            {
                // Return error text if available, or a default message
                return jsonResponse.error_text ?? "Error fetching city data";
            }

            // Return city name if found
            return jsonResponse?.name ?? "City not found";
        }
    }
}
