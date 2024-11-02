using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace PhotoGallery.Controllers;
public abstract class BaseController : ControllerBase
{
    protected int? UserId
    {
        get
        {
            var userIdClaim = HttpContext?.User?.FindFirst(ClaimConstants.ObjectId);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId)
                ? userId
                : null;
        }
    }

}
