using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static class GuidExtensions
    {
        public static Guid ToGuid(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("GUID input cannot be null or empty.", nameof(input));
            }

            if (!Guid.TryParse(input, out Guid parsedGuid))
            {
                throw new ArgumentException(
                    $"The value '{input}' is not a valid GUID. " +
                    "Please provide a valid GUID in the correct format, for example: " +
                    "'FA896E59-5EAF-450E-AA6B-E2321D96026B'.",
                    nameof(input));
            }

            return parsedGuid;
        }
    }
}