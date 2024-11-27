using CompanyManager.API.Library.Services;
using CompanyManager.Library.Models;
using CompanyManager.Library.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CompanyManager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notes")]
    public class NoteController : Controller
    {
        private readonly NotesService _notesService;
        private readonly ILogger<NoteController> _logger;

        public NoteController(NotesService notesService, ILogger<NoteController> logger)
        {
            _notesService = notesService;
            _logger = logger;
        }
        [HttpGet("GetAllNotes")]
        public async Task<IActionResult> GetAllNotes()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _notesService.GetAllUserNotes(userId);

            if (result != null)
                return Ok(result);
            else 
                return NotFound();
        }

        [HttpPost("CreateNote")]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _notesService.CreateNoteAsync(userId, note.Title, note.HTMLDescription);

            if (result > 0)
                return Ok($"Note created successfully with ID {result}");
            else
                return StatusCode(500, "An error occurred while creating the note.");
        }

        [HttpPost("CreateEmptyNote")]
        public async Task<IActionResult> CreateEmptyNote()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _notesService.CreateEmptyNoteAsync(userId);

            if (result > 0)
                return Ok($"Note created successfully with ID {result}");
            else
                return StatusCode(500, "An error occurred while creating the note.");
        }

        [HttpDelete("DeleteNote/{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _notesService.DeleteNoteAsync(noteId, userId);

            if (result > 0)
                return Ok($"Note with ID {noteId} deleted successfully.");
            else
                return NotFound($"Note with ID {noteId} not found for the user.");
        }

        [HttpPut("UpdateNote/{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] Note note)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var result = await _notesService.UpdateNoteAsync(noteId, userId, note.Title, note.HTMLDescription);

            if (result > 0)
                return Ok($"Note with ID {noteId} updated successfully.");
            else
                return NotFound($"Note with ID {noteId} not found for the user.");
        }

    }
}
