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

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Countries
        [Permission("COUNTRIES:VIEW")]
        public async Task<IActionResult> Index()
        {
            var countries = _context.Countries
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy);
            return View(await countries.ToListAsync());
        }


        [Permission("COUNTRIES:VIEW")]
        public async Task<IActionResult> CountryCities(int id)
        {
            var cities = await _context.Cities
                .Include(c => c.Country)
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .Where(x=>x.CountryId==id)
                .ToListAsync();
            return View(cities);
        }

        [Permission("COUNTRIES:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [Permission("COUNTRIES:CREATE")]
        public IActionResult Create()
        {
          
            return View();
        }

        [Permission("COUNTRIES:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            var userId = User.GetUserId();
            country.CreatedOn = DateTime.Now;
            country.CreatedById = userId;
            _context.Add(country);

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
           
            return View(country);
        }

        [Permission("COUNTRIES:MODIFY")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
           return View(country);
        }

        [Permission("COUNTRIES:MODIFY")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }

            try
                {

                    var userId = User.GetUserId();
                    country.ModifiedOn= DateTime.Now;
                    country.ModifiedById = userId;

                    _context.Add(country);

                    _context.Update(country);
                    await _context.SaveChangesAsync(userId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
          
                 return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Countries
                .Include(c => c.CreatedBy)
                .Include(c => c.ModifiedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var country = await _context.Countries.FindAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
