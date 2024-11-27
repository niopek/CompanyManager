
using CompanyManager.Library.Services;

AuthService authService = new(new HttpClient());

var user = await authService.LoginAsync(new CompanyManager.Library.Models.Models.LoginRequestModel() { Username = "niopek", Password = "qo2eweesd" });

NotesService notesService = new(new HttpClient());

if(user != null && user.Token != null)
{
    var result = await notesService.GetAllNotesAsync(user.Token.Token);

    if(result != null)
    {
        foreach(var note in result)
        {
            Console.WriteLine(note.Title);
            Console.WriteLine(note.HTMLDescription);
        }
    }
}