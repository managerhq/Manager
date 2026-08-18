using System;

namespace ManagerServer.Model.Attributes
{
    public class AddLineLabelAttribute : Attribute
    {
        private string value;

        public AddLineLabelAttribute(string value)
        {
            this.value = value;
        }

        public string GetTranslatedString()
        {
            return ManagerServer.Globalization.Strings.GetPropertyValue(value);
        }
    }
}
