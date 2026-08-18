using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ScriptExtensions
{
    [ProtoContract]
    [Title(nameof(Strings.Extensions))]
    [Guide("Create script extensions to customize Manager functionality.")]
    [Guide("Note: This is a legacy feature. Consider using the new extensions system instead.")]
    [Fields(typeof(ManagerServer.Model.ScriptExtension))]
    internal sealed class ScriptExtensionForm : NakedVueForm<ManagerServer.Model.ScriptExtension>
    {
        public override Task ProcessRequest()
        {
            if (!IsAdministrator()) return Task.CompletedTask;
            return base.ProcessRequest();
        }
    }
}
