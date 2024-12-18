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

    [ObservableProperty]
    private string? selectedNoteInit;
    [ObservableProperty]
    private string? selectedNoteTitleInit;

    [ObservableProperty]
    private bool hasChanges = false;

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
            if(SelectedNote.HTMLDescription != SelectedNoteInit)
            {
                HasChanges = true;
            }
            else
            {
                HasChanges = false;
            }
        }
    }

    async partial void OnSelectedNoteChanged(Note? oldValue, Note? newValue)
    {
        if (newValue != null)
        {
            // Twoja akcja na zmianę SelectedNote  // cos innego bo selectednote juz tu jest inne chyba
            if(oldValue != null)
            {
                if (App.User is not null && App.User.Token is not null && (oldValue.HTMLDescription != SelectedNoteInit || oldValue.Title != SelectedNoteTitleInit))
                     await NoteService.UpdateNoteAsync(App.User.Token.Token, oldValue);
            }

            WeakReferenceMessenger.Default.Send(newValue.HTMLDescription, "NotesView");
            NoteDetailsVisibility = Visibility.Visible;
            SelectedNoteInit = newValue.HTMLDescription;
            SelectedNoteTitleInit = newValue.Title;
            HasChanges = false;
            
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
    public async Task SaveNote()
    {
        if (App.User is not null && App.User.Token is not null && SelectedNote is not null)
        {
            if (HasChanges)
            {
                var result = await NoteService.UpdateNoteAsync(App.User.Token.Token, SelectedNote);
                if (result)
                {
                    SelectedNoteInit = SelectedNote.HTMLDescription;
                    SelectedNoteTitleInit = SelectedNote.Title;
                    HasChanges = false;
                }
            }
            
        }
    }

    [RelayCommand]
    public async Task AddNewNote()
    {
        if (App.User is not null && App.User.Token is not null)
        {
            var result = await NoteService.CreateEmptyNoteAsync(App.User.Token.Token);

            if (string.IsNullOrWhiteSpace(result))
            {
                MessageBox.Show("Błąd przy dodawaniu nowej notatki");
                return;
            }

            var parsingResult = int.TryParse(result, out int resultInt);

            if (!parsingResult)
            {
                MessageBox.Show("Błąd przy dodawaniu nowej notatki");
                return;
            }

            ListOfNotes.Add(new Note()
            {
                Id = resultInt,
                HTMLDescription = "",
                Title = "nowa notatka"
            });

        }
    }

    [RelayCommand]
    public async Task DeleteNoteAsync()
    {
        if (App.User is not null && App.User.Token is not null && SelectedNote != null)
        {
            var confirmationResult = MessageBox.Show(
            $"Czy na pewno chcesz usunąć notatkę \"{SelectedNote.Title}\"?",
            "Potwierdzenie usunięcia",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            // Jeśli użytkownik wybrał "No", zakończ metodę
            if (confirmationResult != MessageBoxResult.Yes)
            {
                return;
            }


            var result = await NoteService.DeleteNoteAsync(App.User.Token.Token, SelectedNote.Id);

            if (!result)
            {
                MessageBox.Show("Błąd przy usuwaniu notatki");
                return;
            }

            ListOfNotes.Remove(SelectedNote);
            SelectedNote = null;
            SelectedNoteInit = "";
            SelectedNoteTitleInit = "";
            NoteDetailsVisibility = Visibility.Collapsed;
            HasChanges = false;
        }
     }

}
