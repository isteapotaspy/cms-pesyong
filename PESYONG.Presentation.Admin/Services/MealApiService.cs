using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class MealApiService : IMealApiService
{
    private readonly HttpClient _httpClient;

    public MealApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MealResponse>> GetMealsAsync()
    {2
        var result = await _httpClient.GetFromJsonAsync<List<MealResponse>>("api/meals");

        return result ?? new List<MealResponse>();
    }

    public async Task<MealResponse?> GetMealByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<MealResponse>($"api/meals/{id}");
    }

    public async Task<MealResponse> CreateMealAsync(CreateMealRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/meals", request);

        response.EnsureSuccessStatusCode();

        var meal = await response.Content.ReadFromJsonAsync<MealResponse>();

        return meal ?? throw new InvalidOperationException("Server returned an empty meal response.");
    }

    public async Task<MealResponse> UpdateMealAsync(int id, UpdateMealRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/meals/{id}", request);

        response.EnsureSuccessStatusCode();

        var meal = await response.Content.ReadFromJsonAsync<MealResponse>();

        return meal ?? throw new InvalidOperationException("Server returned an empty meal response.");
    }

    public async Task DeleteMealAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/meals/{id}");

        response.EnsureSuccessStatusCode();
    }
}