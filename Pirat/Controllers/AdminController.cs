using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pirat.Model.Api.Admin;
using Pirat.Other;
using Swashbuckle.AspNetCore.Annotations;


namespace Pirat.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController : Controller
    {
        private readonly string _adminKey;

        public AdminController()
        {
            _adminKey = Environment.GetEnvironmentVariable("PIRAT_ADMIN_KEY");
        }

        [HttpPost("verify-key")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(AdminKeyVerificationResponse))]
        public IActionResult VerifyKey([FromBody] AdminKeyVerificationRequest request)
        {
            NullCheck.ThrowIfNull<AdminKeyVerificationRequest>(request);
            return Ok(new AdminKeyVerificationResponse()
            {
                success = _adminKey.Equals(request.adminKey, StringComparison.InvariantCulture)
            });
        }
    }
}
