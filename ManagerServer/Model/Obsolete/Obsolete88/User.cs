using ManagerServer.Model.Attributes;
using System.Collections.Generic;

namespace ManagerServer.Model.Obsolete.Obsolete88
{
    [ProtoContract]
    [Guid("8112e1a9-fe00-47c4-9d7a-2d034f8a1f34")]
    public sealed class User : NamedObject
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(14)] public string EmailAddress;
        [ProtoMember(2)] public string Username;
        [ProtoMember(3)] public string Password;
        [ProtoMember(5)] public UserType Type;
        [ProtoMember(9)] public string[] Businesses;
        [ProtoMember(8)] public List<Session> Sessions = new List<Session>();
        [ProtoMember(13)] public byte[] Session;
        [ProtoMember(11)] public Guid? MultifactorAuthentication;
        [ProtoMember(12)] public bool Verified;

        [ProtoMember(15)] public string PasswordResetToken;
        [ProtoMember(16)] public DateTime PasswordResetTokenExpiry;

        [ProtoMember(4)] public string Obsolete_Session;
        [ProtoMember(7)] public Visibility Obsolete_Guides;
        [ProtoMember(6)] public Guid[] Obsolete_Businesses;

        public override string GetName() => Username;

        public bool Verify(string password, string authenticationToken)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (string.IsNullOrWhiteSpace(Password)) return false;

            foreach (var e in Password.Split('|'))
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, e))
                    {
                        if (MultifactorAuthentication.HasValue && Verified)
                        {
                            var tfa = new Google.Authenticator.TwoFactorAuthenticator();
                            var result = tfa.ValidateTwoFactorPIN(MultifactorAuthentication.Value.ToString(), authenticationToken);

                            if (result)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (BCrypt.Net.SaltParseException)
                {
                }
            }

            return false;
        }
    }

    [ProtoContract]
    public sealed class Session
    {
        [ProtoMember(1)] public Guid Key;
        [ProtoMember(2)] public DateTime Timestamp = DateTime.UtcNow;
        [ProtoMember(3)] public string UserAgent;
        [ProtoMember(4)] public string Location;
    }
}
