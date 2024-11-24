using CompanyManager.API.Library.Common;
using CompanyManager.Library.Models;
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
        
        _logger?.LogInformation($"Pobrałem notatki dla użytkownika {id}");

        return result;
    }
}
