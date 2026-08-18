using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    [Title(nameof(Strings.ProductionOrder), nameof(Strings.Edit))]
    [Guide("The `ProductionOrder` form enables you to manage the manufacturing process by documenting the conversion of raw materials and components into finished products.")]
    [Guide("Production orders serve as authorization to begin production and help plan material requirements while tracking the entire manufacturing process.")]
    [Header("Why Use Production Orders")]
    [Guide("Production orders are essential for manufacturing businesses, providing a systematic way to track material consumption, labor costs, and overhead allocation during the production process.")]
    [Guide("They ensure accurate inventory valuation by capturing all costs associated with creating finished goods, including direct materials, direct labor, and manufacturing overhead.")]
    [Header("Creating a Production Order")]
    [Guide("When creating a production order, specify the finished goods to be produced and their quantities.")]
    [Guide("List all raw materials and components required, including quantities needed for the production run.")]
    [Guide("Add any non-inventory costs such as labor, utilities, or other manufacturing expenses.")]
    [Header("Tracking and Completion")]
    [Guide("You can track the order through various stages of completion and record actual versus planned consumption.")]
    [Guide("The system automatically adjusts inventory levels when the order is completed, decreasing raw materials and increasing finished goods.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.ProductionOrder))]
    internal sealed class ProductionOrderForm : NakedVueForm<ManagerServer.Model.ProductionOrder>
    {
        protected override bool CanHaveImage() => true;
    }
}