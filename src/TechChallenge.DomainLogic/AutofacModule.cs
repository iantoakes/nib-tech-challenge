using Autofac;
using Autofac.Features.AttributeFilters;
using TechChallenge.DomainLogic.Repositories;
using TechChallenge.DomainLogic.Services;

namespace TechChallenge.DomainLogic
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WarehouseService>().As<IWarehouseService>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().SingleInstance().WithAttributeFiltering();
            builder.RegisterType<ProductRepository>().As<IProductRepository>().SingleInstance().WithAttributeFiltering();
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<ProductService>().As<IProductService>();

            base.Load(builder);
        }
    }
}
