using ElmahCore;
using HelpDeskSystem.ClaimsManagement;
using HelpDeskSystem.Data;
using HelpDeskSystem.Extensions;
using HelpDeskSystem.Models;
using HelpDeskSystem.Services;
using HelpDeskSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> rolemanager, SignInManager<ApplicationUser> signInManager)
        {
            _rolemanager = rolemanager;

            _signInManager = signInManager;

            _userManager = userManager;

            _context = context;

        }

        // GET: UsersController
        [Permission("USERS:VIEW")]
        public async Task<ActionResult> Index(ApplicationUserViewModel vm)
        {
            vm.ApplicationUsers = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Gender)
                .ToListAsync();

            ViewData["RoleId"] = new SelectList(_context.Roles.ToList(), "Id", "Name");

            return View(vm);
        }


        [Permission("USERS:VIEW")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Gender"), "Id", "Description");
            ViewData["RoleId"] = new SelectList(_context.Roles.ToList(), "Id", "Name");
            return View();
        }


        [Permission("USERS:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ApplicationUser user)
        {
            try
            {
                var roledetails = await _context.Roles.Where(x => x.Id == user.RoleId).FirstOrDefaultAsync();
                var userId = User.GetUserId();
                ApplicationUser registereduser = new();
                registereduser.FirstName =user.FirstName;
                registereduser.UserName = user.UserName;
                registereduser.MiddleName = user.MiddleName;
                registereduser.LastName = user.LastName;
                registereduser.NormalizedUserName = user.UserName;
                registereduser.Email = user.Email;
                registereduser.EmailConfirmed = true;
                registereduser.GenderId = user.GenderId;
                registereduser.RoleId = user.RoleId;
                registereduser.City = user.City;
                registereduser.Country = user.Country;
                registereduser.PhoneNumber = user.PhoneNumber;

                var result = await _userManager.CreateAsync(registereduser, user.PasswordHash);
                if(result.Succeeded) 
                {
                    await _userManager.AddToRoleAsync(registereduser, roledetails.Name);
                    TempData["MESSAGE"] = "User Details successfully Created";

                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
            {
                return View();
            }
            ViewData["GenderId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Gender"), "Id", "Description",user.GenderId);
            ViewData["RoleId"] = new SelectList(_context.Roles.ToList(), "Id", "Name",user.RoleId);
        }


        [Permission("USERS:CHANGEPASSWORD")]
        public async Task<ActionResult> ChangePassword(string id,ResetPasswordViewModel vm)
        {
            var user  = await _context.Users.Where(x=>x.Id==id).FirstOrDefaultAsync();
            vm.Id = user.Id;
            vm.Email = user.Email;
            vm.FullName = user.FullName;
            vm.FirstName = user.FirstName;
            vm.MiddleName = user.MiddleName;
            vm.LastName = user.LastName;
            vm.RoleId = user.RoleId;
            vm.GenderId = user.GenderId;
            if(vm.GenderId > 0)
            {
                vm.Gender = await _context.SystemCodeDetails.Where(x => x.Id == vm.GenderId).FirstOrDefaultAsync();
            }
            if (!string.IsNullOrEmpty(vm.RoleId))
            {
                vm.Role = await _context.Roles.Where(x => x.Id == vm.RoleId).FirstOrDefaultAsync();
            }
            return View(vm);
        }



        [Permission("USERS:CHANGEPASSWORD")]
        public async Task<ActionResult> ConfirmChangePassword(ResetPasswordViewModel vm)
        {
            try
            {
                var loggedInuser = User.GetUserId();
                var user = await _context.Users.Where(x => x.Id == vm.Id).FirstOrDefaultAsync();
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync(loggedInuser);

                    TempData["MESSAGE"] = "Password reset Succesfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Password details could not be reset successfully";
                    return View(vm);    
                }
            }
            catch(Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                TempData["Error"] = "Password details could not be reset successfully" + ex.Message;
                return View(vm);
            }
                
        }


        [Permission("USERS:ACTIVATEUSER")]
        public async Task<ActionResult> ActivateUser(string id, ResetPasswordViewModel vm)
        {
            var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            vm.Id = user.Id;
            vm.Email = user.Email;
            vm.FullName = user.FullName;
            vm.FirstName = user.FirstName;
            vm.MiddleName = user.MiddleName;
            vm.LastName = user.LastName;
            vm.RoleId = user.RoleId;
            vm.GenderId = user.GenderId;
            if (vm.GenderId > 0)
            {
                vm.Gender = await _context.SystemCodeDetails.Where(x => x.Id == vm.GenderId).FirstOrDefaultAsync();
            }
            if (!string.IsNullOrEmpty(vm.RoleId))
            {
                vm.Role = await _context.Roles.Where(x => x.Id == vm.RoleId).FirstOrDefaultAsync();
            }
            return View(vm);
        }



        [Permission("USERS:ACTIVATEUSER")]
        public async Task<ActionResult> ConfirmActivateUser(ResetPasswordViewModel vm)
        {
            try
            {
                var loggedInuser = User.GetUserId();
                var user = await _context.Users.Where(x => x.Id == vm.Id).FirstOrDefaultAsync();

                if (user !=null)
                {
                    user.LockoutEnabled = true;
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                    user.IsLocked = false;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync(loggedInuser);

                    TempData["MESSAGE"] = "User Account Activated Succesfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "User Account details could not be activated successfully";
                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                TempData["Error"] = "User Account details could not be activated successfully" + ex.Message;
                return View(vm);
            }

        }



        [Permission("USERS:UPDATE")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
