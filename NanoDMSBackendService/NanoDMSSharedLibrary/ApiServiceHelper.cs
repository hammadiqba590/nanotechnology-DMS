using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    public class ApiServiceHelper
    {
        private readonly HttpClient _httpClient;

        public ApiServiceHelper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse?> SendRequestAsync<TRequest, TResponse>(string url,HttpMethod method,TRequest? requestData = null,string? jwtToken = null,string? basicUsername = null,string? basicPassword = null) where TRequest : class
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                // Add Authorization Header (JWT or Basic)
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                }
                else if (!string.IsNullOrEmpty(basicUsername) && !string.IsNullOrEmpty(basicPassword))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{basicUsername}:{basicPassword}");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                // Add content if needed
                if (requestData != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    string json = JsonSerializer.Serialize(requestData);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode}, Message: {errorMessage}");
                }

                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                throw new Exception($"API Request Failed: {ex.Message}");
            }
        }

        public async Task<JsonElement?> SendRequestAsync<TRequest>(
    string url,
    HttpMethod method,
    TRequest? requestData = null,
    string? jwtToken = null,
    string? basicUsername = null,
    string? basicPassword = null
) where TRequest : class
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                if (!string.IsNullOrEmpty(jwtToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                }
                else if (!string.IsNullOrEmpty(basicUsername) && !string.IsNullOrEmpty(basicPassword))
                {
                    var byteArray = Encoding.ASCII.GetBytes($"{basicUsername}:{basicPassword}");
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }

                if (requestData != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    string json = JsonSerializer.Serialize(requestData);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseContent))
                {
                    return null;
                }

                using var document = JsonDocument.Parse(responseContent);
                return document.RootElement.Clone(); // Clone so it's accessible outside using scope
            }
            catch (Exception ex)
            {
                // Optionally log the error here
                return null; // fail gracefully
            }
        }

    }


    //Method Without Request Data (For GET, DELETE)//

    //public async Task<TResponse?> SendRequestAsync<TResponse>(string url, HttpMethod method)
    //    {
    //        return await SendRequestAsync<object, TResponse>(url, method, null);
    //    }


    //Usage Example//

    //var apiService = new ApiService(new HttpClient());

    //    // GET request (no request body)
    //    var response = await apiService.SendRequestAsync<MyResponse>("https://api.example.com/data", HttpMethod.Get);

    //    // POST request with data
    //    var requestData = new { Name = "Hammad", Email = "hammad@example.com" };
    //    var postResponse = await apiService.SendRequestAsync<object, ApiResponse>("https://api.example.com/create-user", HttpMethod.Post, requestData);



}
