using Autofac;
using bat.logic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.IoC
{
    public class Autofac
    {
        public static ContainerBuilder RegisterAll(ContainerBuilder builder)
        {
            if (builder == null)
                builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(t => t.BaseType == typeof(_ServiceClassBaseMarker))
                .AsSelf().InstancePerLifetimeScope();
            
            return builder;
        }
    }
}
