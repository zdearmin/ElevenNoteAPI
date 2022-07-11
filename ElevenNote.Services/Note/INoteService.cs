using System.Security.AccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElevenNote.Models.Note;

namespace ElevenNote.Services.Note
{
    public interface INoteService
    {
        Task<bool> CreateNoteAsync(NoteCreate request);
        Task<IEnumerable<NoteListItem>> GetAllNotesAsync();
    }
}