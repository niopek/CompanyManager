using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CompanyManager.Library.Models.Models;
using CompanyManager.Library.Services;
using CompanyManager.WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CompanyManager.WPF.ViewModels;

public partial class NotesViewModel : ObservableObject
{

    [ObservableProperty]
    private ObservableCollection<Note> listOfNotes;
    [ObservableProperty]
    private Note? selectedNote;


    [ObservableProperty]
    private NotesService _noteService;

    [ObservableProperty]
    private Visibility _noteDetailsVisibility;

    public NotesViewModel(NotesService notesService)
    {
        NoteService = notesService;
        ListOfNotes =  new();
        Task.Run(async () => { await GetNotes(); });

        WeakReferenceMessenger.Default.Register<string, string>(this, "NotesViewModel", (r, message) =>
        {
            HandleMessage(message);
        });

        NoteDetailsVisibility = Visibility.Collapsed;
        
    }

    private void HandleMessage(string message)
    {
        if(SelectedNote is not null)
        {
            SelectedNote.HTMLDescription = message;
        }
    }

    partial void OnSelectedNoteChanged(Note? oldValue, Note? newValue)
    {
        if (newValue != null)
        {
            // Twoja akcja na zmianę SelectedNote
            WeakReferenceMessenger.Default.Send(newValue.HTMLDescription, "NotesView");
            NoteDetailsVisibility = Visibility.Visible;
        }
    }

    public async Task GetNotes()
    {
        if(App.User is not null && App.User.Token is not null)
        {
            ListOfNotes = new ObservableCollection<Note>(await NoteService.GetAllNotesAsync(App.User.Token.Token));
        }
    }
    [RelayCommand]
    private void Test()
    {
        NoteDetailsVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    public async Task SaveNote()
    {
        if (App.User is not null && App.User.Token is not null && SelectedNote is not null)
        {
            var result = await NoteService.UpdateNoteAsync(App.User.Token.Token, SelectedNote);
        }
    }

}
