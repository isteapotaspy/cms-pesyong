using Microsoft.AspNetCore.Mvc;

namespace CMS.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImagesController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    public ImagesController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImageUploadResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImageUploadResponseDto>> UploadImage(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No image file was uploaded.");

        var extension = Path.GetExtension(file.FileName);

        if (!AllowedExtensions.Contains(extension))
            return BadRequest("Invalid image type. Only JPG, JPEG, PNG, and WEBP are allowed.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = System.IO.File.Create(filePath);
        await file.CopyToAsync(stream);

        var imageUrl = $"/images/{fileName}";

        return Ok(new ImageUploadResponseDto
        {
            FileName = fileName,
            ImageUrl = imageUrl
        });
    }
}

public sealed class ImageUploadResponseDto
{
    public string FileName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}