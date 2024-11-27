using CompanyManager.Library.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CompanyManager.Library.Services;

public class NotesService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public NotesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiUrl = Config.ApiHost + Config.NotesPrefix;
    }

    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<Note>?> GetAllNotesAsync(string token)
    {
        SetAuthorizationHeader(token);
        var response = await _httpClient.GetAsync($"{_apiUrl}GetAllNotes");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignoruj wielkość liter w nazwach pól
            };
            return JsonSerializer.Deserialize<List<Note>>(content, options);
        }

        return null;
    }

    public async Task<string?> CreateNoteAsync(string token, Note note)
    {
        SetAuthorizationHeader(token);
        var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}CreateNote", content);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : null;
    }

    public async Task<string?> CreateEmptyNoteAsync(string token)
    {
        SetAuthorizationHeader(token);
        var response = await _httpClient.PostAsync($"{_apiUrl}CreateEmptyNote", null);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : null;
    }

    public async Task<bool> DeleteNoteAsync(string token, int noteId)
    {
        SetAuthorizationHeader(token);
        var response = await _httpClient.DeleteAsync($"{_apiUrl}DeleteNote/{noteId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateNoteAsync(string token, Note note)
    {
        SetAuthorizationHeader(token);
        var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_apiUrl}UpdateNote/{note.Id}", content);

        return response.IsSuccessStatusCode;
    }
}
