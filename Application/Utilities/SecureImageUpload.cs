using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.Utilities
{
    public static class SecureImageUpload
    {
        public const long MaximumFileSize = 5 * 1024 * 1024;

        private static readonly IReadOnlyDictionary<string, byte[][]> Signatures =
            new Dictionary<string, byte[][]>(StringComparer.OrdinalIgnoreCase)
            {
                [".jpg"] = new[] { new byte[] { 0xFF, 0xD8, 0xFF } },
                [".png"] = new[] { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
                [".gif"] = new[]
                {
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
                    new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }
                },
                [".webp"] = new[] { new byte[] { 0x52, 0x49, 0x46, 0x46 } }
            };

        public static bool TrySave(
            IFormFile file,
            string destinationDirectory,
            out string fileName,
            out string errorMessage)
        {
            fileName = null;
            errorMessage = null;

            if (file == null || file.Length == 0)
            {
                errorMessage = "لطفاً یک تصویر برای بارگذاری انتخاب کنید.";
                return false;
            }

            if (file.Length > MaximumFileSize)
            {
                errorMessage = "حجم تصویر نباید بیشتر از ۵ مگابایت باشد.";
                return false;
            }

            var detectedExtension = DetectExtension(file);
            if (detectedExtension == null)
            {
                errorMessage = "فرمت تصویر معتبر نیست. فرمت‌های JPG، PNG، GIF و WebP مجاز هستند.";
                return false;
            }

            Directory.CreateDirectory(destinationDirectory);
            fileName = $"{Guid.NewGuid():N}{detectedExtension}";
            var fullPath = Path.Combine(destinationDirectory, fileName);

            using var output = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            file.CopyTo(output);
            return true;
        }

        private static string DetectExtension(IFormFile file)
        {
            Span<byte> header = stackalloc byte[12];
            using var stream = file.OpenReadStream();
            var bytesRead = stream.Read(header);

            foreach (var candidate in Signatures)
            {
                var signatureMatched = false;
                foreach (var signature in candidate.Value)
                {
                    if (bytesRead >= signature.Length &&
                        header[..signature.Length].SequenceEqual(signature))
                    {
                        signatureMatched = true;
                        break;
                    }
                }

                if (!signatureMatched)
                {
                    continue;
                }

                if (candidate.Key.Equals(".webp", StringComparison.OrdinalIgnoreCase) &&
                    (bytesRead < 12 || !header[8..12].SequenceEqual(new byte[] { 0x57, 0x45, 0x42, 0x50 })))
                {
                    continue;
                }

                return candidate.Key;
            }

            return null;
        }
    }
}
