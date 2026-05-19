using CMS.Contracts.Admin.Images;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Server.Controllers;

[ApiController]
[Route("api/images")]
public sealed class ImagesController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    private readonly IWebHostEnvironment _environment;

    public ImagesController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    private string UploadRoot =>
        Path.Combine(_environment.ContentRootPath, "UploadedImages");

    [HttpPost]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ImageUploadResponseDto>> UploadImage(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No image file was uploaded.");
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest("Image file is too large. Maximum allowed size is 5 MB.");
        }

        var originalExtension = Path.GetExtension(file.FileName);

        if (string.IsNullOrWhiteSpace(originalExtension) ||
            !AllowedExtensions.Contains(originalExtension))
        {
            return BadRequest("Only JPG, JPEG, PNG, and WEBP images are allowed.");
        }

        var normalizedExtension = originalExtension.ToLowerInvariant();

        if (!ContentTypes.TryGetValue(normalizedExtension, out var contentType))
        {
            return BadRequest("Unsupported image type.");
        }

        Directory.CreateDirectory(UploadRoot);

        var storedFileName = $"{Guid.NewGuid():N}{normalizedExtension}";
        var fullPath = Path.Combine(UploadRoot, storedFileName);

        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var imageUrl =
            $"{Request.Scheme}://{Request.Host}/api/images/{Uri.EscapeDataString(storedFileName)}";

        var response = new ImageUploadResponseDto
        {
            FileName = storedFileName,
            ImageUrl = imageUrl,
            SizeBytes = file.Length,
            ContentType = contentType
        };

        return Ok(response);
    }

    [HttpGet("{fileName}")]
    public IActionResult GetImage(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);

        if (!string.Equals(fileName, safeFileName, StringComparison.Ordinal))
        {
            return BadRequest("Invalid file name.");
        }

        var extension = Path.GetExtension(safeFileName);

        if (!ContentTypes.TryGetValue(extension, out var contentType))
        {
            return NotFound();
        }

        var fullPath = Path.Combine(UploadRoot, safeFileName);

        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        return PhysicalFile(fullPath, contentType);
    }
}