using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Employees
{
    [ProtoContract]
    internal sealed class GetEmployeeBatch : GetObjectBatchEndpoint<Model.Employee, GetEmployee, PostEmployee, PutEmployee, DeleteEmployee>
    {
    }
}
