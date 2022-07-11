using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ElevenNote.Data;
using ElevenNote.Models.Note;

namespace ElevenNote.Services.Note
{
    public class NoteService : INoteService
    {
        private readonly int _userId;
        private readonly ApplicationDbContext _dbContext;
        public NoteService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var value = userClaims.FindFirst("Id")?.Value;
            var validId = int.TryParse(value, out _userId);
            if (!validId)
            {
                throw new Exception("Attempted to build NoteService without User Id claim.");
            }

            _dbContext = dbContext;
        }

        public async Task<bool> CreateNoteAsync(NoteCreate request)
        {
            var noteEntity = new NoteEntity
            {
                Title = request.Title,
                ContentDisposition = request.Content,
                CreatedUtc = DateTimeOffset.Now,
                OwnerId = _userId
            };

            _dbContext.Notes.Add(noteEntity);

            var numberOfChanges = await _dbContext.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var notes = await _dbContext.Notes
            .Where(entity => entity.OwnerId == _userId)
            .Select(entity => new NoteListItem
            {
                Id = entity.Id,
                Title = entity.Title,
                CreatedUtc = entity.CreatedUtc
            })
            .ToListAsync();

            return notes;
        }
    }
}