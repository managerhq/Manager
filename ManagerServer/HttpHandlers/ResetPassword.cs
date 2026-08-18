using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class ResetPassword : LoginTemplate
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(2)] public byte[] Token;
        [ProtoMember(3)] public bool InvalidToken;
        [ProtoMember(4)] public bool PasswordUpdated;
        [ProtoMember(5)] public bool PasswordMismatch;

        protected override void InnerInnerGet()
        {
            if (PasswordUpdated)
            {
                using (Div(@class: "text-green-600 font-bold")) Write(Strings.PasswordHasBeenReset);

                using (Div(@class: "flex gap-4 items-center"))
                {
                    using (DefaultLink(new Login().ToUrl())) Write(Strings.ReturnToLogin);
                }
                return;
            }

            var user = FindUserByToken(Username, Token).GetAwaiter().GetResult();

            if (user == null)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidOrExpiredResetLink);

                using (Div(@class: "flex gap-4 items-center"))
                {
                    using (DefaultLink(new Login().ToUrl())) Write(Strings.ReturnToLogin);
                }
                return;
            }

            using (Div())
            {
                using (Label()) Write(Strings.NewPassword);
                InputPassword(name: nameof(FormData.Password), autofocus: true, @class: "form-control");
            }

            using (Div())
            {
                using (Label()) Write(Strings.ConfirmPassword);
                InputPassword(name: nameof(FormData.ConfirmPassword), @class: "form-control");
            }

            if (PasswordMismatch)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.PasswordsDoNotMatch);
            }

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.ResetPassword);
                }
                using (DefaultLink(new Login().ToUrl())) Write(Strings.Cancel);
            }
        }

        public sealed class FormData
        {
            public string Password;
            public string ConfirmPassword;
        }

        protected override async Task InnerPost()
        {
            if (!Request.HasFormContentType) return;

            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Password = form[nameof(FormData.Password)],
                ConfirmPassword = form[nameof(FormData.ConfirmPassword)]
            };

            if (string.IsNullOrWhiteSpace(formData.Password))
            {
                Response.Redirect(new ResetPassword { Username = Username, Token = Token, PasswordMismatch = true }.ToUrl());
                return;
            }

            if (formData.Password != formData.ConfirmPassword)
            {
                Response.Redirect(new ResetPassword { Username = Username, Token = Token, PasswordMismatch = true }.ToUrl());
                return;
            }

            var user = await FindUserByToken(Username, Token);
            if (user == null)
            {
                Response.Redirect(new ResetPassword { Username = Username, Token = Token, InvalidToken = true }.ToUrl());
                return;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(formData.Password);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = default;
            await ApplicationData.Users.Save(user);

            Response.Redirect(new ResetPassword { PasswordUpdated = true }.ToUrl());
        }

        private static async Task<UserRecord> FindUserByToken(string username, byte[] token)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            if (token == null) return null;
            if (token.Length == 0) return null;

            var user = await ApplicationData.Instance.Users.GetByUsernameAsync(username);

            if (user == null) return null;
            if (user.PasswordResetToken == null) return null;
            if (!user.PasswordResetToken.SequenceEqual(token)) return null;
            if (user.PasswordResetTokenExpiry < DateTime.UtcNow) return null;

            return user;
        }
    }
}
