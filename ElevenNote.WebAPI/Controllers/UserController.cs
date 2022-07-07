using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ElevenNote.Services.User;

namespace ElevenNote.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // _service is a field so that all of the methods/endpoints can utilize the injected service
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
    }
}
