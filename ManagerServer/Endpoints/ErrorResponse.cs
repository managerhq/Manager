using System.Text.Json.Serialization;

namespace ManagerServer.Endpoints
{
    internal sealed class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; }
    }
}
