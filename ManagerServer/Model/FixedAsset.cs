using System;
using System.Collections.Generic;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("00082353-9fca-4ab4-91ae-20505479cbda")]
    public sealed class FixedAsset : NamedObject, ICustomFields, IComparable<FixedAsset>, ICode
    {
        [Guide("Enter a unique code or reference number to identify this fixed asset.")]
        [Guide("Asset codes are optional but recommended for asset tracking and physical verification. Common formats include asset tags, serial numbers, or internal reference codes.")]
        [Guide("This code appears in the fixed asset register and helps match physical assets to accounting records.")]
        [ProtoMember(10), Short, Placeholder(nameof(Strings.Optional)), NoWrap] public string ItemCode { get; set; }
        [Guide("Enter a descriptive name for the fixed asset that clearly identifies what it is.")]
        [Guide("Use specific names that distinguish similar assets. Examples: 'Dell Laptop - Finance Dept', '2019 Toyota Hilux - Reg ABC123', or 'Office Building - 123 Main Street'.")]
        [Guide("This name appears in all reports and the fixed asset register.")]
        [ProtoMember(1)] public string ItemName { get; set; }
        [Guide("Enter the annual depreciation rate as a percentage without the % symbol.")]
        [Guide("For example, enter 20 for 20% per year, or 10 for 10% per year. This rate is applied using the diminishing value (declining balance) method.")]
        [Guide("The depreciation expense is automatically calculated and posted to the profit and loss statement based on this rate.")]
        [ProtoMember(17), Append("%")] public decimal DepreciationRate { get; set; }
        [Guide("Enter additional details about the fixed asset to help with identification and management.")]
        [Guide("Include information such as: serial numbers, model numbers, purchase date, supplier details, warranty information, physical location, or technical specifications.")]
        [Guide("This description is for internal reference and does not appear on financial statements.")]
        [ProtoMember(2), Long, Textarea] public string Description { get; set; }
        [Guide("Assign this fixed asset to a specific division for divisional cost allocation.")]
        [Guide("The asset's depreciation expense will be allocated to the selected division in divisional profit reports.")]
        [Guide("This field only appears if divisions are enabled under `Settings` → `Divisions`.")]
        [ProtoMember(22), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Select a custom control account to categorize this asset differently from the default fixed assets account.")]
        [Guide("Custom control accounts help separate different asset types on the balance sheet, such as vehicles, equipment, buildings, or computer hardware.")]
        [Guide("This field only appears if custom control accounts for fixed assets have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(14), Autocomplete(typeof(ControlAccountForFixedAssets))] public Guid? ControlAccountForFixedAssets { get; set; }
        [Guide("Select a custom accumulated depreciation account to track this asset's depreciation separately.")]
        [Guide("This account accumulates all depreciation expenses for this asset over its useful life, reducing the asset's net book value on the balance sheet.")]
        [Guide("This field only appears if custom control accounts for accumulated depreciation have been created under `Settings` → `Control Accounts`.")]
        [ProtoMember(18), Autocomplete(typeof(ControlAccountForFixedAssetsAccumulatedDepreciation))] public Guid? ControlAccountForFixedAssetsAccumulatedDepreciation { get; set; }
        [Guide("Enable this option to record depreciation expense to a specific account rather than the default.")]
        [Guide("Useful when different types of assets need their depreciation tracked separately in the profit and loss statement.")]
        [ProtoMember(20)] public bool CustomDepreciationExpenseAccount { get; set; }
        [Guide("Select the profit and loss account where this asset's depreciation expense will be recorded.")]
        [Guide("Choose an appropriate expense account based on the asset type or department. For example, 'Vehicle Depreciation' for vehicles or 'Office Equipment Depreciation' for computers.")]
        [ProtoMember(21), IfTrue(nameof(CustomDepreciationExpenseAccount)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountFixedAssetDepreciation))] public Guid? CustomDepreciationExpenseAccountSelection { get; set; }
        [Guide("Check this box when the fixed asset is no longer owned by the business due to sale, disposal, or write-off.")]
        [Guide("Marking an asset as disposed stops future depreciation calculations and removes it from active asset lists.")]
        [Guide("The asset and its history remain in the system for reporting purposes.")]
        [ProtoMember(6)] public bool DisposedFixedAsset { get; set; }
        [Guide("Enter the date when the asset was sold, scrapped, or otherwise disposed of.")]
        [Guide("Depreciation is automatically calculated up to this date. Any gain or loss on disposal is calculated based on the net book value at this date.")]
        [ProtoMember(7), IfTrue(nameof(DisposedFixedAsset)), Prepend(nameof(Strings.DisposalDate)), NoLabel, NoWrap] public DateTime? DisposalDate { get; set; }
        [Guide("Select the profit and loss account to record any gain or loss when this asset is disposed of.")]
        [Guide("The gain or loss is automatically calculated as the difference between the disposal proceeds and the asset's net book value.")]
        [Guide("If not specified, the default `Fixed Asset Loss on Disposal` account is used.")]
        [ProtoMember(19), IfTrue(nameof(DisposedFixedAsset)), Prepend(nameof(Strings.Account)), NoLabel, Autocomplete(typeof(ProfitAndLossStatementAccount), Placeholder = typeof(ProfitAndLossStatementAccountFixedAssetLossOnDisposal))] public Guid? CustomExpenseAccountForDisposal { get; set; }
        [ProtoMember(9)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(23)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(25)] public bool Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(4)] public decimal Obsolete_StartingBalanceAcquisitionCost2 { get; set; }
        [ProtoMember(8)] public decimal Obsolete_StartingBalanceAccumulatedDepreciation2 { get; set; }
        [ProtoMember(3)] public bool Obsolete_HasOpeningBalance { get; set; }
        [ProtoMember(5)] public DateTime Obsolete_OpeningBalanceDate { get; set; }
        [ProtoMember(11)] public DepreciationMethod Obsolete_DepreciationMethod { get; set; }
        [ProtoMember(12)] public int? Obsolete_EffectiveLife { get; set; }
        [ProtoMember(13)] public bool Obsolete_HasStartingBalance { get; set; }
        [ProtoMember(15)] public decimal Obsolete_StartingBalanceCost { get; set; }
        [ProtoMember(16)] public decimal Obsolete_StartingBalanceAccumulatedDepreciation { get; set; }

        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        int IComparable<FixedAsset>.CompareTo(FixedAsset other) => (IsInactive(), ItemCode, ItemName).CompareTo((other.IsInactive(), other.ItemCode, other.ItemName));
        string ICode.Code => ItemCode;

        public override bool IsInactive() => DisposedFixedAsset && DisposalDate.HasValue;

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ItemCode)) return ItemCode + " - " + ItemName;
                else return ItemName;
            }
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (DisposedFixedAsset && DisposalDate.HasValue) return false;
            if (filter is ControlAccountForFixedAssets && ControlAccountForFixedAssets != filter.Key) return false;
            return true;
        }

        public override string GetName()
        {
            return NameWithCode;
        }
    }
}
