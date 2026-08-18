using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("844ab100-63d8-40d7-abc9-1a578197f3a8")]
    public sealed class WebServiceForExchangeRates : Object, IWebService
    {
        [Guide("Check to enable automatic exchange rate updates from the web service.")]
        [ProtoMember(1)] public bool Enabled { get; set; }
        [Guide("Enter the URL of the exchange rate web service, or leave blank to use the default service.")]
        [ProtoMember(2), Prepend("https://"), IfTrue(nameof(Enabled)), NoLabel, Placeholder("forex.manager.io")] public string Url { get; set; }

        string IWebService.GetUrl()
        {
            if (Enabled)
            {
                if (string.IsNullOrWhiteSpace(Url)) return "https://forex.manager.io";
                return "https://"+Url;
            }
            return null;
        }
    }
}
