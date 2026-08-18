using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("01e1bcb2-74c1-467d-aedc-5e8f738fe776")]
    public sealed class CustomTheme : NamedObject
    {
        [Guide("Enter a name for this theme. This identifies the theme when selecting it for forms or reports.")]
        [Guide("Use descriptive names like 'Modern Invoice' or 'Classic Statement' to easily identify the theme's purpose.")]
        [ProtoMember(2)] public string Name { get; set; }
        [Guide("Enter the HTML/CSS template code. This controls the visual appearance and layout of documents using this theme.")]
        [Guide("Themes use Liquid templating language with HTML and CSS to customize how transactions appear when printed or emailed.")]
        [ProtoMember(1), Code] public string Template { get; set; }
        [Guide("Check this box to deactivate the theme. Inactive themes won't appear in selection lists but remain in existing documents.")]
        [Guide("Deactivate outdated themes instead of deleting them to preserve the formatting of historical documents.")]
        [ProtoMember(3)] public bool Inactive { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public string GetCompleteTheme()
        {
            return TryMigrateFromPostMessage(TryMigrateFromPartialHtml(TryMigrateFromDotLiquid(Template))) ?? string.Empty;
        }

        public static string TryMigrateFromDotLiquid(string template)
        {
            if (template == null) return template;
            if (!template.Contains("{{business.") && !template.Contains("{{ business.") && !template.Contains("{% endif %}")) return template;

            var buffer = System.Text.UTF8Encoding.UTF8.GetBytes(template);

            template = template.Replace("assign unit price = null", "assign unit_price = null");

            return $@"<script src=""/resources/liquidjs/liquid.browser.min.js""></script><script>
window.onload = async () => {{
const engine = new liquidjs.Liquid();

const response = await fetch(`/api4/transaction-view-v1?${{window.location.search.slice(1)}}`);
const body = await response.json();

const template = new TextDecoder().decode(Uint8Array.from(atob('{Convert.ToBase64String(buffer)}'), c => c.charCodeAt(0)));

engine.parseAndRender(template, body).then(html => document.body.innerHTML = html).catch(error => {{
    const message = (error && error.message) ? error.message : String(error);
    const wrapper = document.createElement('div');
    wrapper.style.cssText = 'padding:30px;max-width:800px;font-family:Helvetica Neue,Helvetica,Arial,sans-serif;color:#171717;line-height:1.5';
    const heading = document.createElement('h2');
    heading.textContent = 'Theme template error';
    heading.style.cssText = 'margin:0 0 12px 0;font-size:18px';
    const intro = document.createElement('p');
    intro.textContent = ""This theme's template could not be rendered because of a Liquid syntax or evaluation error:"";
    intro.style.cssText = 'margin:0 0 12px 0';
    const pre = document.createElement('pre');
    pre.textContent = message;
    pre.style.cssText = 'background:#fef2f2;border:1px solid #fecaca;color:#b91c1c;padding:12px;border-radius:4px;white-space:pre-wrap;word-break:break-word;margin:0 0 12px 0';
    const legacy = document.createElement('p');
    legacy.textContent = 'Note: your theme uses the legacy Liquid templating language, which is no longer the recommended approach. For a future-proof theme, consider creating a new theme written in plain HTML and JavaScript.';
    legacy.style.cssText = 'margin:0;color:#525252;font-size:12px';
    wrapper.append(heading, intro, pre, legacy);
    document.body.innerHTML = '';
    document.body.appendChild(wrapper);
}});
}};
</script>";
        }

        private static string TryMigrateFromPostMessage(string template)
        {
            if (template == null) return null;
            if (!template.Contains(".postMessage(")) return template;

            var script = @"if (!window.parent.__themeContextListenerInstalled) {
    window.parent.__themeContextListenerInstalled = true;
    window.parent.addEventListener('message', async (event) => {
        const { data } = event;
        if (data?.type !== 'context-request') return;
        const { requestId } = data;
        const response = await fetch(`/api4/transaction-view-v1?${event.source.location.search.slice(1)}`);
        const body = await response.json();
        event.source.postMessage({ type: 'context-response', requestId, body }, event.origin);
    });
}";

            var output = $"{template}<script>{script}</script>";
            output = output.Replace("</style>", "@media screen { body > table { background-color: #fff; padding: 20px; width: auto }}</style>");
            output = output.Replace("resources/qrcode/qrcode.js", "/resources/qrcode/qrcode.js");
            output = output.Replace("resources/writtennumber/writtennumber.js", "/resources/writtennumber/writtennumber.js");
            output = output.Replace("if (event.source !== window.parent) return;", string.Empty);

            return output;
        }

        private static string TryMigrateFromPartialHtml(string template)
        {
            if (template == null) return null;
            if (template.StartsWith("<!DOCTYPE html>")) return template;

            return @"<!DOCTYPE html>
<html lang=""" + ManagerServer.Globalization.Languages.GetLanguage() + @""" dir=""" + (ManagerServer.Globalization.Languages.IsRightToLeft() ? "rtl" : "ltr") + @""">
<head>
	<meta charset=""UTF-8"" />
	<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
	<style>
        *, ::after, ::before, ::backdrop, ::file-selector-button {
			margin: 0;
			padding: 0;
		}

		*, ::after, ::before, ::backdrop, ::file-selector-button {
			box-sizing: border-box;
			border: 0 solid;
		}
		body {
            margin: 0px;
			font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif;
			color: #171717;
            padding: 30px;
            min-width: 800px;
			font-size: 12px;
            background-color: #fff;
			line-height: 1.428571429;
		}
		table {
            border-collapse: separate;
            border-spacing: 0px;
			font-size: 12px;            
		}
        table table {
            width: 100%;
        }
        a {
            color: #171717;
            text-decoration: none;
        }
        * {
            border: 0px solid #000;
        }

        @media screen { main { background-color: #fff }}

        @media print {
			body {
				min-width: auto; /* Allow natural width for print */
			}
		}
	</style>
</head>
<body><main>" + template + @"</main></body></html>";
        }
    }
}
