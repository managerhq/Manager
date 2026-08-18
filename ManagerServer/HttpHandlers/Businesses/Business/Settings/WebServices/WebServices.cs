using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.WebServices
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.WebServices))]
    [Guide("Web services allow Manager to connect to external data sources for automatic updates. This feature helps keep your financial data current without manual entry.")]
    [Guide("Currently, web services are primarily used to fetch real-time *exchange rates* from online sources. This ensures your multi-currency transactions use accurate conversion rates.")]
    [Guide("To configure a web service, click the **New Web Service** button and select the type of service you want to set up. Each service will have its own configuration options based on the data it provides.")]
    internal sealed class WebServices : NakedNamespaces
    {
    }
}
