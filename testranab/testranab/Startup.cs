using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testranab.Startup))]
namespace testranab
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
