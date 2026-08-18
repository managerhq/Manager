using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.CashFlowStatement;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    [Title(nameof(Strings.CashFlowStatement))]
    [Guide("The Cash Flow Statement shows cash movements from operating, investing, and financing activities.")]
    [Guide("It reconciles cash changes between periods using direct or indirect method.")]
    internal sealed class CashFlowStatementView : DefaultView<GetCashFlowStatementView>
    {
    }
}