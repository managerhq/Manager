namespace ManagerServer.Endpoints
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class ProducesContentAttribute : Attribute
    {
        public string ContentType { get; }
        public Type BodyType { get; }

        public ProducesContentAttribute(string contentType, Type bodyType = null)
        {
            ContentType = contentType;
            BodyType = bodyType ?? typeof(string);
        }
    }
}
