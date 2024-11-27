using CompanyManager.Library.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyManager.Library.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiUrl = Config.Host; // Zakładam, że endpointy znajdują się w prefiksie "auth/"
    }

    public async Task<string> RegisterUserAsync(RegisterUserRequestModel request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}register", content);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync(); // Zwraca wiadomość np. "User created successfully."
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Registration failed: {error}");
        }
    }

    public async Task<UserModel?> LoginAsync(LoginRequestModel request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}login", content);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignoruj wielkość liter w nazwach pól
            };

            var resultModel = JsonSerializer.Deserialize<UserModel>(responseData, options);

            return resultModel; // Zwraca dane użytkownika
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Login failed: {error}");
        }
    }
}
