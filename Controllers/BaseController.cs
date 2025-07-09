using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AwlrAziz.Data;
// using AwlrAziz.Helpers;

namespace AwlrAziz.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // var username = ClaimIdentity.Username;
            var _context = filterContext.HttpContext.RequestServices.GetRequiredService<AwlrAzizContext>();   
            
            base.OnActionExecuting(filterContext);         
        }
    }
}