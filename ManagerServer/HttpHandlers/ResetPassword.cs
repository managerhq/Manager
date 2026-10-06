using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    internal sealed class ResetPassword : LoginTemplate
    {
        [ProtoMember(1)] public string Username;
        [ProtoMember(3)] public bool InvalidCode;
        [ProtoMember(4)] public bool PasswordUpdated;
        [ProtoMember(5)] public bool PasswordMismatch;
        [ProtoMember(6)] public bool CodeSent;
        [ProtoMember(7)] public bool TooManyAttempts;

        // Codes are short enough to be typed, so they are short enough to guess if guessing is free. Every
        // attempt in the server goes through one gate that holds each slot for 100ms, capping the rate at
        // roughly 10 per second no matter how many requests arrive in parallel. Against 2^40 codes that is
        // about 36,000 guesses inside a code's one-hour life — a 1 in 30 million chance of landing one.
        private static readonly Helpers.AttemptThrottle throttle = new Helpers.AttemptThrottle(
            interval: TimeSpan.FromMilliseconds(100),
            maxWait: TimeSpan.FromSeconds(5));

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

            if (CodeSent)
            {
                using (Div(@class: "text-green-600 font-bold")) Write(Strings.PasswordResetCodeSent);
            }

            using (Div())
            {
                using (Label()) Write(Strings.Username);
                InputText(name: nameof(FormData.Username), value: Username, @class: "form-control");
            }

            using (Div())
            {
                using (Label()) Write(Strings.ResetCode);
                InputText(name: nameof(FormData.Code), autofocus: true, @class: "form-control", maxlength: 16, autocomplete: "one-time-code", autocapitalize: false, placeholder: "XXXX-XXXX");
            }

            if (InvalidCode)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidOrExpiredResetCode);
            }

            if (TooManyAttempts)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.TooManyResetAttempts);
            }

            using (Div())
            {
                using (Label()) Write(Strings.NewPassword);
                InputPassword(name: nameof(FormData.Password), @class: "form-control");
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
            public string Username;
            public string Code;
            public string Password;
            public string ConfirmPassword;
        }

        protected override async Task InnerPost()
        {
            if (!Request.HasFormContentType) return;

            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                Username = form[nameof(FormData.Username)].ToString().Trim().ToLowerInvariant(),
                Code = form[nameof(FormData.Code)],
                Password = form[nameof(FormData.Password)],
                ConfirmPassword = form[nameof(FormData.ConfirmPassword)]
            };

            if (string.IsNullOrWhiteSpace(formData.Password))
            {
                Response.Redirect(new ResetPassword { Username = formData.Username, PasswordMismatch = true }.ToUrl());
                return;
            }

            if (formData.Password != formData.ConfirmPassword)
            {
                Response.Redirect(new ResetPassword { Username = formData.Username, PasswordMismatch = true }.ToUrl());
                return;
            }

            var attempt = await throttle.Run(() => FindUserByCode(formData.Username, formData.Code));

            if (!attempt.Entered)
            {
                Response.Redirect(new ResetPassword { Username = formData.Username, TooManyAttempts = true }.ToUrl());
                return;
            }

            var user = attempt.Result;
            if (user == null)
            {
                Response.Redirect(new ResetPassword { Username = formData.Username, InvalidCode = true }.ToUrl());
                return;
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(formData.Password);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = default;
            await ApplicationData.Users.Save(user);

            Response.Redirect(new ResetPassword { PasswordUpdated = true }.ToUrl());
        }

        private static async Task<UserRecord> FindUserByCode(string username, string code)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            var supplied = Helpers.PasswordResetCode.Normalize(code);
            if (supplied == null) return null;

            var user = await ApplicationData.Instance.Users.GetByUsernameAsync(username);

            if (user == null) return null;
            if (!Helpers.PasswordResetCode.Matches(user.PasswordResetToken, supplied)) return null;
            if (user.PasswordResetTokenExpiry < DateTime.UtcNow) return null;

            return user;
        }
    }
}
