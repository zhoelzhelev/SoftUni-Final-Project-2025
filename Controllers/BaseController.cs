using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Schedulefy.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    protected bool IsUerAuthenticated()
    {
        return this.User.Identity?.IsAuthenticated ?? false;
    }

    protected string? GetUserId()
    {
        string? userId = null;

        bool isAuthenticated = this.IsUerAuthenticated();

        if (isAuthenticated)
        {
            userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        return userId;
    }
}
