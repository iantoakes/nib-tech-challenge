using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using NLog;
using TechChallenge.Services;

namespace TechChallenge
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            Logger.Info("--------------------App startup--------------------");

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<WarehouseService>().As<IWarehouseService>().SingleInstance();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
