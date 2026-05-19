using CMS.Contracts.Admin.Images;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IImageApiService
{
    Task<ImageUploadResult> UploadImageAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}