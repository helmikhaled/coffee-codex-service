using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeCodex.API.Authentication;

[ApiController]
[Route("admin/auth-check")]
[Authorize(Policy = AuthorizationPolicyNames.AdminOnly)]
public sealed class AuthCheckController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Get()
    {
        return NoContent();
    }
}
