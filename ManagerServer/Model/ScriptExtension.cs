using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("9a8fc328-7553-469f-88ed-dc533f2160b2")]
    public sealed class ScriptExtension : NamedObject
    {
        [Guide("Enter a descriptive name for this script extension.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Enter the JavaScript code to be injected. The code will run after the page loads.")]
        [ProtoMember(3), Javascript] public string Script { get; set; }
        [Guide("Choose where this script should be injected - on a specific page or everywhere.")]
        [ProtoMember(6), NoWrap] public LocationType Location { get; set; }
        [Guide("Enter the URL path where the script should run (e.g., 'sales-invoices' for the sales invoices page).")]
        [ProtoMember(5), IfEnum(nameof(Location), (int)LocationType.Custom), Prepend("/"), EmptyLabel] public string CustomLocation { get; set; }
        [Guide("Check to temporarily disable this script extension without deleting it.")]
        [ProtoMember(4)] public bool Inactive { get; set; }

        [ProtoMember(7)] public string Obsolete_Description { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public bool IsMatch(string path)
        {
            if (Inactive) return false;
            else if (Location == LocationType.Everywhere) return true;
            else if (string.IsNullOrWhiteSpace(CustomLocation)) return false;
            else if (path == '/' + CustomLocation) return true;
            else return false;
        }

        public bool Contains(Guid key)
        {
            if (string.IsNullOrWhiteSpace(Script)) return false;
            if (Script.Contains(key.ToString())) return true;
            return false;
        }
    }

    public enum LocationType : int
    {
        Custom = 0,
        Everywhere = 1
    }
}
