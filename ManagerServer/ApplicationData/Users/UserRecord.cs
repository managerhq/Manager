using System;
using System.Collections.Generic;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer
{
    [ProtoContract]
    public sealed class UserRecord
    {
        [ProtoMember(2)] public string Name;
        [ProtoMember(3)] public string EmailAddress;
        [ProtoMember(4)] public string Username;
        [ProtoMember(5)] public string Password;
        [ProtoMember(6)] public UserType Type;
        [ProtoMember(7)] public string[] Businesses;
        [ProtoMember(8)] public List<UserSession> Sessions = new List<UserSession>();
        [ProtoMember(9)] public byte[] Session;
        [ProtoMember(10)] public Guid? MultifactorAuthentication;
        [ProtoMember(11)] public bool Verified;
        [ProtoMember(14)] public byte[] PasswordResetToken;
        [ProtoMember(13)] public DateTime PasswordResetTokenExpiry;

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
}
