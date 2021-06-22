using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Security.AuthenTication
{
    public class PolicyRequirenment:IAuthorizationRequirement
    {
        public PolicyRequirenment(string roleName)
        {
            RoleName = roleName;
        }
        public string RoleName { get; set; }
    }
}
