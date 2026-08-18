using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    internal abstract class ViewEndpoint<T> : AuthorizedEndpoint<T>
    {
        [InheritedProtoMember(200)] public Guid? Key { get; set; }
        [InheritedProtoMember(300)] public string Referrer { get; set; }
        [InheritedProtoMember(400)] public string Language { get; set; }
        [InheritedProtoMember(450)] public string Handler { get; set; }
    }
}
