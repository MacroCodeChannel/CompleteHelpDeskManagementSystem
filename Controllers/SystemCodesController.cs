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
using AutoMapper;
using HelpDeskSystem.Services;
using Microsoft.AspNetCore.Authorization;
using HelpDeskSystem.ClaimsManagement;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class SystemCodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SystemCodesController(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: SystemCodes
        [Permission("SYSTEMCODES:VIEW")]
        public async Task<IActionResult> Index(SystemCodeViewModel vm)
        {
           var systemcodes =_context
                .SystemCodes
                .Include(x=>x.CreatedBy)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Code))
                {
                    systemcodes = systemcodes.Where(x => x.Code.Contains(vm.Code));
                }
                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    systemcodes = systemcodes.Where(x => x.CreatedById == vm.CreatedById);
                }
                if (!string.IsNullOrEmpty(vm.Description))
                {
                    systemcodes = systemcodes.Where(x => x.Description == vm.Description);
                }
            }


            vm.SystemCodes = await systemcodes.ToListAsync();

            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");

            return View(vm);
        }


        [Permission("SYSTEMCODES:VIEW")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCode == null)
            {
                return NotFound();
            }

            return View(systemCode);
        }


        [Permission("SYSTEMCODES:CREATE")]
        public IActionResult Create()
        {
            return View();
        }


        [Permission("SYSTEMCODES:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemCodeViewModel vm)
        {

                var userId = User.GetUserId();

                SystemCode systemCodedetails = new();
                var systemCode = _mapper.Map(vm, systemCodedetails);
               systemCode.CreatedOn = DateTime.Now;
               systemCode.CreatedById = userId;

                _context.Add(systemCode);
                await _context.SaveChangesAsync(userId);

            

            return RedirectToAction(nameof(Index));
           
            return View(systemCode);
        }

        [Permission("SYSTEMCODES:MODIFY")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes.FindAsync(id);
            if (systemCode == null)
            {
                return NotFound();
            }
            return View(systemCode);
        }

        [Permission("SYSTEMCODES:MODIFY")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SystemCode systemCode)
        {
            if (id != systemCode.Id)
            {
                return NotFound();
            }

                try
                {
                    var userId = User.GetUserId();
                    systemCode.ModifiedOn = DateTime.Now;
                    systemCode.ModifiedById = userId;

                    _context.Update(systemCode);
                    await _context.SaveChangesAsync(userId);

                 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCodeExists(systemCode.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


                return RedirectToAction(nameof(Index));
            
            return View(systemCode);
        }

        // GET: SystemCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCode = await _context.SystemCodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemCode == null)
            {
                return NotFound();
            }

            return View(systemCode);
        }

        // POST: SystemCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var systemCode = await _context.SystemCodes.FindAsync(id);
            if (systemCode != null)
            {
                _context.SystemCodes.Remove(systemCode);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCodeExists(int id)
        {
            return _context.SystemCodes.Any(e => e.Id == id);
        }
    }
}
