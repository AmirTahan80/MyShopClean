using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Security.AuthenTication
{
    public class PolicyHandler : AuthorizationHandler<PolicyRequirenment>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        public PolicyHandler(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirenment requirement)
        {
            if (!_signInManager.IsSignedIn(context.User))
            {
                return Task.CompletedTask;
            }
            else
            {
                if (requirement.RoleName == "Founder" && context.User.IsInRole("Founder"))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if ((requirement.RoleName == "Manager") && (context.User.IsInRole("Manager") || context.User.IsInRole("Founder")))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if ((requirement.RoleName == "Writer") && (context.User.IsInRole("Writer") || context.User.IsInRole("Manager") || context.User.IsInRole("Founder")))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }

                if ((requirement.RoleName == "Customer") && (context.User.IsInRole("Customer") || context.User.IsInRole("Writer") || context.User.IsInRole("Manager") || context.User.IsInRole("Founder")))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            return Task.CompletedTask;
        }
    }
}
