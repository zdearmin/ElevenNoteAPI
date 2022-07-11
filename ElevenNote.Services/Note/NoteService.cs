using System.Security.Cryptography.X509Certificates;
using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ElevenNote.Data;
using ElevenNote.Data.Entities;
using ElevenNote.Models.Note;
using AutoMapper;

namespace ElevenNote.Services.Note
{
    public class NoteService : INoteService
    {
        private readonly int _userId;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public NoteService(IHttpContextAccessor httpContextAccessor, IMapper mapper, ApplicationDbContext dbContext)
        {
            var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var value = userClaims.FindFirst("Id")?.Value;
            var validId = int.TryParse(value, out _userId);
            if (!validId)
            {
                throw new Exception("Attempted to build NoteService without User Id claim.");
            }

            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> CreateNoteAsync(NoteCreate request)
        {
            // Setting the OwnerId at the same time we map to the new type
            // Replaced commented code below with:
            var noteEntity = _mapper.Map<NoteCreate, NoteEntity>(request, opt => opt.AfterMap((src, dest) => dest.OwnerId = _userId));

            // OR

            // var noteEntity = new NoteEntity
            // {
            //     Title = request.Title,
            //     Content = request.Content,
            //     CreatedUtc = DateTimeOffset.Now,
            //     OwnerId = _userId
            // };

            _dbContext.Notes.Add(noteEntity);

            var numberOfChanges = await _dbContext.SaveChangesAsync();
            return numberOfChanges == 1;
        }

        public async Task<IEnumerable<NoteListItem>> GetAllNotesAsync()
        {
            var notes = await _dbContext.Notes
            .Where(entity => entity.OwnerId == _userId)
            // Replaced commented code below with the _mapper.Map code
            // Takes in a generic to determine the destination type and the source as an argument to the method
            .Select(entity => _mapper.Map<NoteListItem>(entity))

            // OR

            // .Select(entity => new NoteListItem
            // {
            //     Id = entity.Id,
            //     Title = entity.Title,
            //     CreatedUtc = entity.CreatedUtc
            // })

            .ToListAsync();

            return notes;
        }

        public async Task<NoteDetail> GetNoteByIdAsync(int noteId)
        {
            // Find the first note that has the given Id and an OwnerId that matches the requesting userId
            var noteEntity = await _dbContext.Notes
            .FirstOrDefaultAsync(e => e.Id == noteId && e.OwnerId == _userId);

            // If noteEntity is null then return null, otherwise initialize and return a new NoteDetail
            return noteEntity is null ? null : _mapper.Map<NoteDetail>(noteEntity);

            // OR

            // : new NoteDetail
            // {
            //     Id = noteEntity.Id,
            //     Title = noteEntity.Title,
            //     Content = noteEntity.Content,
            //     CreatedUtc = noteEntity.CreatedUtc,
            //     ModifiedUtc = noteEntity.ModifiedUtc
            // };
        }

        public async Task<bool> UpdateNoteAsync(NoteUpdate request)
        {
            // THIS
            // Check the database to see if there's a note entity that matches the request information
            // Any returns true if any entity exists
            var noteIsUserOwned = await _dbContext.Notes.AnyAsync(note => note.Id == request.Id && note.OwnerId == _userId);

            // If the Any check returns false then we know the Note either does not exist
            // Or the note is not owned by the user
            if (!noteIsUserOwned)
            {
                return false;
            }

            // Map from Update to Entity and set OwnerId again
            var newEntity = _mapper.Map<NoteUpdate, NoteEntity>(request, opt => opt.AfterMap((src, dest) => dest.OwnerId = _userId));

            // Update the Entry State, which is another way to tell the DbContext something has changed
            _dbContext.Entry(newEntity).State = EntityState.Modified;

            // Because we don't currently have access to our CreatedUtc value, we'll just mark it as not modified
            _dbContext.Entry(newEntity).Property(e => e.CreatedUtc).IsModified = false;

            // OR

            // // Find the note and validate it's owned by the user
            // var noteEntity = await _dbContext.Notes.FindAsync(request.Id);

            // // By using the null conditional operator we can check if it's null at the same time we check the OwnerId
            // // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-
            // if (noteEntity?.OwnerId != _userId)
            // {
            //     return false;
            // }

            // // Now we update the entity's properties
            // noteEntity.Title = request.Title;
            // noteEntity.Content = request.Content;
            // noteEntity.ModifiedUtc = DateTimeOffset.Now;

            // Save the changes to the database and capture how many rows were updated
            var numberOfChanges = await _dbContext.SaveChangesAsync();

            // numberOfChanges is stated to be equal to 1 because only one row is updated
            return numberOfChanges == 1;
        }

        public async Task<bool> DeleteNoteAsync(int noteId)
        {
            // Find the note by the given Id
            var noteEntity = await _dbContext.Notes.FindAsync(noteId);

            // Validate the note exists and is owned by the user
            if (noteEntity?.OwnerId != _userId)
            {
                return false;
            }

            // Remove the note from the DbContext and assert that the one change was saved
            _dbContext.Notes.Remove(noteEntity);
            return await _dbContext.SaveChangesAsync() == 1;
        }
    }
}