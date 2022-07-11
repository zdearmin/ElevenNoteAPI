using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Services.Note;
using ElevenNote.Models.Note;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElevenNote.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;
        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        // POST api/Note
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreate request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _noteService.CreateNoteAsync(request))
            {
                return Ok("Note created successfully.");
            }
            return BadRequest("Note could not be created.");

        }

        // GET api/Note
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NoteListItem>),200)]
        public async Task<IActionResult> GetAllNotes()
        {
            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }

        // GET api/Note/{Id}
        [HttpGet("{noteId:int}")]
        [ProducesResponseType(typeof(IEnumerable<NoteDetail>),200)]
        public async Task<IActionResult> GetNoteById([FromRoute] int noteId)
        {
            var detail = await _noteService.GetNoteByIdAsync(noteId);

            // Similar to our service method, we're using a ternary to determine our return type
            // If the returned value (detail) is not null, return it with a 200 Ok
            // Otherwise return a NotFound() 404 response
            return detail is not null ? Ok(detail) : NotFound();
        }

        // PUT api/Note
        [HttpPut]
        public async Task<IActionResult> UpdateNoteById([FromBody] NoteUpdate request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await _noteService.UpdateNoteAsync(request) ? Ok("Note updated successfully.") : BadRequest("Note could not be updated.");
        }

        // DELETE api/Note/{Id}
        [HttpDelete("{noteId:int}")]
        public async Task<IActionResult> DeleteNote([FromRoute] int noteId)
        {
            return await _noteService.DeleteNoteAsync(noteId) ? Ok($"Note {noteId} was deleted successfully.") : BadRequest($"Note {noteId} could not be deleted.");
        }
    }
}
