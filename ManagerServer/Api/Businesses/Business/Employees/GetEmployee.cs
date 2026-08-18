using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Employees
{
    [ProtoContract]
    internal sealed class GetEmployee : GetObjectEndpoint<Model.Employee>
    {
    }
}
