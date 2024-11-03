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
    public class SystemTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemTasks
        [Permission("SYSTEMTASKS:VIEW")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemTasks
                 .Include("ChildTasks.ChildTasks.ChildTasks.ChildTasks")
                 .Include(s=>s.CreatedBy)
                 .Where(i => i.ParentId == null);
            return View(await applicationDbContext.ToListAsync());
        }


        [Permission("SYSTEMTASKS:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemTask == null)
            {
                return NotFound();
            }

            return View(systemTask);
        }


        [Permission("SYSTEMTASKS:CREATE")]
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name");
            return View();
        }


        [Permission("SYSTEMTASKS:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemTask systemTask)
        {

            var userId = User.GetUserId();
            systemTask.CreatedOn = DateTime.Now;
            systemTask.CreatedById = userId;

            _context.Add(systemTask);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));

            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }


        [Permission("SYSTEMTASKS:MODIFY")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks.FindAsync(id);
            if (systemTask == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }


        [Permission("SYSTEMTASKS:MODIFY")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemTask systemTask)
        {
            if (id != systemTask.Id)
            {
                return NotFound();
            }

           
                try
                {
                var userId = User.GetUserId();
                systemTask.ModifiedOn = DateTime.Now;
                    systemTask.ModifiedById = userId;

                    _context.Update(systemTask);
                    await _context.SaveChangesAsync(userId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemTaskExists(systemTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }


        [Permission("SYSTEMTASKS:MODIFY")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks
                .Include(s => s.Parent)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemTask == null)
            {
                return NotFound();
            }

            return View(systemTask);
        }

        // POST: SystemTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var userId = User.GetUserId();
            var systemTask = await _context.SystemTasks.FindAsync(id);
            if (systemTask != null)
            {
                _context.SystemTasks.Remove(systemTask);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool SystemTaskExists(int id)
        {
            return _context.SystemTasks.Any(e => e.Id == id);
        }
    }
}
