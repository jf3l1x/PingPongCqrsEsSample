using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Constant.Module.Interfaces;
using Constant.Module.Interfaces.Bus;
using Constant.Module.Interfaces.Configuration;
using Constant.Module.Interfaces.Persistence.ReadModel;
using LightInject;
using Owin;
using Ping.Shared;
using Ping.Shared.Model.Read;
using Ping.Shared.Services;
using Ping.Web.Persistence.Dapper;
using Ping.Web.Services;
using Ping.Web.Services.Default;

namespace Ping.Web
{
    public class Engine : IWebModule
    {
        public IAppBuilder RegisterApi(IAppBuilder app, IWebModuleContainer configurationContainer)
        {
            HttpConfiguration configuration = CreateHttpConfiguration();
            ServiceContainer container = CreateContainer(configurationContainer);
            container.RegisterApiControllers();
            container.EnableWebApi(configuration);
            app.UseWebApi(configuration);


            return app;
        }


        private HttpConfiguration CreateHttpConfiguration()
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();

            // OData
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<PingSummary>("PingSummaries");

            configuration.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            return configuration;
        }

        private ServiceContainer CreateContainer(IWebModuleContainer factory)
        {
            var container = new ServiceContainer();

            container.Register<IRouteMessages, MessageRouter>();
            container.Register<IMutateMessages, DefaultMessageMutator>();
            container.Register<IResolveTypeName,TypeNameResolver>();
            container.Register<IConnectionFactory,SqlConnectionFactory>();
            container.RegisterInstance( factory.CreateBus(null,
                   new[] {container.GetInstance<IMutateMessages>()}, container.GetInstance<IRouteMessages>(),
                   new[] { new TypeNameResolver() }));


            container.Register<IReadFromRepository<PingSummary>, Repository>();

            return container;
        }
    }
}