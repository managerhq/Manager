using System;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    public abstract class BusinessHandler : HttpHandler
    {
        [InheritedProtoMember(100)] public string Business;
    }
}