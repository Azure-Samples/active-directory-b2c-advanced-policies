using Owin;

namespace B2CAppMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            ConfigureAuthentication(appBuilder);
        }
    }
}
