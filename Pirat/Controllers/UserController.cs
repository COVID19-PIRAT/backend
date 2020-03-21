using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pirat.DatabaseContext;

namespace Pirat.Controllers
{

    //[Route("api/[controller]/[action]")]
    [ApiController]
    [Route("/user")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;

        private readonly ApplicationContext _context;

        public UserController(ILogger<UserController> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Get()
        {
            var user = new User() {Age = 20, Name = "Dummy", Id = new Guid() };
            //_context.Add(user);
            //_context.SaveChanges();
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Post([FromBody] User u)
        {
            //_context.Add(user);
            //_context.SaveChanges();
            return Ok(u);
        }
    }

}
