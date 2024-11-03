using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using System.Security.Claims;
using HelpDeskSystem.Services;
using Microsoft.AspNetCore.Authorization;
using HelpDeskSystem.ClaimsManagement;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class SystemSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemSettings
        [Permission("SYSTEMSETTINGS:VIEW")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemSettings.Include(s => s.CreatedBy).Include(s => s.ModifiedBy);
            return View(await applicationDbContext.ToListAsync());
        }


        [Permission("SYSTEMSETTINGS:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }


        [Permission("SYSTEMSETTINGS:CREATE")]
        public IActionResult Create()
        {
         
            return View();
        }


        [Permission("SYSTEMSETTINGS:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemSetting systemSetting)
        {
            var userId = User.GetUserId();
            systemSetting.CreatedOn = DateTime.Now;
            systemSetting.CreatedById = userId;

            _context.Add(systemSetting);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));

            return View(systemSetting);
        }


        [Permission("SYSTEMSETTINGS:MODIFY")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting == null)
            {
                return NotFound();
            }
             return View(systemSetting);
        }


        [Permission("SYSTEMSETTINGS:MODIFY")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemSetting systemSetting)
        {
            if (id != systemSetting.Id)
            {
                return NotFound();
            }

                try
                {
                    var userId = User.GetUserId();
                    systemSetting.ModifiedById = userId;
                    systemSetting.ModifiedOn = DateTime.Now;
                    _context.Update(systemSetting);
                    await _context.SaveChangesAsync(userId);
                  return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemSettingExists(systemSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
            }

               return View(systemSetting);
        }


        [Permission("SYSTEMSETTINGS:DELETE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings
                .Include(s => s.CreatedBy)
                .Include(s => s.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // POST: SystemSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();
            var systemSetting = await _context.SystemSettings.FindAsync(id);
            if (systemSetting != null)
            {
                _context.SystemSettings.Remove(systemSetting);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool SystemSettingExists(int id)
        {
            return _context.SystemSettings.Any(e => e.Id == id);
        }
    }
}
