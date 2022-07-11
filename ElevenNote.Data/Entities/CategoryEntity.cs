using System.Security.AccessControl;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElevenNote.Data.Entities
{
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int NoteId { get; set; }
        public NoteEntity Note { get; set; }
        [Required]
        public string Title { get; set; }
    }
}