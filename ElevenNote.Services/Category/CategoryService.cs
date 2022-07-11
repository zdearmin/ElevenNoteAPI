using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ElevenNote.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly int _userId;
        public CategoryService(IHttpContextAccessor httpContextAccessor)
        {
            var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var value = userClaims.FindFirst("Id")?.Value;
            var validId = int.TryParse(value, out _userId);
            if (!validId)
                throw new Exception("Attempted to build Category Service without User Id claim.");
        }
    }
}