using System.Threading.Tasks;
using Fair.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fair.Security
{
    public class CanReadSearchRequirement : IAuthorizationRequirement
    {
    }

    public class CanReadSearchHandler : AuthorizationHandler<CanReadSearchRequirement, Search>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadSearchRequirement requirement, Search search)
        {
            if (context.User.HasClaim(FairClaims.IsAdmin.ToString(), true.ToString()) ||
                context.User.HasClaim(FairClaims.SearchDepartmentChair.ToString(), search.SearchId.ToString()) ||
                context.User.HasClaim(FairClaims.SearchCommitteeChair.ToString(), search.SearchId.ToString()) ||
                context.User.HasClaim(FairClaims.SearchCommitteeMember.ToString(), search.SearchId.ToString()))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
