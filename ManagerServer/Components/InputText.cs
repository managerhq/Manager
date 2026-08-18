using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class InputText : ComponentBase
    {
        public string Name;
        public string Value;
        public string Form;
        public string Placeholder;

        public override void BuildString(StringBuilder sb)
        {
            sb.InputText(name: Name, form: Form, value: Value, placeholder: Placeholder, @class: "form-control min-w-[12ch]");
        }
    }
}
