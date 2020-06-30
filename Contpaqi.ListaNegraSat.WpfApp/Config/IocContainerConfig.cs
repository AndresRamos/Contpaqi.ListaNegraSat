using System.Reflection;
using Autofac;
using Caliburn.Micro;
using ListaNegraSat.Core.Application.Articulo69B.Queries.BuscarContribuyentes69B;
using ListaNegraSat.Core.Application.Cfdis.Interfaces;
using ListaNegraSat.Core.Application.Contribuyentes.Interfaces;
using ListaNegraSat.Core.Application.Empresas.Interfaces;
using ListaNegraSat.Core.Application.Expedientes.Interfaces;
using ListaNegraSat.Infrastructure.ContpaqiAdd.Factories;
using ListaNegraSat.Infrastructure.ContpaqiAdd.Repositories;
using ListaNegraSat.Infrastructure.ContpaqiContabilidad.Factories;
using ListaNegraSat.Infrastructure.ContpaqiContabilidad.Repositories;
using ListaNegraSat.Presentation.WpfApp.Models;
using ListaNegraSat.Presentation.WpfApp.ViewModels;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Actualizaciones;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Articulo69B;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Comprobantes;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Configuracion;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Contabilidad;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Empresas;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Expedientes;
using MahApps.Metro.Controls.Dialogs;
using MediatR;

namespace ListaNegraSat.Presentation.WpfApp.Config
{
    public static class IocContainerConfig
    {
        public static IContainer Configure()
        {
            var containerBuilder = new ContainerBuilder();
            RegisterWpfAppServices(containerBuilder);
            RegisterViewModels(containerBuilder);
            RegisterContpaqiContabilidadModule(containerBuilder);
            RegisterContpaqiAddModule(containerBuilder);

            return containerBuilder.Build();
        }

        private static void RegisterWpfAppServices(ContainerBuilder containerBuilder)
        {
            //  register the single window manager for this container
            containerBuilder.Register<IWindowManager>(c => new WindowManager()).InstancePerLifetimeScope();
            //  register the single event aggregator for this container
            containerBuilder.Register<IEventAggregator>(c => new EventAggregator()).InstancePerLifetimeScope();

            // Mahapps
            containerBuilder.RegisterInstance(DialogCoordinator.Instance).ExternallyOwned();

            //Mediatr
            containerBuilder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            containerBuilder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            containerBuilder.RegisterAssemblyTypes(typeof(BuscarContribuyentes69BQuery).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }

        private static void RegisterViewModels(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConfiguracionAplicacion>().SingleInstance();

            containerBuilder.RegisterType<ShellViewModel>().SingleInstance();
            containerBuilder.RegisterType<AcercaDeViewModel>();

            containerBuilder.RegisterType<Articulo69BListadoCompletoViewModel>();
            containerBuilder.RegisterType<EditarConfiguracionAplicacionViewModel>();
            containerBuilder.RegisterType<ContribuyentesContabilidadViewModel>();
            containerBuilder.RegisterType<SeleccionarEmpresaContabilidadViewModel>();
            containerBuilder.RegisterType<ComprobantesListaViewModel>();
            containerBuilder.RegisterType<ExpedientesListaViewModel>();
            containerBuilder.RegisterType<ActualizacionAplicacionViewModel>();
        }

        private static void RegisterContpaqiContabilidadModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(context =>
            {
                var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                return ContabilidadGeneralesDbContextFactory.Crear(configuracionAplicacion.ContpaqiContabilidadConnectionString);
            });

            containerBuilder.Register(context =>
            {
                var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                return ContabilidadEmpresaDbContextFactory.Crear(configuracionAplicacion.ContpaqiContabilidadConnectionString, configuracionAplicacion.EmpresaContabilidad.BaseDatos);
            });

            containerBuilder.RegisterType<EmpresaContabilidadRepository>().As<IEmpresaContabilidadRepository>();
            containerBuilder.RegisterType<ContribuyenteContabilidadRepository>().As<IContribuyenteContabilidadRepository>();
        }

        private static void RegisterContpaqiAddModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(context =>
            {
                var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                return AddDocumentMetadataDbContextFactory.Crear(configuracionAplicacion.ContpaqiAddConnetionString, configuracionAplicacion.EmpresaContabilidad.GuidCompany);
            });
            containerBuilder.Register(context =>
            {
                var configuracionAplicacion = context.Resolve<ConfiguracionAplicacion>();
                return AddOtherMetadataDbContextFactory.Crear(configuracionAplicacion.ContpaqiAddConnetionString, configuracionAplicacion.EmpresaContabilidad.GuidCompany);
            });

            containerBuilder.RegisterType<ComprobanteAddRepository>().As<IComprobanteAddRepository>();
            containerBuilder.RegisterType<ExpedienteAddRepository>().As<IExpedienteAddRepository>();
        }
    }
}