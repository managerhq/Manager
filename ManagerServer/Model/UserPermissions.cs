using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("c6a5d19f-6f47-4716-841d-ba06ca9fc311")]
    public sealed class UserPermissions : Object
    {
        [Guide("Username of the user. This needs to be identical to username set under `Users` tab.")]
        [ProtoMember(1)] public string Username { get; set; }
        [ProtoMember(7)] public Guid[] BankAndCashAccounts { get; set; }

        [Guide("This field determines level of access user will have to this specific business:")]
        [Guide("- Select `CustomAccess` to further configure which specific tabs, reports and screens under `Settings` tab this user can access. Then select access level for each group of screens.")]
        [Guide("- Select `FullAccess` for user to have full access to the business. If user has `FullAccess`, they will also be able to use `Backup` button to download entire copy of the business onto their computer.")]
        [ProtoMember(12)] public UserPermissionsAccessType AccessType { get; set; }
        [ProtoMember(13)] public Dictionary<string, bool> Namespaces { get; set; }
        [ProtoMember(14)] public Dictionary<string, PermittedActions?> Namespaces2 { get; set; }

        [ProtoMember(2)] public bool Obsolete_FullAccess { get; set; }
        [ProtoMember(9)] public Dictionary<string, PermittedActions?> Obsolete_Tabs2 { get; set; }
        [ProtoMember(10)] public Dictionary<string, PermittedActions?> Obsolete_Reports2 { get; set; }
        [ProtoMember(11)] public Dictionary<string, PermittedActions?> Obsolete_Settings2 { get; set; }

        [ProtoMember(3)] public PermittedActions Obsolete_PermittedActions { get; set; }
        [ProtoMember(4)] public string[] Obsolete_Tabs { get; set; }
        [ProtoMember(5)] public string[] Obsolete_Reports { get; set; }
        [ProtoMember(6)] public string[] Obsolete_Settings { get; set; }
        [ProtoMember(8)] public Guid[] Obsolete_CashAccounts { get; set; }

        public bool FullAccess
        {
            get
            {
                return AccessType == UserPermissionsAccessType.FullAccess;
            }
        }

        public Guid[] GetBankCashAccounts()
        {
            if (AccessType == UserPermissionsAccessType.FullAccess) return new Guid[0];

            var list = new List<Guid>();
            if (BankAndCashAccounts != null && BankAndCashAccounts.Length > 0) list.AddRange(BankAndCashAccounts);
            return list.ToArray();
        }

        private PermittedActions? GetPermittedActions(string @namespace)
        {
            if (AccessType == UserPermissionsAccessType.FullAccess) return PermittedActions.ViewCreateUpdateDelete;

            if (Namespaces == null) return null;
            if (Namespaces2 == null) return null;

            var prefix = "ManagerServer.HttpHandlers.Businesses.Business.";

            if (@namespace.Length < prefix.Length) return null;
            var key = @namespace.Substring(prefix.Length);

            var path = string.Empty;
            foreach (var e in key.Split('.'))
            {
                if (path.Length > 0) path += ".";
                path += e;
                if (!Namespaces.ContainsKey(path)) return null;
                if (!Namespaces[path]) return null;
            }

            if (Namespaces2.ContainsKey(path))
            {
                return Namespaces2[path];
            }
            else if (Namespaces.Keys.Any(x => x.StartsWith(path + ".")))
            {
                return PermittedActions.View;
            }

            return null;
        }

        public bool CanCreate(string @namespace)
        {
            var permittedActions = GetPermittedActions(@namespace);
            if (!permittedActions.HasValue) return false;
            return (int)permittedActions.Value <= 2;
        }

        public bool CanUpdate(string @namespace)
        {
            var permittedActions = GetPermittedActions(@namespace);
            if (!permittedActions.HasValue) return false;
            return (int)permittedActions.Value <= 1;
        }

        public bool CanDelete(string @namespace)
        {
            var permittedActions = GetPermittedActions(@namespace);
            if (!permittedActions.HasValue) return false;
            return (int)permittedActions.Value == 0;
        }

        public bool CanView(string @namespace)
        {
            var permittedActions = GetPermittedActions(@namespace);
            if (!permittedActions.HasValue) return false;
            return (int)permittedActions.Value <= 3;            
        }
    }    

    public enum PermittedActions : int
    {
        ViewCreateUpdateDelete = 0,
        ViewCreateUpdate = 1,
        ViewCreate = 2,
        View = 3
    }
}

