using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.BillableTime
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTime), nameof(Strings.Edit))]
    [Guide("The `BillableTime` entry form enables you to track hours worked on customer projects or tasks that will be invoiced, ensuring accurate time-based billing for professional services.")]
    [Guide("Billable time tracking is essential for service-based businesses that charge by the hour or need to document time spent on client work. Each entry captures the date, duration, hourly rate, and description of work performed. The system maintains these entries as unbilled until they are included in a sales invoice, allowing you to accumulate time over a period before billing. You can assign different hourly rates for different types of work or employees.")]
    [Guide("When recording billable time, select the customer, enter the date and hours worked, and provide a detailed description of the services performed. Choose the appropriate income account and hourly rate. The description will appear on the customer's invoice, so make it professional and clear. You can mark time entries as non-billable if needed for internal tracking. The status column shows whether time has been invoiced, is pending, or is non-billable.")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.BillableTime))]
    internal sealed class BillableTimeEntryForm : NakedVueForm<ManagerServer.Model.BillableTime>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(ManagerServer.Model.BillableTime form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.BillableTime billableTime)
            {
                Copy(source, form);
            }
        }
    }
}