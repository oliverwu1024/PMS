using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/protected")]
public class ProtectedController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "AdminPolicy")]
    public IActionResult GetAdminData()
    {
        return Ok("This is protected data for admin.");
    }

    [HttpGet("user")]
    [Authorize(Policy = "UserPolicy")]
    public IActionResult GetUserData()
    {
        return Ok("This is protected data for user.");
    }

}
