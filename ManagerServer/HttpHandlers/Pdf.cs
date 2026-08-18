using ManagerServer.Globalization;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class Pdf : HttpHandler
    {
        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();
            var html = string.Empty;
            if (form.ContainsKey("Html")) html = form["Html"].ToString();

            if (string.IsNullOrWhiteSpace(html))
            {
                Response.ContentType = "application/pdf";
                return;
            }

            Response.ContentType = "application/pdf";

            var pdfService = HttpContext.RequestServices.GetService(typeof(Services.Pdf)) as Services.Pdf;
            if (pdfService != null)
            {
                await pdfService.GetPdf(html, Response.Body);
                return;
            }

            try
            {
                await ManagerServer.Pdf.PdfRenderer.RenderToStreamAsync(html, Response.Body);
            }
            catch (PuppeteerException ex)
            {
                Response.StatusCode = 400;
                Write(ex.Message);
            }
        }
    }
}
