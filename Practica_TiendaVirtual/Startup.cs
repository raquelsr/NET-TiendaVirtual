using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Practica_TiendaVirtual.Startup))]
namespace Practica_TiendaVirtual
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
