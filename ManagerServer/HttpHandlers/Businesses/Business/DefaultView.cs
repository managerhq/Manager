using ManagerServer.Api.Businesses.Business;
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class DefaultView<T> : BaseView3 where T : IView
    {
        protected override bool CanHaveAttachments() => true;
        protected override Guid? GetHistoryKey() => Key;

        internal override string GetIframeUrl()
        {
            return new ManagerServer.Api.Businesses.Business.GetView()
            {
                Handler = typeof(T).FullName,
                Business = Business,
                Key = Key,
                Theme = ForceDefaultTheme ? null : GetCustomTheme(),
                Language = Strings.CurrentLanguage.Value,
                Referrer = this.ToUrl()
            }.ToUrl();
        }
    }
}