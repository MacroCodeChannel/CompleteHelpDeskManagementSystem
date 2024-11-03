using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Data;
using HelpDeskSystem.Models;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using HelpDeskSystem.Services;
using HelpDeskSystem.ClaimsManagement;
using Microsoft.AspNetCore.Authorization;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Permission("CITIES:VIEW")]
        public async Task<IActionResult> Index()
        {
            var cities = await  _context.CitiesView.ToListAsync();
            return View(cities);
        }


        [Permission("CITIES:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }


        [Permission("CITIES:CREATE")]
        public IActionResult Create()
        {
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
          
            return View();
        }


        [Permission("CITIES:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(City city)
        {
            var userId = User.GetUserId();
            city.CreatedOn = DateTime.Now;
            city.CreatedById = userId;
            _context.Add(city);
            await _context.SaveChangesAsync(userId);

            TempData["MESSAGE"] = "City Details successfully added";

            return RedirectToAction(nameof(Index));


            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
         
            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                    .FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", city.CountryId);
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,City city)
        {
            if (id != city.Id)
            {
                return NotFound();
            }

                try
                {
                    var userId = User.GetUserId();
                    city.ModifiedById = userId;
                    city.ModifiedOn = DateTime.Now; 
                    _context.Update(city);
                    await _context.SaveChangesAsync(userId);
                   TempData["MESSAGE"] = "City Details successfully Updated";

            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!CityExists(city.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
             
            return RedirectToAction(nameof(Index));
            
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Id", city.CountryId);
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", city.CreatedById);
            ViewData["ModifiedById"] = new SelectList(_context.Users, "Id", "Id", city.ModifiedById);
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
            }

            await _context.SaveChangesAsync(userId);
            TempData["MESSAGE"] = "City Details successfully deleted";

            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.Id == id);
        }
    }
}
