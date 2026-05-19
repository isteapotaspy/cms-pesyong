using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using CMS.Contracts.Admin.Images;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class ImageApiService : IImageApiService
{
    private readonly HttpClient _httpClient;

    public ImageApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<ImageUploadResult> UploadImageAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Image file path is required.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Selected image file was not found.", filePath);
        }

        await using var fileStream = File.OpenRead(filePath);

        using var form = new MultipartFormDataContent();

        using var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(filePath));

        form.Add(
            fileContent,
            name: "file",
            fileName: Path.GetFileName(filePath));

        using var response = await _httpClient.PostAsync(
            "api/images",
            form,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Image upload failed. Status: {(int)response.StatusCode}. Response: {responseBody}");
        }

        var result = JsonSerializer.Deserialize<ImageUploadResult>(
            responseBody,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return result ?? throw new InvalidOperationException("Image upload response was empty.");
    }

    private static string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}