using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Meals;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class MealApiService : IMealApiService
{
    private readonly HttpClient _httpClient;

    private const string BaseRoute = "api/admin/meals";

    public MealApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<MealDto>> GetMealsAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<MealDto>>(
            BaseRoute,
            cancellationToken);

        return result ?? [];
    }

    public async Task<MealDto?> GetMealByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<MealDto>(
            $"{BaseRoute}/{id}",
            cancellationToken);
    }

    public async Task<MealDto> CreateMealAsync(
        CreateMealRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(
            BaseRoute,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var createdMeal = await response.Content.ReadFromJsonAsync<MealDto>(
            cancellationToken: cancellationToken);

        return createdMeal
            ?? throw new InvalidOperationException("API did not return the created meal.");
    }

    public async Task<MealDto> UpdateMealAsync(
        int id,
        UpdateMealRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"{BaseRoute}/{id}",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var updatedMeal = await response.Content.ReadFromJsonAsync<MealDto>(
            cancellationToken: cancellationToken);

        return updatedMeal
            ?? throw new InvalidOperationException("API did not return the updated meal.");
    }

    public async Task DeleteMealAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"{BaseRoute}/{id}",
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}