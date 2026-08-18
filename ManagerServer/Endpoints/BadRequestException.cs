namespace ManagerServer.Endpoints
{
    internal sealed class BadRequestException : System.Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
