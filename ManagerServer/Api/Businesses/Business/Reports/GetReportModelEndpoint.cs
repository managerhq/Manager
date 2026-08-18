using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports
{
    internal abstract class GetReportModelEndpoint<T> : ViewEndpoint<V2.ReportModel2>, IView where T : Model.Object, new()
    {
        public sealed override V2.ReportModel2 AuthorizedHandle()
        {
            var business = GetApplicationData().Businesses.Get(Business);
            var report = business.SingleOrDefault<T>(Key);
            if (report == null) return new V2.ReportModel2();

            Languages.SetLanguage(Language);

            var model = Build(business, report);
            if (model == null) return new V2.ReportModel2();

            if (string.IsNullOrEmpty(model.Title)) model.Title = DefaultTitle;

            var businessDetails = business.Single<Model.BusinessDetails>();
            model.Business = businessDetails.Name;
            model.Direction = Languages.IsRightToLeft() ? "rtl" : "ltr";
            if (string.IsNullOrWhiteSpace(model.Business)) model.Business = Business;

            return model;
        }

        protected abstract V2.ReportModel2 Build(Database business, T report);

        public View GetView()
        {
            var o = AuthenticatedHandle();
            return ViewMapper.From(o);
        }

        protected abstract string DefaultTitle { get; }
    }
}
