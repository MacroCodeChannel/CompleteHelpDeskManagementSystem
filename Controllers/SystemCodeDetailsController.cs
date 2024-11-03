using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using HelpDeskSystem.Data.Migrations;
using System.Security.Claims;
using HelpDeskSystem.Services;
using Microsoft.AspNetCore.Authorization;
using HelpDeskSystem.ClaimsManagement;
using HelpDeskSystem.ViewModels;

namespace HelpDeskSystem.Controllers
{
    [Authorize]
    public class SystemCodeDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemCodeDetails
        [Permission("SYSTEMCODEDETAILS:VIEW")]
        public async Task<IActionResult> Index(SystemCodeDetailViewModel vm)
        {
            var systemCodeDetails = _context
                .SystemCodeDetails
                .Include(s => s.SystemCode)
                .Include(s=>s.CreatedBy)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Description))
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.Description == vm.Description);
                }

                if (vm.SystemCodeId > 0)
                {
                    systemCodeDetails = systemCodeDetails.Where(x => x.SystemCodeId == vm.SystemCodeId);
                }
            }


            vm.SystemCodeDetails = await systemCodeDetails.ToListAsync();

            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            return View(vm);
        }


        [Permission("SYSTEMCODEDETAILS:VIEW")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails
                .Include(s => s.SystemCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            return View(systemCodeDetail);
        }


        [Permission("SYSTEMCODEDETAILS:CREATE")]

        public IActionResult Create()
        {
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description");
            return View();
        }


        [Permission("SYSTEMCODEDETAILS:CREATE")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemCodeDetail systemCodeDetail)
        {

            var userId = User.GetUserId();
            systemCodeDetail.CreatedOn = DateTime.Now;
            systemCodeDetail.CreatedById = userId;


               _context.Add(systemCodeDetail);
                await _context.SaveChangesAsync(userId);


            return RedirectToAction(nameof(Index));
           
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }


        [Permission("SYSTEMCODEDETAILS:MODIFY")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails.FindAsync(id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Description", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        [Permission("SYSTEMCODEDETAILS:MODIFY")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SystemCodeDetail systemCodeDetail)
        {
            if (id != systemCodeDetail.Id)
            {
                return NotFound();
            }

            
                try
                {
                    var userId = User.GetUserId();
                    systemCodeDetail.ModifiedOn = DateTime.Now;
                    systemCodeDetail.ModifiedById = userId;

                    _context.Update(systemCodeDetail);
                    await _context.SaveChangesAsync(userId);

                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCodeDetailExists(systemCodeDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Id", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        // GET: SystemCodeDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails
                .Include(s => s.SystemCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            return View(systemCodeDetail);
        }

        // POST: SystemCodeDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var systemCodeDetail = await _context.SystemCodeDetails.FindAsync(id);
            if (systemCodeDetail != null)
            {
                _context.SystemCodeDetails.Remove(systemCodeDetail);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCodeDetailExists(int id)
        {
            return _context.SystemCodeDetails.Any(e => e.Id == id);
        }
    }
}
