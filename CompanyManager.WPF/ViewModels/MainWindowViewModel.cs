using CommunityToolkit.Mvvm.ComponentModel;
using CompanyManager.Library.Models.Models;
using CompanyManager.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject currentViewModel;

    public MainWindowViewModel(NotesViewModel notesViewModel)
    {
        CurrentViewModel = notesViewModel; 
    }



}
