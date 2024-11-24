using CompanyManager.API.Library.Common;
using CompanyManager.API.Library.Services;
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
    }
}
