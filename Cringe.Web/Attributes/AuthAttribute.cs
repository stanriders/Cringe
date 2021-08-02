using System.Threading.Tasks;
using Cringe.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cringe.Web.Attributes
{
    public class AuthAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbContext = context.HttpContext.RequestServices.GetService<PlayerDatabaseContext>();

            if (dbContext is null ||
                !context.ActionArguments.ContainsKey("u") ||
                !context.ActionArguments.ContainsKey("h") ||
                !await dbContext.Players.AnyAsync(x =>
                    x.Username ==
                    (string) context.ActionArguments["u"])) // && x.Password == (string) context.ActionArguments["h"]
                context.Result = new UnauthorizedResult();
                
                return;
            }
            
            await next.Invoke();
        }
    }
}
