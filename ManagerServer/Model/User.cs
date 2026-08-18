using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    public enum UserType : int
    {
        Administrator = 0,
        Restricted = 1
    }

    public enum Visibility : int
    {
        Visible = 0,
        Hidden = 1
    }
}
