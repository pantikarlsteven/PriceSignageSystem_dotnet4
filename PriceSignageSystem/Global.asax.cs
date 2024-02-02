using Autofac;
using Autofac.Integration.Mvc;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using PriceSignageSystem.Models.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PriceSignageSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Create the DI container builder
            var builder = new ContainerBuilder();

            // Register the ApplicationDbContext
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();

            // Register your interfaces and implementations
            builder.RegisterType<STRPRCRepository>().As<ISTRPRCRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<TypeRepository>().As<ITypeRepository>();
            builder.RegisterType<SizeRepository>().As<ISizeRepository>();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>();
            builder.RegisterType<QueueRepository>().As<IQueueRepository>();
            builder.RegisterType<RegistersRepository>().As<IRegistersRepository>();
            builder.RegisterType<EditReasonRepository>().As<IEditReasonRepository>();

            // Register the MVC controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Build the container
            var container = builder.Build();

            // Set the MVC dependency resolver to use Autofac
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Enable session state
            //HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);

            BundleTable.EnableOptimizations = true;


        }
    }
}
