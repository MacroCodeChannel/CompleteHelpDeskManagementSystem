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
using HelpDeskSystem.ViewModels;
using HelpDeskSystem.Services;
using HelpDeskSystem.ClaimsManagement;
using Microsoft.AspNetCore.Authorization;
using HelpDeskSystem.Data.Migrations;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
   
    public class TicketSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketSubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketSubCategories
        [Permission("SUBCATEGORIES:VIEW")]
        public async Task<IActionResult> Index(int id,TicketSubCategoriesVM vm)
        {
            

           var ticketSubCategories = _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .Where(x=>x.CategoryId== id)    
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    ticketSubCategories = ticketSubCategories.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    ticketSubCategories = ticketSubCategories.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    ticketSubCategories = ticketSubCategories.Where(x => x.Name == vm.Name);
                }

                if (vm.CategoryId > 0)
                {
                    ticketSubCategories = ticketSubCategories.Where(x => x.CategoryId == vm.CategoryId);
                }
            }


            vm.CategoryId = id;
            vm.TicketSubCategories = await ticketSubCategories.ToListAsync();
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

            return View(vm);
        }

        [Permission("SUBCATEGORIES:VIEW")]
        public async Task<IActionResult> SubCategories(TicketSubCategoriesVM vm)
        {
            vm.TicketSubCategories = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .ToListAsync();

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");


            return View(vm);
        }


        [Permission("SUBCATEGORIES:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        [Permission("SUBCATEGORIES:CREATE")]
        public IActionResult Create(int Id)
        {
            TicketSubCategory  category = new ();
            category.CategoryId = Id;

            return View(category);
        }

        [Permission("SUBCATEGORIES:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, TicketSubCategory ticketSubCategory)
        {
            var userId = User.GetUserId();
            ticketSubCategory.CreatedById = userId;
            ticketSubCategory.CreatedOn = DateTime.Now;

            ticketSubCategory.Id = 0;
            ticketSubCategory.CategoryId = id;
             _context.Add(ticketSubCategory);
             await _context.SaveChangesAsync(userId);


            TempData["MESSAGE"] = "Ticket Sub-Category Details successfully Created";

            return RedirectToAction("Index", new { id=id});
            
            return View(ticketSubCategory);
        }

        [Permission("SUBCATEGORIES:UPDATE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories.FindAsync(id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }
           
            return View(ticketSubCategory);
        }

        [Permission("SUBCATEGORIES:UPDATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketSubCategory ticketSubCategory)
        {
            if (id != ticketSubCategory.Id)
            {
                return NotFound();
            }

                try
                {
                    var loggedIUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ticketSubCategory.ModifiedById = loggedIUser;
                    ticketSubCategory.ModifiedOn = DateTime.Now;

                    _context.Update(ticketSubCategory);
                    await _context.SaveChangesAsync(loggedIUser);
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketSubCategoryExists(ticketSubCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name", ticketSubCategory.CategoryId);
             return View(ticketSubCategory);
        }

        // GET: TicketSubCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketSubCategory = await _context.TicketSubCategories
                .Include(t => t.Category)
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketSubCategory == null)
            {
                return NotFound();
            }

            return View(ticketSubCategory);
        }

        // POST: TicketSubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var ticketSubCategory = await _context.TicketSubCategories.FindAsync(id);
            if (ticketSubCategory != null)
            {
                _context.TicketSubCategories.Remove(ticketSubCategory);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketSubCategoryExists(int id)
        {
            return _context.TicketSubCategories.Any(e => e.Id == id);
        }
    }
}
