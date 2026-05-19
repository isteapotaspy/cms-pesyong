using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Images;

public sealed record ImageUploadResponseDto
{
    public string FileName { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public string ContentType { get; init; } = string.Empty;
}
