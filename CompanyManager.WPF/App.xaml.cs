using CommunityToolkit.Mvvm.Messaging;
using CompanyManager.Library.Models.Models;
using CompanyManager.Library.Services;
using CompanyManager.WPF.ViewModels;
using CompanyManager.WPF.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace CompanyManager.WPF;

public partial class App : Application
{
    // Właściwość ServiceProvider umożliwia dostęp do kontenera DI
    public static IServiceProvider? ServiceProvider { get; private set; }
    public static UserModel? User { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Konfiguracja usług
        var services = new ServiceCollection();

        // Rejestracja ViewModel-i i usług
        ConfigureServices(services);

        // Zbudowanie kontenera DI
        ServiceProvider = services.BuildServiceProvider();

        

        // Uruchomienie pierwszego okna (np. LoginWindow)
        var loginWindow = new LoginWindow
        {
            DataContext = ServiceProvider.GetRequiredService<LoginWindowViewModel>()
        };

        WeakReferenceMessenger.Default.Register<UserModel>(this, (r, userModel) =>
        {
            User = userModel;

            var mainWindowViewModel = App.ServiceProvider!.GetRequiredService<MainWindowViewModel>();

            // Możesz przekazać userModel do MainWindowViewModel, jeśli potrzebujesz

            // Utworzenie i pokazanie MainWindow
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            mainWindow.Show();

            loginWindow.Close();
        });

        loginWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Rejestracja ViewModel-i
        services.AddSingleton<LoginWindowViewModel>();
        services.AddTransient<HttpClient>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<NotesViewModel>();
        services.AddSingleton<NotesService>();

        // Rejestracja serwisów (przykład)
        services.AddSingleton<AuthService>();
    }
}
