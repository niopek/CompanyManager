using CompanyManager.API.Library.Common;
using CompanyManager.Library.Models;
using CompanyManager.Library.Models.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.API.Library.Services;

public class NotesService
{
    Database _db;
    private readonly ILogger<NotesService>? _logger;
    public NotesService(Database db, ILogger<NotesService>? logger = null)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Note>> GetAllUserNotes(int id)
    {
        
        var result = await _db.LoadDataAsyncSP<Note, dynamic>("GetAllUserNotes", new { UserId = id });
        
        _logger?.LogInformation($"Notes downloaded for User id: {id}");

        return result;
    }

    public async Task<int> CreateNoteAsync(int userId, string title, string note)
    {
        var returnValue = await _db.SaveDataSP("CreateNote", new { UserId = userId, Title = title, Note = note });

        _logger?.LogInformation($"Created a new note for user {userId} with title: {title}");

        return returnValue;
    }

    public async Task<int> CreateEmptyNoteAsync(int userId)
    {
        var returnValue = await _db.SaveDataSP("CreateEmptyNote", new { UserId = userId });

        _logger?.LogInformation($"Created a new empty note for user {userId}");

        return returnValue;
    }

    public async Task<int> DeleteNoteAsync(int noteId, int userId)
    {
        var returnValue = await _db.SaveDataSP("DeleteNote", new { NoteId = noteId, UserId = userId });

        _logger?.LogInformation($"Deleted note {noteId} for user {userId}");

        return returnValue;
    }

    public async Task<int> UpdateNoteAsync(int noteId, int userId, string title, string note)
    {
        var returnValue = await _db.SaveDataSP("UpdateNote", new { NoteId = noteId, UserId = userId, Title = title, Note = note });

        _logger?.LogInformation($"Updated note {noteId} for user {userId} with new title: {title}");

        return returnValue;
    }


}
