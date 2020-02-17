using Autofac;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.CommandProcessor;
using Project.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Project.Web.AppStart
{
    public class DependencyModule : Autofac.Module
    {
        //public IConfiguration Configuration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            // The generic ILogger<TCategoryName> service was added to the ServiceCollection by ASP.NET Core.
            // It was then registered with Autofac using the Populate method. All of this starts
            // with the services.AddAutofac() that happens in Program and registers Autofac
            // as the service provider.
            //builder.Register(c => new ValuesService(c.Resolve<ILogger<ValuesService>>()))
            //    .As<IValuesService>()
            //    .InstancePerLifetimeScope();

            // Register Entity Framework
            //var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            //builder.RegisterType<ApplicationDbContext>()
            //    .WithParameter("options", dbContextOptionsBuilder.Options)
            //    .InstancePerLifetimeScope();

            //var identityOptions = new IdentityOptions<IdentityUser>()

            AutowireAssemblies(builder);
            SetupCustomRules(builder);
        }

        private static void AutowireAssemblies(ContainerBuilder builder)
        {
            var assembly = Assembly.GetEntryAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();


            builder.RegisterAssemblyTypes(Assembly.Load("ProjectWeb.Commands")).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.Load("Project.Web.Handlers")).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.Load("Project.RequestProcessor")).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.Load("Project.Services")).AsImplementedInterfaces();
        }

        private static void SetupCustomRules(ContainerBuilder builder)
        {
            // Instantiate only one logger
            // TODO: read log level from configuration
            //var logger = new Logger(Uptime.ProcessId, LogLevel.Debug);
            //builder.RegisterInstance(logger).As<ILogger>().SingleInstance();


            //builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            //var config = new Config(new ConfigData(logger));
            //builder.RegisterInstance(config).As<IConfig>().SingleInstance();
            //builder.RegisterGeneric(typeof(CosmoRepo<,>)).As(typeof(IRepository<,>)).InstancePerDependency();

            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
               .AsClosedTypesOf(typeof(IRequestHandler<,>))
               .AsImplementedInterfaces();

            //builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
            //   .AsClosedTypesOf(typeof(ICommandHandlerAsync<,>))
            //   .AsImplementedInterfaces();

            //builder.RegisterInstance(config.ServicesConfig).As<IServicesConfig>().SingleInstance();

            //var cosomoOptions = new CosomoConfig()
            //{
            //    CosmosDbKey = config.ServicesConfig.CosmosDbKey,
            //    CosmosDbThroughput = config.ServicesConfig.CosmosDbThroughput,
            //    CosmosDbUri = config.ServicesConfig.CosmosDbUri,
            //    DataBaseName = config.ServicesConfig.CosmosDbDatabaseName
            //};
            //builder.RegisterInstance(cosomoOptions).As<CosomoConfig>().SingleInstance();


            //builder.RegisterType<GuidKeyGenerator>().As<IKeyGenerator>().SingleInstance();
        }

    }
}
