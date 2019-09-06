using System.Threading.Tasks;
using Fair.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fair.Security
{
    public class CanWriteSearchRequirement : IAuthorizationRequirement
    {
    }

    public class CanWriteSearchHandler : AuthorizationHandler<CanWriteSearchRequirement, Search>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanWriteSearchRequirement requirement, Search search)
        {
            // In ASP.NET Core, if the resource is null, the authorization handler is not called. So to handle
            // the case when a new search is to be created, we pass an empty search whose Position is null.
            if (context.User.HasClaim(FairClaims.IsAdmin.ToString(), true.ToString()) ||
                search.Position == null && context.User.HasClaim(claim => claim.Type == FairClaims.DepartmentChair.ToString()) ||
                search.Position != null && context.User.HasClaim(FairClaims.DepartmentChair.ToString(), search.DepartmentId.ToString()))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
