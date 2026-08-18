using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.AccessTokens
{
    [ProtoContract]
    [Title(nameof(Strings.AccessToken), nameof(Strings.Edit))]
    [Guide("The Access Token form is used to create API access tokens.")]
    [Guide("Access tokens allow external applications to interact with your business data through the API.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.AccessToken))]
    internal sealed class AccessTokenForm : NakedVueForm<ManagerServer.Model.AccessToken>
    {
    }
}