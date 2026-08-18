using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Email))]
    [Guide("This screen faciliate sending emails from Manager.")]
    internal sealed class EmailForm : BusinessTemplate
    {
        [ProtoMember(1)] public Guid? Source;

        protected override void InnerGet2()
        {
            if (!Request.HasFormContentType)
            {
                return;
            }

            var form = Request.ReadFormAsync().GetAwaiter().GetResult();

            var attachment = form["2be520d4-1fa1-4118-a5a5-627e0576a1c4"].ToString();
            var filename = form[nameof(SendEmail.FormData.Filename)].ToString();
            var to = form[nameof(SendEmail.FormData.To)].ToString();
            var subject = form[nameof(SendEmail.FormData.Subject)].ToString();
            var body = form[nameof(SendEmail.FormData.Body)].ToString();
            var variables = form["Variables"].ToString();            

            using (Div(@class: "flex items-start gap-4"))
            {
                using (Div(@class: "card flex-1"))
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.Email);
                    }
                    using (Div(@class: "card-form"))
                    {
                        using (Div(@class: "form-group"))
                        {
                            using (Div(@class: "input-group"))
                            {
                                using (Div(@class: "input-group-text")) Write(Strings.To);
                                InputText(name: nameof(SendEmail.FormData.To), value: to, @class: "form-control", form: nameof(EmailForm));
                            }
                        }

                        using (Div(@class: "form-group"))
                        {
                            using (Div(@class: "input-group"))
                            {
                                using (Div(@class: "input-group-text")) Write(Strings.Subject);
                                InputText(name: nameof(SendEmail.FormData.Subject), value: subject, @class: "form-control", form: nameof(EmailForm));
                            }
                        }

                        using (Div(@class: "form-group"))
                        {
                            Textarea(name: nameof(SendEmail.FormData.Body), rows: 10, text: body, @class: "form-control field-sizing-content resize", spellcheck: true, form: nameof(EmailForm));
                        }

                        using (Script())
                        {
                            Write(@"const engine = new liquidjs.Liquid();
const variables = JSON.parse('" + JavaScriptEncoder.Default.Encode(variables) + @"');
engine.parseAndRender('" + JavaScriptEncoder.Default.Encode(subject) + @"', variables).then(x => document.getElementsByName('" + nameof(SendEmail.FormData.Subject) + @"')[0].value = x.trim());
engine.parseAndRender('" + JavaScriptEncoder.Default.Encode(body) + @"', variables).then(x => document.getElementsByName('" + nameof(SendEmail.FormData.Body) + @"')[0].value = x.trim());
");
                        }

                        using (Div(@class: "form-group"))
                        {
                            using (Div(@class: "flex flex-wrap gap-2"))
                            {
                                using (Div(@class: "input-group"))
                                {
                                    using (Div(@class: "input-group-text"))
                                    {
                                        using (Label(@class: "flex items-start gap-2 mb-0 whitespace-normal text-start"))
                                        {
                                            InputHidden(name: nameof(SendEmail.FormData.PDF), value: attachment, form: nameof(EmailForm));
                                            InputHidden(name: nameof(SendEmail.FormData.Filename), value: filename, form: nameof(EmailForm));
                                            InputCheckbox(disabled: true, @checked: true, @class: "form-check-input");
                                            Write(filename + ".pdf");
                                        }
                                    }
                                }

                                if (Source.HasValue)
                                {
                                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Attachment>().Where(x => x.Object == Source.Value).OrderBy(x => x.Name))
                                    {
                                        using (Div(@class: "input-group"))
                                        {
                                            using (Div(@class: "input-group-text"))
                                            {
                                                using (Label(@class: "flex items-start gap-2 mb-0 cursor-pointer whitespace-normal text-start"))
                                                {
                                                    InputCheckbox(name: nameof(SendEmail.FormData.Attachments), value: e.Key.ToString(), @class: "form-check-input", form: nameof(EmailForm));
                                                    Write(e.Name);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "flex gap-2 items-center"))
                        {
                            using (Form(id: nameof(EmailForm), method: "POST", action: new SendEmail() { Business = Business, Source = Source, Referrer = Referrer }.ToUrl(), hxBoost: true, hxDisabledElt: "button"))
                            {
                                using (Button(@class: "btn btn-primary"))
                                {
                                    Write(Strings.Send);
                                    I(@class: "htmx-indicator ms-2 fas fa-circle-notch fa-spin !hidden");
                                }
                            }
                            using (A(href: Referrer, @class: "btn")) Write(Strings.Cancel);
                        }
                    }
                }

                using (Div(@class: "card flex-1"))
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "card-title")) Write(filename + ".pdf");
                    }

                    using (Div(@class: "card-body p-0"))
                    {
                        IFrame(src: "data:application/pdf;base64," + attachment, @class: "w-full aspect-2/3");
                    }
                }
            }            
        }

        protected override async Task InnerPost()
        {
            await Request.ReadFormAsync();
            await Get();
        }
    }
}
