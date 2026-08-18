using ManagerServer.Attributes;
using ManagerServer.Globalization;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    [Title(nameof(Strings.Guides))]
    [Guide("Manager.io is a versatile accounting software that caters to the unique needs of diverse businesses.")]
    [Guide("Customize the software by activating only the modules you need, adding custom fields to capture business-specific data, and generating reports tailored to your operations.")]
    [Guide("For example, a retail store may activate the *Inventory Items* tab, while a consulting firm might prioritize *Billable Time*.")]
    [Header("Available Editions")]
    [Guide("Manager.io is available in three editions: **Desktop Edition**, **Cloud Edition** and **Server Edition**.")]
    [Guide("All editions have all the modules and features. The only difference is where the software is running.")]
    [Guide("**Desktop Edition** is installed on your computer whether it's Windows, Mac or Linux. It's free forever to use, however due to its nature it doesn't support multi-user access.")]
    [Guide(@"**Cloud Edition** is hosted in the cloud. There is nothing to install and users can access the software from any computer or mobile device through a web browser. *Cloud Edition* also supports multi-user access. <a href=""cloud-edition"">Sign up for free trial</a>.")]
    [Guide("**Server Edition** is installed on your server.")]
    [Guide("Manager.io businesses are compatible across all editions and all operating systems. This means you can transfer your data between different editions and different operating systems with ease.")]
    [Header("Getting Started with Desktop Edition")]
    [Guide(@"To install *Desktop Edition*, go to **<a href=""download"">download page</a>** and download the program for your operating system.")]
    [Guide("Upon opening the *Desktop Edition* of Manager.io, you will be directed to the *Businesses* screen.")]
    [TopLevelTabScreenshot(icon: "fa-building", name: nameof(Strings.Businesses))]
    [LinkGuide("For more information, see:", typeof(Businesses.Businesses))]
    [Header("Getting Started with Cloud Edition")]
    [Guide("If you have signed up for *Cloud Edition*, access your cloud edition by visiting your login URL.")]
    [Guide(@"Enter your *Username* and *Password*. The default account username is ""administrator"".")]
    [Guide(@"If you've forgotten your *Password*, visit **<a href=""https://cloud.manager.io"">cloud.manager.io</a>** and use the **Forgot password** link to reset it.")]
    [Guide("Once logged in, the *Businesses* screen will appear, similar to the *Desktop Edition*.")]
    [LinkGuide("For more information, see:", typeof(Businesses.Businesses))]
    internal sealed class Default : HttpHandler
    {
        public override Task Get()
        {
            if (Edition.IsDesktop)
            {
                Response.Redirect(new Businesses.Businesses().ToUrl());
                return Task.CompletedTask;
            }

            if (this.GetCurrentUser() != null)
            {
                Response.Redirect(new Businesses.Businesses().ToUrl());
                return Task.CompletedTask;
            }

            Response.Redirect(new Login().ToUrl());
            return Task.CompletedTask;
        }
    }
}
