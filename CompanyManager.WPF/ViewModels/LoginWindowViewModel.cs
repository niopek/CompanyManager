using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CompanyManager.Library.Models.Models;
using CompanyManager.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CompanyManager.WPF.ViewModels;

public partial class LoginWindowViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string loginText = "niopek", passwordText = "qo2eweesd", infoText = string.Empty;

    [ObservableProperty]
    private string registerLoginText = string.Empty, registerPasswordText = string.Empty, regitserInfoText = string.Empty, registerPasswordConfirmationText = string.Empty, registerEmailText = string.Empty;

    [ObservableProperty]
    private UserModel? _user;

    [ObservableProperty]
    private Visibility loginView, registerView;
    public LoginWindowViewModel(AuthService authservice)
    {
        _authService = authservice;
        LoginView = Visibility.Visible;
        RegisterView = Visibility.Collapsed;
    }


    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(LoginText) || string.IsNullOrWhiteSpace(PasswordText))
        {
            MessageBox.Show("Wprowadź login i hasło");
            return;
        }

        UserModel? user = null;

        try
        {
            user = await _authService.LoginAsync(new() { Username = LoginText, Password = PasswordText });
        }catch(Exception ex)
        {
            InfoText = ex.Message;
        }        

        if(user is null)
        {
            InfoText = "Błędny login lub hasło";
            return;
        }

        User = user;

        InfoText = $"Zalogowano {User.Username}";
        WeakReferenceMessenger.Default.Send(User);

    }
    [RelayCommand]
    private void GoToRegisterPage()
    {
        LoginView = Visibility.Collapsed;
        RegisterView = Visibility.Visible;
    }

    [RelayCommand]
    private void GoToBackToLoginPage()
    {
        LoginView = Visibility.Visible;
        RegisterView = Visibility.Collapsed;
    }

    [RelayCommand]
    private async Task Register()
    {
        // Walidacja wejścia
        if (string.IsNullOrWhiteSpace(RegisterLoginText) ||
            string.IsNullOrWhiteSpace(RegisterEmailText) ||
            string.IsNullOrWhiteSpace(RegisterPasswordText) ||
            string.IsNullOrWhiteSpace(RegisterPasswordConfirmationText))
        {
            RegitserInfoText = "Wszystkie pola muszą być wypełnione.";
            return;
        }

        // Sprawdzenie zgodności hasła z potwierdzeniem
        if (RegisterPasswordText != RegisterPasswordConfirmationText)
        {
            RegitserInfoText = "Hasła nie są zgodne.";
            return;
        }

        // Tworzenie modelu żądania
        var registerRequest = new RegisterUserRequestModel
        {
            Username = RegisterLoginText,
            Email = RegisterEmailText,
            Password = RegisterPasswordText
        };

        try
        {
            // Wywołanie metody rejestracji
            var responseMessage = await _authService.RegisterUserAsync(registerRequest);

            // Obsługa pomyślnego wyniku
            RegitserInfoText = responseMessage; // np. "User created successfully."
        }
        catch (HttpRequestException ex)
        {
            // Obsługa błędów z serwera
            RegitserInfoText = ex.Message; // np. "Registration failed: <error>"
        }
        catch (Exception ex)
        {
            // Obsługa innych błędów
            RegitserInfoText = $"Wystąpił błąd: {ex.Message}";
        }
    }

}

