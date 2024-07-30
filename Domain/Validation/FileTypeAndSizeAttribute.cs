using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

public class FileTypeAndSizeAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;
    private readonly long _maxFileSize;

    public FileTypeAndSizeAttribute(string[] allowedExtensions, long maxFileSize)
    {
        _allowedExtensions = allowedExtensions;
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var files = value as List<IFormFile>;
        if (files != null)
        {
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"File type not allowed. Allowed types are: {string.Join(", ", _allowedExtensions)}");
                }

                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size exceeded. Maximum allowed size is {(_maxFileSize / 1024 / 1024)} MB.");
                }
            }
        }

        return ValidationResult.Success;
    }
}
