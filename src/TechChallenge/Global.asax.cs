using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using NLog;

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
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, DataFileName);
            builder.Register(c => path).Keyed<string>("DataFilePath");
            RegisterAutofacModules(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private void RegisterAutofacModules(ContainerBuilder builder)
        {
            Logger.Info(() => "Registering Autofac modules in referenced application assemblies");

            var executingUri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string executingPath = Path.GetDirectoryName(executingUri.LocalPath);

            var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
            var enumerateFiles = Directory.EnumerateFiles(HttpRuntime.AppDomainAppPath, "*.dll", SearchOption.AllDirectories).ToList();
            assemblies.AddRange(
                enumerateFiles
                    .Where(filename => Regex.IsMatch(filename, @"TechChallenge\.[A-Za-z]+\.dll"))
                    .Select(Assembly.LoadFrom)
            );

            Logger.Info(() => $"{assemblies.Count} application assemblies found");
            assemblies.ForEach(a => builder.RegisterAssemblyModules(a));
        }

    }
}
