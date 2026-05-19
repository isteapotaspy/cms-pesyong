using System.IO;

namespace PESYONG.Presentation.Admin.Services;

public sealed class AdminTokenStore : IAdminTokenStore
{
    private readonly string _folderPath;
    private readonly string _filePath;

    public AdminTokenStore()
    {
        _folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PesyongAdmin");

        _filePath = Path.Combine(_folderPath, "admin-token.txt");
    }

    public async Task SaveTokenAsync(string token)
    {
        Directory.CreateDirectory(_folderPath);
        await File.WriteAllTextAsync(_filePath, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!File.Exists(_filePath))
            return null;

        var token = await File.ReadAllTextAsync(_filePath);

        return string.IsNullOrWhiteSpace(token)
            ? null
            : token;
    }

    public Task ClearTokenAsync()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);

        return Task.CompletedTask;
    }
}