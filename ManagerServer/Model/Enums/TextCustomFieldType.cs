using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagerServer.Model.Enums
{
    public enum TextCustomFieldType : int
    {
        SingleLineText = 0,
        ParagraphText = 1,
        DropdownList = 2,
        QrCode = 3
    }
}
