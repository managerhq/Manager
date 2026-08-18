namespace ManagerServer.Api.Businesses.Business.Settings.StartingBalances.Employees
{
    [ProtoContract]
    internal sealed class GetEmployeeStartingBalanceBatch : GetObjectBatchEndpoint<Model.EmployeeStartingBalance, GetEmployeeStartingBalance, PostEmployeeStartingBalance, PutEmployeeStartingBalance, DeleteEmployeeStartingBalance>
    {
    }
}
