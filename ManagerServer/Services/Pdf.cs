using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ManagerServer.Services
{
    public sealed class Pdf
    {
        private string authorizationToken;
        private string endpoint;

        public Pdf(string token, string url)
        {
            authorizationToken = token;
            endpoint = url;
        }

        public async Task GetPdf(string html, Stream output)
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationToken);
            request.Content = new StringContent(html, System.Text.Encoding.UTF8, "text/html");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"PDF service {(int)response.StatusCode}: {body}");
            }

            await response.Content.CopyToAsync(output);
        }
    }
}
