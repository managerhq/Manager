using System;
using System.Security.Cryptography;
using System.Text;

namespace ManagerServer.Helpers
{
    /// <summary>
    /// A short code the user types into the password reset screen themselves, instead of a link we email them.
    /// The server cannot know its own public address when it sits behind a reverse proxy, so any emailed link
    /// has to be built from either client-supplied input or operator configuration. A typed code needs neither.
    /// </summary>
    internal static class PasswordResetCode
    {
        // Crockford base32: no I, L, O or U, so the code cannot be misread as 1/0 or spell anything unfortunate.
        private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        private const int Length = 8; // 8 characters of 5 bits = 40 bits of entropy.

        /// <summary>Generates a code and returns it in the grouped form shown to the user, e.g. "4K7M-P2WD".</summary>
        internal static string Generate()
        {
            var bytes = RandomNumberGenerator.GetBytes(5); // 40 bits
            var value = 0UL;
            foreach (var b in bytes) value = (value << 8) | b;

            var code = new char[Length];
            for (var i = Length - 1; i >= 0; i--)
            {
                code[i] = Alphabet[(int)(value & 31)];
                value >>= 5;
            }

            return new string(code, 0, 4) + "-" + new string(code, 4, 4);
        }

        /// <summary>
        /// Strips the grouping and corrects the characters Crockford base32 leaves out, so the code matches
        /// however the user retyped it — lower case, spaces instead of the dash, letter O for zero.
        /// </summary>
        internal static byte[] Normalize(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var builder = new StringBuilder(Length);
            foreach (var c in code)
            {
                var upper = char.ToUpperInvariant(c);
                switch (upper)
                {
                    case 'O': builder.Append('0'); break;
                    case 'I':
                    case 'L': builder.Append('1'); break;
                    case 'U': builder.Append('V'); break;
                    default:
                        if (Alphabet.Contains(upper)) builder.Append(upper);
                        break; // Anything else (the dash, spaces) is grouping, not code.
                }

                if (builder.Length > Length) return null;
            }

            if (builder.Length != Length) return null;

            return Encoding.ASCII.GetBytes(builder.ToString());
        }

        /// <summary>Compares a stored code against one the user typed, without leaking where they diverge.</summary>
        internal static bool Matches(byte[] stored, byte[] supplied)
        {
            if (stored == null) return false;
            if (supplied == null) return false;

            return CryptographicOperations.FixedTimeEquals(stored, supplied);
        }
    }
}
