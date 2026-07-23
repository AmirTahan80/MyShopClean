using Application.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text;
using Xunit;

namespace Tests;

public class SecurityUtilitiesTests
{
    [Fact]
    public void SafeHtmlRenderer_RemovesExecutableContent_AndPreservesFormatting()
    {
        const string input =
            "<p>متن <strong>مهم</strong></p>" +
            "<script>alert(1)</script>" +
            "<img src=\"javascript:alert(1)\" onerror=\"alert(1)\">" +
            "<a href=\"javascript:alert(1)\">پیوند</a>";

        var result = SafeHtmlRenderer.Sanitize(input);

        Assert.Contains("<strong>", result);
        Assert.Contains("</strong>", result);
        Assert.DoesNotContain("script", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("javascript:", result, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SecureImageUpload_RejectsAFileWhoseContentIsNotAnImage()
    {
        var content = Encoding.UTF8.GetBytes("<script>alert(1)</script>");
        using var stream = new MemoryStream(content);
        var file = new FormFile(stream, 0, stream.Length, "upload", "photo.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
        var destination = Path.Combine(Path.GetTempPath(), $"myshop-upload-{Guid.NewGuid():N}");

        try
        {
            var saved = SecureImageUpload.TrySave(
                file,
                destination,
                out _,
                out var errorMessage);

            Assert.False(saved);
            Assert.Contains("فرمت تصویر معتبر نیست", errorMessage);
            Assert.False(Directory.Exists(destination));
        }
        finally
        {
            if (Directory.Exists(destination))
            {
                Directory.Delete(destination, recursive: true);
            }
        }
    }

    [Fact]
    public void AccessTokenHasher_DoesNotStorePlaintext_AndUsesExactComparison()
    {
        const string token = "a-secure-random-token-with-more-than-32-characters";

        var stored = AccessTokenHasher.Hash(token);

        Assert.DoesNotContain(token, stored);
        Assert.True(AccessTokenHasher.Verify(stored, token));
        Assert.False(AccessTokenHasher.Verify(stored, token + "-changed"));
    }
}
