using CompanyManager.WPF.ViewModels;
using CompanyManager.WPF.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace CompanyManager.WPF;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }
    public static IConfiguration? Configuration { get; private set; }

    public App()
    {
        var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory());

        Configuration = builder.Build();

        AppHost = Host.CreateDefaultBuilder()
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<LoginWindowViewModel>();

            services.AddSingleton<LoginWindow>((serviceProvider) => new LoginWindow()
            {
                DataContext = serviceProvider.GetRequiredService<LoginWindowViewModel>()
            });
        }).Build();
    }
    

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startUpMainWindow = AppHost.Services.GetRequiredService<LoginWindow>();
        startUpMainWindow.Show();

        base.OnStartup(e);

    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        AppHost?.Dispose();
        base.OnExit(e);
    }
}
