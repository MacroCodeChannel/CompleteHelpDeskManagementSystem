using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Security.Claims;

namespace HelpDeskSystem.ClaimsManagement
{
    public abstract class AttributeAuthorizationHandler<TRequirement, TAttribute> : AuthorizationHandler<TRequirement>
        where TRequirement : IAuthorizationRequirement
        where TAttribute : Attribute
    {
        private readonly  IHttpContextAccessor _contextAccessor;    
        protected AttributeAuthorizationHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }



        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {

            List<PermissionAttribute> attributes = new ();

            var actions = _contextAccessor.HttpContext.GetEndpoint().Metadata;

            var allpermission = (PermissionAttribute)actions.FirstOrDefault(x=>x.GetType() == typeof(PermissionAttribute)); 

            attributes.Add(allpermission);


            return HandleRequirementAsync(context, requirement, attributes);
        }

        protected abstract Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TRequirement requirement,
            IEnumerable<PermissionAttribute> attributes);

      
    }

    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        // Add any custom requirement properties or methods if needed
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        public string Name { get; }

        public PermissionAttribute(string name) : base("Permission")
        {
            Name = name;
        }
    }

    public class PermissionAuthorizationHandler : AttributeAuthorizationHandler<PermissionAuthorizationRequirement, PermissionAttribute>
    {


        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
            :base(httpContextAccessor)
        {
            
        }



        protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement,
        IEnumerable<PermissionAttribute> attributes)
        {
            if (attributes == null || !attributes.Any())
            {
                context.Fail();
                return;
            }

            foreach (var permissionAttribute in attributes)
            {
                var hasPermission = await AuthorizeAsync(context.User, permissionAttribute.Name);
                if (!hasPermission)
                {
                    context.Fail();
                    return;
                }
            }

            context.Succeed(requirement);
        }


        private Task<bool> AuthorizeAsync(ClaimsPrincipal user, string permission)
        {
            var userPermissions = user.FindFirstValue("UserPermission")?.ToLower();
            // Check for permission in user's claims
            var haspermission = Task.FromResult(userPermissions != null && userPermissions.Contains(permission.ToLower()));
           
            return haspermission;
        }
    }

}