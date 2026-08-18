using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.IntangibleAssets
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.IntangibleAsset), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for intangible asset.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(IntangibleAssetStartingBalance))]
    internal sealed class IntangibleAssetStartingBalanceForm : NakedVueForm<IntangibleAssetStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}