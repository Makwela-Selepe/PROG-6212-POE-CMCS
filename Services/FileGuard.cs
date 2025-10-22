using System.Text.RegularExpressions;

namespace ContractMonthlyClaimSystem.Services;

public class FileGuard : IFileGuard
{
    private static readonly string[] _allowedExtensions = [".pdf", ".doc", ".docx", ".xlsx", ".png", ".jpg", ".jpeg"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public bool IsAllowed(string fileName, long size, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            error = "File name cannot be empty.";
            return false;
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            error = $"File type not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}";
            return false;
        }

        if (size <= 0)
        {
            error = "File cannot be empty.";
            return false;
        }

        if (size > MaxFileSize)
        {
            error = $"File size exceeds the maximum limit of {MaxFileSize / 1024 / 1024} MB.";
            return false;
        }

        if (Regex.IsMatch(fileName, @"[<>:""|?*]") || fileName.Contains(".."))
        {
            error = "File name contains invalid characters.";
            return false;
        }

        return true;
    }
}