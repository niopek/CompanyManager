using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.WPF.ViewModels;

public partial class LoginWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _Test = "abc";
}

