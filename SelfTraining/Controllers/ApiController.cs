using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SelfTraining.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        public ApiController()
        {
        }

        [HttpGet("hello")]
        [Authorize]
        public async Task<IActionResult> hello(string? code)
        {
            return Ok("hello");
        }
    }
}

