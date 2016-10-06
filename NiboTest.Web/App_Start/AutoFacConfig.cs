using Autofac;
using Autofac.Integration.Mvc;
using NiboTest.Web;
using NiboTest.Web.Data;
using System.Web.Mvc;

namespace NiboTest
{
    public class AutoFacConfig
    {
        public static void Config()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterFilterProvider();            

            builder.RegisterType<Context>().AsSelf().InstancePerRequest();

            IContainer container = builder.Build();

            AutofacDependencyResolver dependencyResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(dependencyResolver);

        }
    }
}