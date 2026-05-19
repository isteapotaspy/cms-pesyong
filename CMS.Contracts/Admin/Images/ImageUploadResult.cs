using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Images;

public sealed class ImageUploadResult
{
    public string FileName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string ContentType { get; set; } = string.Empty;
}
