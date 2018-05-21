using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using NLog;
using TechChallenge.Repositories;
using TechChallenge.Services;

namespace TechChallenge
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private const string DataFileName = @"Data\data.json";

        protected void Application_Start()
        {
            Logger.Info("--------------------App startup--------------------");

            GlobalConfiguration.Configure(WebApiConfig.Register);

            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<WarehouseService>().As<IWarehouseService>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().SingleInstance().WithParameter("dataFileName", DataFileName);
            builder.RegisterType<ProductRepository>().As<IProductRepository>().SingleInstance().WithParameter("dataFileName", DataFileName);
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<ProductService>().As<IProductService>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
