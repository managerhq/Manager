using PuppeteerSharp;
using PuppeteerSharp.Media;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ManagerServer.Pdf
{
    internal static class PdfRenderer
    {
        public static async Task RenderToStreamAsync(string html, Stream output)
        {
            var executablePath = await BrowserLocator.ResolveExecutablePathAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = executablePath,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            var page = await browser.NewPageAsync();
            // Disable JS so any <script>, inline on* handler or javascript: URL in the supplied HTML cannot run.
            // page.EvaluateExpressionHandleAsync below still works — it goes through CDP Runtime.evaluate, not page scripts.
            await page.SetJavaScriptEnabledAsync(false);

            // Block file://, loopback, link-local, and RFC1918 sub-resource fetches so attacker-supplied HTML
            // cannot turn the renderer into an SSRF / LFI primitive (cloud metadata, intranet probing, local files).
            await page.SetRequestInterceptionAsync(true);
            page.Request += async (_, e) =>
            {
                try
                {
                    if (IsRequestAllowed(e.Request.Url))
                        await e.Request.ContinueAsync();
                    else
                        await e.Request.AbortAsync();
                }
                catch
                {
                    // Page may already have moved on; nothing we can do here.
                }
            };

            await page.SetContentAsync(html, new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
            });

            await page.EmulateMediaTypeAsync(MediaType.Print);
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");

            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                PrintBackground = true,
                PreferCSSPageSize = true,
                Format = PaperFormat.A4
            });

            await output.WriteAsync(pdfBytes, 0, pdfBytes.Length);
        }

        private static bool IsRequestAllowed(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;

            var scheme = uri.Scheme;
            if (scheme.Equals("data", StringComparison.OrdinalIgnoreCase)) return true;
            if (scheme.Equals("about", StringComparison.OrdinalIgnoreCase)) return true;
            if (!scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
                !scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var host = uri.DnsSafeHost;
            if (string.IsNullOrEmpty(host)) return false;
            if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return false;

            if (IPAddress.TryParse(host, out var literal))
            {
                return IsPublicAddress(literal);
            }

            // Resolve hostname so an attacker-controlled domain pointing at a private IP is still blocked.
            IPAddress[] addresses;
            try
            {
                addresses = Dns.GetHostAddresses(host);
            }
            catch
            {
                return false;
            }
            if (addresses == null || addresses.Length == 0) return false;
            foreach (var address in addresses)
            {
                if (!IsPublicAddress(address)) return false;
            }
            return true;
        }

        private static bool IsPublicAddress(IPAddress address)
        {
            if (IPAddress.IsLoopback(address)) return false;
            if (IPAddress.Any.Equals(address) || IPAddress.IPv6Any.Equals(address)) return false;

            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                var bytes = address.GetAddressBytes();
                // 10.0.0.0/8
                if (bytes[0] == 10) return false;
                // 172.16.0.0/12
                if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return false;
                // 192.168.0.0/16
                if (bytes[0] == 192 && bytes[1] == 168) return false;
                // 169.254.0.0/16 (link-local; covers AWS/GCP/Azure metadata 169.254.169.254)
                if (bytes[0] == 169 && bytes[1] == 254) return false;
                // 127.0.0.0/8
                if (bytes[0] == 127) return false;
                // 0.0.0.0/8
                if (bytes[0] == 0) return false;
                // 100.64.0.0/10 (carrier-grade NAT)
                if (bytes[0] == 100 && bytes[1] >= 64 && bytes[1] <= 127) return false;
                return true;
            }

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                if (address.IsIPv6LinkLocal) return false;
                if (address.IsIPv6SiteLocal) return false;
                if (address.IsIPv6Multicast) return false;
                var bytes = address.GetAddressBytes();
                // fc00::/7 unique local
                if ((bytes[0] & 0xFE) == 0xFC) return false;
                // IPv4-mapped IPv6 — re-check the embedded v4 address.
                if (address.IsIPv4MappedToIPv6)
                {
                    return IsPublicAddress(address.MapToIPv4());
                }
                return true;
            }

            return false;
        }
    }
}
