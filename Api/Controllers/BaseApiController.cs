using Microsoft.AspNetCore.Mvc;
using SharedWarehousingCore.Helpers;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected Task<int> GetUserId()
    {
        var value = HttpContext.User.Claims.FirstOrDefault(u =>
            u.Type.Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))?.Value;
        if (value != null)
            return Task.FromResult(int.Parse(value));

        throw new Exception("Nierozpoznany użytkownik");
    }
    
    protected List<string> GetUserRoles() => HttpContext.User.Claims.Where(u => u.Type.Contains("role"))
        ?.Select(x => x.Value).ToList() ?? throw new BasicException("Brak Dostępu");
}