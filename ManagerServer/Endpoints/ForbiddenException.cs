namespace ManagerServer.Endpoints
{
    internal sealed class ForbiddenException : System.Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
