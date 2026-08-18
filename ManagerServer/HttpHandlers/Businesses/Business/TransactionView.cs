using ManagerServer.Globalization;
using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class TransactionView<T> : BaseView3 where T : Model.Object, new()
    {
        protected override bool CanHaveAttachments()
        {
            return true;
        }

        internal override string GetIframeUrl()
        {
            return new ManagerServer.Api.Businesses.Business.GetView {
                Business = Business,
                Key = Key,
                Theme = ForceDefaultTheme ? null : GetCustomTheme(),
                Language = Strings.CurrentLanguage.Value,
                Referrer = this.ToUrl()
            }.ToUrl();
        }
    }
}
