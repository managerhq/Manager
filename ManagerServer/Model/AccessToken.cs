using ManagerServer.Model.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("10431cbb-2b55-4154-b610-c45488afb808")]
    public sealed class AccessToken : Object
    {
        [Guide("Enter a descriptive name for this access token that clearly identifies its purpose or the application using it. For example, 'E-commerce Website Integration' or 'Mobile App Access'.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("The system-generated password for this access token. This password provides programmatic access to your Manager data through the API. Store it securely and never share it publicly.")]
        [ProtoMember(2), Hidden] public string Password { get; set; }
    }
}