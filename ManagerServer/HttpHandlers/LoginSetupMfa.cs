using System;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    [Title(nameof(Strings.EnforceMultifactorAuthentication))]
    [Guide("Multi-factor authentication (MFA) adds an extra layer of security to your account by requiring a verification code in addition to your password.")]
    [Guide("This helps protect your account even if your password is compromised.")]
    [Header("Setting Up Multi-Factor Authentication")]
    [Guide("To enable MFA on your account, you will need an authenticator app on your mobile device.")]
    [Guide("Popular authenticator apps include Google Authenticator, Microsoft Authenticator, Authy, or any app that supports TOTP (Time-based One-Time Password) codes.")]
    [Header("Setup Steps")]
    [Guide("1. Install an authenticator app on your mobile device if you haven't already.")]
    [Guide("2. Open the authenticator app and select the option to add a new account.")]
    [Guide("3. Scan the QR code displayed on this page using your authenticator app.")]
    [Guide("4. The app will generate a 6-digit verification code that changes every 30 seconds.")]
    [Guide("5. Enter the current verification code in the `Authentication Code` field below.")]
    [Guide("6. Click `Update` to complete the setup.")]
    [Header("Important Notes")]
    [Guide("Keep your authenticator app installed and accessible. You will need it every time you log in.")]
    [Guide("If you lose access to your authenticator app, you may need to contact your system administrator to regain access to your account.")]
    [Guide("Some authenticator apps allow you to back up your codes to the cloud. Consider enabling this feature to prevent lockouts.")]
    internal sealed class LoginSetupMfa : LoginTemplate
    {
        [ProtoMember(1)] public bool InvalidAuthenticationCode;
        [ProtoMember(2)] public string Issuer;

        protected override void InnerInnerGet()
        {
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                Response.Redirect("/");
                return;
            }

            if (!currentUser.MultifactorAuthentication.HasValue)
            {
                Response.Redirect("/");
                return;
            }

            var tfa = new Google.Authenticator.TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode(Issuer, currentUser.Username, currentUser.MultifactorAuthentication.Value.ToString(), false, 3);
            var qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;

            using (P()) Write(Strings.MultiFactorAuthenticationSetup);

            using (Div(@class: "flex")) Img(src: qrCodeImageUrl, @class: "border rounded");

            using (Div())
            {
                using (Label()) Write(Strings.AuthenticationCode);
                InputText(name: nameof(FormData.AuthenticationCode), @class: "form-control");
            }

            if (InvalidAuthenticationCode)
            {
                using (Div(@class: "text-red-600 font-bold")) Write(Strings.InvalidAuthenticationCode);
            }

            using (Div(@class: "flex gap-4 items-center"))
            {
                using (PrimaryButton())
                {
                    I(@class: "htmx-indicator me-2 fas fa-circle-notch fa-spin !hidden");
                    Write(Strings.Update);
                }
            }
        }

        public sealed class FormData
        {
            public string AuthenticationCode;
        }

        protected override async Task InnerPost()
        {
            if (!Request.HasFormContentType) return;

            var currentUser = GetCurrentUser();

            if (currentUser == null || !currentUser.MultifactorAuthentication.HasValue)
            {
                Response.Redirect("/");
                return;
            }

            var form = await Request.ReadFormAsync();
            var formData = new FormData()
            {
                AuthenticationCode = form[nameof(FormData.AuthenticationCode)]
            };

            if (!string.IsNullOrWhiteSpace(formData.AuthenticationCode))
            {
                var tfa = new Google.Authenticator.TwoFactorAuthenticator();
                var result = tfa.ValidateTwoFactorPIN(currentUser.MultifactorAuthentication.Value.ToString(), formData.AuthenticationCode);

                if (result)
                {
                    currentUser.Verified = true;
                    await ApplicationData.Users.Save(currentUser);
                    Response.Redirect(new Businesses.Businesses().ToUrl());
                }
            }

            Response.Redirect(new LoginSetupMfa() { InvalidAuthenticationCode = true, Issuer = Issuer }.ToUrl());
            return;
        }
    }
}
