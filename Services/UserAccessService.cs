using System.Security.Claims;

namespace HelpDeskSystem.Services
{
    public static class UserAccessService
    {


        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentloggedinUser = user;
                if (currentloggedinUser != null)
                {
                    return currentloggedinUser.FindFirst(ClaimTypes.NameIdentifier).Value;
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetUserName(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentloggedinUser = user;
                if (currentloggedinUser != null)
                {
                    return currentloggedinUser.FindFirstValue(ClaimTypes.Name);
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentloggedinUser = user;
                if (currentloggedinUser != null)
                {
                    return currentloggedinUser.FindFirstValue(ClaimTypes.Email);
                }
                else
                {
                    return null;
                }
            }
        }
        public static string GetUserRoleId(this ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }
            else
            {
                ClaimsPrincipal currentloggedinUser = user;
                if (currentloggedinUser != null)
                {
                    return currentloggedinUser.FindFirst("RoleId").Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
