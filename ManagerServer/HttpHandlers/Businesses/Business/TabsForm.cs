using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Tabs))]
    [Guide("Manager consists of 4 main tabs: **Summary**, **Journal Entries**, **Reports**, and **Settings**. These tabs provide the foundation for a double-entry accounting system.")]
    [Guide("Most businesses will need to activate additional tabs to meet their specific requirements. Each tab provides specialized functionality for different aspects of your business.")]
    [Header("Getting Started")]
    [Guide("To customize which tabs appear in your business, click the **Customize** button at the bottom of the tabs list.")]
    [DefaultTabsAndCustomizeScreenshot]    
    [Guide("You will be taken to the form which contains the following checkboxes. Select the tabs you want to enable for your business:")]
    [Fields(typeof(Tabs))]
    [Guide("After selecting the tabs you need, click the **Update** button to save your changes and apply them to your business.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    [Guide("Keep your interface clean by activating only the tabs you currently need. You can always return to this screen to activate additional tabs as your business grows or your needs change.")]    
    internal sealed class TabsForm : NakedVueForm<ManagerServer.Model.Tabs>
    {
    }
}