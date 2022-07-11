using System.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevenNote.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElevenNote.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        // Establishing field that will hold the service
        private readonly ICategoryService _categoryService;

        // Setting up the constructor, takes in ICategoryService parameter that is assigned to our field
        // Allows controller to access the CategoryService field, allowing access to the service methods
        public CategoryController (ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
    }
}
