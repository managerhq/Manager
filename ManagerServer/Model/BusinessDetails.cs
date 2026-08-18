using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [CustomFields]
    [ProtoContract]
    [Guid("38cf4712-6e95-4ce1-b53a-bff03edad273")]
    public sealed class BusinessDetails : Object, ICustomFields
    {
        [Guide("Enter your business name exactly as it should appear on invoices, reports, and other documents.")]
        [Guide("This name represents your company in all customer-facing materials and official reports.")]
        [Guide("For business templates, leave this field empty to avoid copying the name when creating new businesses.")]
        [ProtoMember(1)] public string Name { get; set; }
        
        [Guide("Enter your complete business address for display on invoices, statements, and correspondence.")]
        [Guide("Use multiple lines to format the address properly - typically street, city/state, postal code, and country.")]
        [Guide("This address appears on all customer documents and should match your official business registration.")]
        [ProtoMember(2), Textarea, Placeholder(nameof(Strings.Address))] public string Address { get; set; }               

        [Guide("Add business-specific information using custom fields configured under `Settings` → `CustomFields`.")]
        [Guide("Common uses include tax registration numbers, business license details, or industry certifications.")]
        [Guide("Custom fields appear on documents and reports based on your configuration settings.")]
        [ProtoMember(5)] public Dictionary<Guid, string> CustomFields { get; set; }
        [Guide("Enhanced custom fields support advanced data types like dates, numbers, and dropdown selections.")]
        [Guide("Use these fields for structured business data that requires validation or specific formatting.")]
        [Guide("Configure field types and validation rules under `Settings` → `CustomFields`.")]
        [ProtoMember(7)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(6)] public string Obsolete_Country { get; set; }
        [ProtoMember(3)] public string Obsolete_BusinessIdentifier { get; set; }
        [ProtoMember(4)] public Guid? Obsolete_Currency { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
    }
}
