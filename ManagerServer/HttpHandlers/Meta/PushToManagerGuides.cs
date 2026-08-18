#if DEBUG
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Meta
{
    [ProtoContract]
    internal sealed class PushToManagerGuides : HttpHandler
    {
        public override async Task Get()
        {
            var rawGuides = GetAll().ToDictionary(x => x.Key);

            var dynamoDbContext = CreateDynamoDBContext();
            var existingGuides = await dynamoDbContext.ScanAsync<GuideWithoutContent>([]).GetRemainingAsync();

            var invalidated = new Dictionary<string, RawGuide>(rawGuides);
            foreach (var e in existingGuides)
            {
                if (invalidated.TryGetValue(e.Key, out RawGuide rawGuide) && rawGuide.Hash == e.Hash)
                {
                    invalidated.Remove(e.Key);
                }
            }

            using (Html())
            {
                using (Head()) { }
                using (Body())
                {
                    using (H1()) Write("Push to Manager Guides");

                    using (P()) Write($"Compiled guides: {rawGuides.Count}");
                    using (P()) Write($"Invalidated guides: {invalidated.Count}");

                    if (invalidated.Count > 0)
                    {
                        using (Ul())
                        {
                            foreach (var key in invalidated.Keys)
                                using (Li()) Write(key);
                        }
                    }

                    using (Form(method: "POST"))
                    {
                        InputSubmit(value: "Push");
                    }
                }
            }
        }

        public override async Task Post()
        {
            var rawGuides = GetAll().ToDictionary(x => x.Key);

            var dynamoDbContext = CreateDynamoDBContext();
            var existingGuides = await dynamoDbContext.ScanAsync<GuideWithoutContent>([]).GetRemainingAsync();

            foreach (var e in existingGuides)
            {
                if (rawGuides.TryGetValue(e.Key, out RawGuide rawGuide) && rawGuide.Hash == e.Hash)
                {
                    rawGuides.Remove(e.Key);
                }
            }

            foreach (var e in rawGuides)
            {
                await dynamoDbContext.SaveAsync(new Guide()
                {
                    Key = e.Key,
                    Content = e.Value.Content,
                    Hash = e.Value.Hash
                });
            }

            Response.Redirect(this.ToUrl());
        }

        private static DynamoDBContext CreateDynamoDBContext()
        {
            var builder = new DynamoDBContextBuilder();
            return builder.WithDynamoDBClient(() => new AmazonDynamoDBClient("XXXXXXXXXXXXXXXXXX", "YYYYYYYYYYYYYYYYYYYYYYY", RegionEndpoint.USEast2)).Build();
        }

        private static RawGuide[] GetAll()
        {
            var list = new System.Collections.Generic.List<RawGuide>();

            foreach (var e in typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ManagerServer.Model.Object))).OrderBy(x => x.FullName))
            {
                if (e.FullName.Contains(".Obsolete.")) continue;
                if (e.IsAbstract) continue;
                var fields = e.GetFieldsAndProperties().Where(x => x.GetCustomAttribute<ProtoMemberAttribute>() != null).ToArray();
                if (fields.Length == 0) continue;

                var guides = fields.SelectMany(x => x.GetCustomAttributes<GuideAttribute>()).ToArray();
                if (guides.Length == 0)
                {
                    throw new System.Exception($"Guide missing: {e.FullName}");
                }
            }

            foreach (var e in typeof(Program).Assembly.GetTypes())
            {
                if (!e.IsSubclassOf(typeof(Template)) && e.Name != "Default") continue;
                if (e.IsAbstract) continue;

                var customAttributes = e.GetCustomAttributes(false).OfType<System.Attribute>().OfType<AbstractGuideAttribute>().ToArray();

                if (!customAttributes.Any())
                {
                    throw new System.Exception($"Guide missing: {e.FullName}");
                }

                var key = e.GetCustomAttribute<KeyAttribute>(false)?.Key ?? HttpHandler.ConvertPascalToKebabCase(e).Substring(1);
                if (string.IsNullOrEmpty(key)) key = "default";

                var titleAttribute = e.GetCustomAttribute<TitleAttribute>(false);

                var title = e.Name;
                if (titleAttribute != null) title = string.Join("-", titleAttribute.Text);

                var sb = new StringBuilder();

                sb.Append("<h1>");
                if (titleAttribute != null)
                {
                    sb.Append(TranslateKeywords(titleAttribute.Text));
                }
                else
                {
                    sb.Append(e.Name);
                }
                sb.Append("</h1>");

                sb.Append(GetHtml(e, customAttributes).Trim());

                list.Add(new RawGuide()
                {
                    Key = key,
                    Title = title,
                    Content = sb.ToString(),
                    Hash = ComputeSHA256(sb.ToString())
                });
            }

            return list.ToArray();
        }

        private static string GetHtml(System.Type owner, AbstractGuideAttribute[] attributes)
        {
            var content = new StringBuilder();

            foreach (var e2 in attributes)
            {
                if (e2 is HeaderAttribute headerAttribute)
                {
                    content.Append(Translate("h2", headerAttribute.Text));
                }
                if (e2 is GuideAttribute guideAttribute)
                {
                    content.Append(Translate("p", guideAttribute.Text));
                }
                if (e2 is LinkGuideAttribute linkGuideAttribute)
                {
                    if (!linkGuideAttribute.Type.GetCustomAttributes<GuideAttribute>().Any())
                    {
                        throw new System.Exception($"{owner.FullName}: Not a valid guide link: {linkGuideAttribute.Type.Name}");
                    }

                    var titleAttribute = linkGuideAttribute.Type.GetCustomAttribute<TitleAttribute>();

                    content.Append("<p>");
                    content.Append(Translate("span", linkGuideAttribute.Text));
                    content.Append(" ");
                    content.Append(@$"<a href=""/guides/{linkGuideAttribute.GetKey()}"">{TranslateKeywords(titleAttribute.Text)}</a>");
                    content.Append("</p>");
                }
                if (e2 is ScreenshotAttribute screenshotAttribute)
                {
                    content.Append(screenshotAttribute.GetHtml());
                }
                if (e2 is ColumnsAttribute columnsAttribute)
                {
                    foreach (var e in owner.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var columnAttributes = e.GetCustomAttributes(false).OfType<AbstractGuideAttribute>().ToArray();
                        if (columnAttributes.Length == 0) continue;

                        var columnName = e.Name.Substring(3); // Substring removes "Get" prefix
                        var labelAttribute = e.GetCustomAttribute<LabelAttribute>() ?? new LabelAttribute(columnName);

                        content.Append($"<details>");
                        content.Append($"<summary>{TranslateKeywords(labelAttribute.Value)}</summary>");
                        content.Append("<div>");
                        content.Append(new ColumnScreenshotAttribute(string.Join('-', labelAttribute.Value), null).GetHtml());
                        content.Append(GetHtml(owner, columnAttributes));
                        content.Append("</div>");
                        content.Append($"</details>");
                    }
                }
                if (e2 is FieldsAttribute fieldAttribute)
                {
                    foreach (var e in fieldAttribute.Type.GetFieldsAndProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (e.GetMemberType() == typeof(CustomFields)) continue;
                        if (e.GetMemberType() == typeof(System.Collections.Generic.Dictionary<System.Guid, string>)) continue;
                        if (e.GetCustomAttribute<HiddenAttribute>() != null) continue;

                        var fieldAttributes = e.GetCustomAttributes(false).OfType<AbstractGuideAttribute>().ToArray();
                        if (fieldAttributes.Length == 0) continue;

                        var labelAttribute = e.GetCustomAttribute<LabelAttribute>() ?? new LabelAttribute(e.Name);

                        content.Append($"<details>");
                        content.Append($"<summary>{TranslateKeywords(labelAttribute.Value)}</summary>");
                        content.Append("<div>");
                        content.Append(GetHtml(fieldAttribute.Type, fieldAttributes));
                        content.Append("</div>");
                        content.Append($"</details>");
                    }
                }
                else if (e2 is NamespaceAttribute namespaceAttribute)
                {
                    var namespaceParts = namespaceAttribute.Type.Namespace.Split('.').Length + 1;

                    var types = namespaceAttribute.Type.Assembly
                        .GetTypes()
                        .Where(x => x.Namespace != null)
                        .Where(x => x.Namespace.StartsWith(namespaceAttribute.Filter))
                        .Where(x => x.Namespace.Split('.').Length == namespaceParts)
                        .GroupBy(x => x.Namespace)
                        .Select(x => x.SingleOrDefault(x => x.Name == x.Namespace.Split('.').Last()) ?? x.FirstOrDefault(x => x.Name.EndsWith("List")) ?? x.FirstOrDefault(x => x.Name.EndsWith("Form")))
                        .Where(x => x != null)
                        .ToArray();

                    content.Append("<ul>");
                    foreach (var e in types)
                    {
                        var titleAttribute = e.GetCustomAttribute<TitleAttribute>() ?? new TitleAttribute(e.Name);
                        var key = new LinkGuideAttribute(null, e).GetKey();
                        content.Append(Translate("li", @$"<a href=""guides/{key}"">{TranslateKeywords(titleAttribute.Text)}</a>"));
                    }
                    content.Append("</ul>");
                }
            }

            return content.ToString();
        }

        private static string ApplyBold(string content)
        {
            var sb = new StringBuilder();
            var parts = content.Split("**");
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append(parts[i]);
                }
                else
                {
                    sb.Append("<strong>");
                    sb.Append(parts[i]);
                    sb.Append("</strong>");
                }
            }
            return sb.ToString();
        }

        private static string ApplyItalics(string content)
        {
            var sb = new StringBuilder();
            var parts = content.Split("*");
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append(parts[i]);
                }
                else
                {
                    sb.Append("<em>");
                    sb.Append(parts[i]);
                    sb.Append("</em>");
                }
            }
            return sb.ToString();
        }

        private static string TranslateKeywords(params string[] keywords)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < keywords.Length; i++)
            {
                var keyword = keywords[i];
                if (i > 0) sb.Append(" &mdash; ");
                sb.Append(@$"<span translate=""true"" keyword=""{keyword}"">{Strings.GetPropertyValue(keyword)}</span>");
            }
            return sb.ToString();
        }

        private static string Translate(string element, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var sb = new StringBuilder();

            var parts = text.Split('`');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sb.Append(parts[i]);
                }
                else
                {
                    var keyword = parts[i];
                    var keywordParts = keyword.Split('-');

                    sb.Append("<code>");
                    for (int i2 = 0; i2 < keywordParts.Length; i2++)
                    {
                        if (i2 > 0) sb.Append(" — ");
                        sb.Append(Strings.GetPropertyValue(keywordParts[i2]));
                    }
                    sb.Append("</code>");
                }
            }

            var contentStr = ApplyItalics(ApplyBold(sb.ToString()));

            var keywords = Strings.English.Value;
            var activeKeywords = keywords.Where(x => contentStr.Contains(x.Value, System.StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Key).ToArray();

            var elementStart = @$"<{element} translate=""true"" keywords=""{string.Join(' ', activeKeywords)}"">";
            var elementEnd = $"</{element}>";

            return elementStart + contentStr + elementEnd;
        }

        private static string ComputeSHA256(string s)
        {
            var hash = string.Empty;
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                foreach (byte b in hashValue)
                {
                    hash += $"{b:X2}";
                }
            }

            return hash.ToLowerInvariant();
        }

        private sealed class RawGuide
        {
            public string Key;
            public string Title;
            public string Content;
            public string Hash;

            public override string ToString() => Key;
        }

        [DynamoDBTable("ManagerGuides2")]
        private sealed class Guide
        {
            [DynamoDBHashKey] public string Key { get; set; }
            [DynamoDBProperty] public string Title { get; set; }
            [DynamoDBProperty] public string Content { get; set; }
            [DynamoDBProperty] public string Hash { get; set; }
        }

        [DynamoDBTable("ManagerGuides2")]
        private sealed class GuideWithoutContent
        {
            [DynamoDBHashKey] public string Key { get; set; }
            [DynamoDBProperty] public string Hash { get; set; }
        }
    }
}
#endif