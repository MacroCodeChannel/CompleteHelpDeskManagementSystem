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
using HelpDeskSystem.ViewModels;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class TicketCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketCategories
        [Permission("CATEGORIES:VIEW")]
        public async Task<IActionResult> Index(TicketCategoryViewModel vm)
        {

            var ticketCategories = _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    ticketCategories = ticketCategories.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    ticketCategories = ticketCategories.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    ticketCategories = ticketCategories.Where(x => x.Name == vm.Name);
                }
            }

            vm.TicketCategories = await ticketCategories.ToListAsync();


            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            return View(vm);
        }


        [Permission("CATEGORIES:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }


        [Permission("CATEGORIES:CREATE")]
        public IActionResult Create()
        {
           
            return View();
        }


        [Permission("CATEGORIES:VIEW")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( TicketCategory ticketCategory)
        {
            var userId = User.GetUserId();

            ticketCategory.CreatedOn = DateTime.Now;
            ticketCategory.CreatedById = userId;
            _context.Add(ticketCategory);
            await _context.SaveChangesAsync(userId);

            TempData["MESSAGE"] = "Ticket Category Details successfully Created";
            return RedirectToAction(nameof(Index));
        }


        [Permission("CATEGORIES:UPDATE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories.FindAsync(id);
            if (ticketCategory == null)
            {
                return NotFound();
            }
              return View(ticketCategory);
        }


        [Permission("CATEGORIES:UPDATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketCategory ticketCategory)
        {
            if (id != ticketCategory.Id)
            {
                return NotFound();
            }

                try
                {
                var userId = User.GetUserId();
                ticketCategory.ModifiedOn = DateTime.Now;
                    ticketCategory.ModifiedById = userId;
                     _context.Update(ticketCategory);
                    await _context.SaveChangesAsync(userId);

                TempData["MESSAGE"] = "Ticket Category Details successfully Updated";

            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!TicketCategoryExists(ticketCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
               return View(ticketCategory);
        }

        // GET: TicketCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketCategory = await _context.TicketCategories
                .Include(t => t.CreatedBy)
                .Include(t => t.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketCategory == null)
            {
                return NotFound();
            }

            return View(ticketCategory);
        }

        // POST: TicketCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var ticketCategory = await _context.TicketCategories.FindAsync(id);
            if (ticketCategory != null)
            {
                _context.TicketCategories.Remove(ticketCategory);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketCategoryExists(int id)
        {
            return _context.TicketCategories.Any(e => e.Id == id);
        }
    }
}
