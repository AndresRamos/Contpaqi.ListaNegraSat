using Autofac;
using Caliburn.Micro;
using ListaNegraSat.Presentation.WpfApp.ViewModels;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Actualizaciones;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Articulo69B;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Comprobantes;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Configuracion;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Contabilidad;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Empresas;
using ListaNegraSat.Presentation.WpfApp.ViewModels.Expedientes;
using MahApps.Metro.Controls.Dialogs;

namespace ListaNegraSat.Presentation.WpfApp.Config;

public static class IocContainerConfig
{
    public static ContainerBuilder AddWpfAppServices(this ContainerBuilder containerBuilder)
    {
        AddCaliburnMicroModuleServices(containerBuilder);
        AddMahappsServices(containerBuilder);
        RegisterViewModels(containerBuilder);
        return containerBuilder;
    }

    private static void AddCaliburnMicroModuleServices(ContainerBuilder containerBuilder)
    {
        //  register the single window manager for this container
        containerBuilder.Register<IWindowManager>(_ => new WindowManager()).InstancePerLifetimeScope();
        //  register the single event aggregator for this container
        containerBuilder.Register<IEventAggregator>(_ => new EventAggregator()).InstancePerLifetimeScope();
    }

    private static void AddMahappsServices(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterInstance(DialogCoordinator.Instance).ExternallyOwned();
    }

    private static void RegisterViewModels(ContainerBuilder containerBuilder)
    {
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
}
