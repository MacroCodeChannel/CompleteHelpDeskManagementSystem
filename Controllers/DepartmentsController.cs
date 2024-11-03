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
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        [Permission("DEPARTMENTS:VIEW")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context
                .Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy);
            return View(await applicationDbContext.ToListAsync());
        }


        [Permission("DEPARTMENTS:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }


        [Permission("DEPARTMENTS:CREATE")]
        public IActionResult Create()
        {
            return View();
        }


        [Permission("DEPARTMENTS:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {

            var userId = User.GetUserId();
            department.CreatedOn = DateTime.Now;
            department.CreatedById = userId;

            _context.Add(department);
            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
            
            return View(department);
        }


        [Permission("DEPARTMENTS:MODIFY")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
             return View(department);
        }


        [Permission("DEPARTMENTS:MODIFY")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            
                try
                {
                    var userId = User.GetUserId();
                    department.ModifiedOn = DateTime.Now;
                    department.ModifiedById = userId;
                    _context.Update(department);
                    await _context.SaveChangesAsync(userId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", department.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", department.ModifiedById);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedBy)
                .Include(d => d.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
