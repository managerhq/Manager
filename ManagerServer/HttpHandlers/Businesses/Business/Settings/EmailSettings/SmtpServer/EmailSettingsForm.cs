using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.EmailSettings.SmtpServer
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.SmtpServer))]
    [Guide("The `SmtpServer` form connects Manager to your email provider's outgoing mail server.")]
    [Guide("SMTP (Simple Mail Transfer Protocol) is the standard technology used for sending emails across the internet.")]
    [Guide("You'll need to obtain SMTP server details from your email provider to complete this setup.")]
    [Guide("Complete these fields with information from your email provider:")]
    [Fields(typeof(ManagerServer.Model.EmailSettings))]
    [Guide("Before saving your settings, click the `TestEmailSettings` button to verify your configuration.")]
    [Guide("Manager will attempt to send a test email to confirm the SMTP connection is working correctly.")]
    [Guide("This helps identify any configuration issues before you start sending actual business emails.")]
    [DefaultButtonScreenshot(nameof(Strings.TestEmailSettings))]
    [Guide("If the test email fails, follow these troubleshooting steps:")]
    [Guide("• Double-check your SMTP server address, port number, and authentication settings")]
    [Guide("• Verify your username and password are correct (some providers require app-specific passwords)")]
    [Guide("• Ensure your firewall or antivirus isn't blocking the SMTP connection")]
    [Guide("• Test the same settings in another email client like Mozilla Thunderbird to isolate the issue")]
    [Guide("Once the test email succeeds, click the `Update` button to save your SMTP configuration.")]
    [Guide("Your email settings will be stored securely and used whenever you send emails from Manager.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    [Guide("After saving, you'll see a new `Email` button appear on transactions and reports throughout Manager.")]
    [Guide("This button lets you instantly email documents to customers and suppliers without leaving the program.")]
    [Guide("The email will include the document as a PDF attachment and use your configured SMTP settings.")]
    [DefaultButtonScreenshot(nameof(Strings.Email))]
    [Guide("Gmail users must follow these specific steps for security:")]
    [Guide("1. Enable 2-step verification in your Google account settings")]
    [Guide("2. Generate an app-specific password for Manager (Google Account → Security → App passwords)")]
    [Guide("3. Use this app-specific password instead of your regular Gmail password")]
    [Guide("4. Set the SMTP server to `smtp.gmail.com` and port to `587` with TLS enabled")]
    [Guide("Google requires app-specific passwords to protect your main account credentials.")]
    [Guide("Yahoo Mail users need to create an app-specific password:")]
    [Guide(@"1. Go to Yahoo Account Security (https://login.yahoo.com/account/security)")]
    [Guide(@"2. Click on 'Generate app password' under 'Account Security'")]
    [Guide(@"3. Select 'Other app' and enter 'Manager' as the app name")]
    [Guide(@"4. Click 'Generate' to create your app password")]
    [Guide(@"5. Copy the generated password and paste it into Manager's `Password` field")]
    [Guide(@"6. Use `smtp.mail.yahoo.com` as the SMTP server with port `587` or `465`")]
    internal sealed class EmailSettingsForm : NakedVueForm<ManagerServer.Model.EmailSettings>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.EmailSettings>();
        }

        protected override void InnerGet4()
        {
            Hr();
            using (Div(@class: "form-group mt-4"))
            {
                using (Button(onclick: "javascript:email()", id: "email-btn", @class: "btn btn-outline")) Write(Strings.TestEmailSettings);
                Write(@"<img src=""resources/ajax-loader.gif"" style=""display: none; margin-left: 10px; margin-right: 10px"" id=""email-ajax-indicator"" />");
            }

            using (Div(style: "color: red; font-weight: bold; display: none; margin-top: 10px", id: "emailError")) { }
            using (Div(style: "color: green; font-weight: bold; display: none; margin-top: 10px", id: "emailSuccess")) { }

            using (Script())
            {
                Write(@"function email() { 
	$('#email-ajax-indicator').show();
    $('#emailError').hide();
    $('#emailSuccess').hide();
    $('#email-btn').prop('disabled', true);
	$.ajax({
			url: '" + new EmailTest() { Business = Business }.ToUrl() + @"',
			type: 'post',
			data: { Json: JSON.stringify(app.$data) },
	        // callback handler that will be called on success
			success: function(response, textStatus, jqXHR){
                $('#email-btn').prop('disabled', false);
				$('#email-ajax-indicator').hide();
                $('#emailSuccess').html(jqXHR.responseText);
                $('#emailSuccess').show();
			},
			// callback handler that will be called on error
			error: function(jqXHR, textStatus, errorThrown){
				$('#email-ajax-indicator').hide();
				$('#email-btn').prop('disabled', false);
                $('#emailError').html(jqXHR.responseText);
                $('#emailError').show();
			}
		});
}");
            }
        }
    }
}
