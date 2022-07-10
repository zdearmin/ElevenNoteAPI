// Using System statement because DateTime type is being used
using System;
// Using this statement allows access to [Key], [Required], etc. attributes/annotations
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ElevenNote.Data.Entities
{
    public class UserEntity
    {
        // Define what the UserEntity looks like
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public List<NoteEntity> Notes { get; set; }
    }
}