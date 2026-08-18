using ManagerServer.Authentication;
using ManagerServer.Globalization;
using System;
using System.IO;

namespace ManagerServer.Api.Businesses.Business.Settings.AccessTokens
{
    [ProtoContract]
    internal sealed class GetAccessTokenView : GetObjectViewEndpoint<Model.AccessToken>
    {
        protected override View Build(Database business, Model.AccessToken obj)
        {
            var view = BuildFromMembers(business, obj);

            var endpoint = $"{Context.Request.Scheme}://{Context.Request.Host}/api2";
            view.Fields.Add(new View.FieldInfo { Label = "Endpoint", Text = endpoint });

            if (string.IsNullOrWhiteSpace(obj.Password))
            {
                var secret = Guid.CreateVersion7();
                obj.Password = BCrypt.Net.BCrypt.HashPassword(secret.ToString());

                var token = new Tuple<string, Guid, Guid>(Business, obj.Key, secret);
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, token);

                view.Fields.Add(new View.FieldInfo { Label = "Secret", Text = Convert.ToBase64String(ms.ToArray()) });
                view.Fields.Add(new View.FieldInfo { Label = string.Empty, Text = "Make sure to copy your access token now. You won't be able to see it again." });

                GetApplicationData().Businesses.Process(Business, obj, Context.GetManagerUser().Username);
            }
            else
            {
                view.Fields.Add(new View.FieldInfo { Label = "Secret", Text = "********" });
            }

            return view;
        }
    }
}
