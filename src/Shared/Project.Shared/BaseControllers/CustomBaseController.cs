using Microsoft.AspNetCore.Mvc;
using Project.Shared.Models.Responses;

namespace Project.Shared.BaseControllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
    }
}
