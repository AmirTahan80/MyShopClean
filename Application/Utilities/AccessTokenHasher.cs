using System;
using System.Security.Cryptography;
using System.Text;

namespace Application.Utilities
{
    public static class AccessTokenHasher
    {
        private const string Prefix = "sha256:";

        public static string Hash(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim()));
            return Prefix + Convert.ToHexString(hash);
        }

        public static bool Verify(string storedValue, string providedToken)
        {
            if (string.IsNullOrWhiteSpace(storedValue))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(providedToken))
            {
                return false;
            }

            var expected = storedValue.StartsWith(Prefix, StringComparison.Ordinal)
                ? storedValue
                : Hash(storedValue);
            var actual = Hash(providedToken);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(expected),
                Encoding.ASCII.GetBytes(actual));
        }
    }
}
