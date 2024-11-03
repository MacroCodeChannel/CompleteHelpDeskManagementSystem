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
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using HelpDeskSystem.Services;
using ElmahCore;
using HelpDeskSystem.ClaimsManagement;
using Microsoft.AspNetCore.Authorization;

namespace HelpDeskSystem.Controllers
{

    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
         private readonly  IConfiguration _configuration;

        private readonly IMapper _mapper;

        public TicketsController(ApplicationDbContext context, IConfiguration configuration,
            IMapper imapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = imapper;
        }

        // GET:Tickets
        [Permission("TICKETS:VIEW")]
        public async Task<IActionResult> Index(TicketViewModel vm)
        {
           
            var  alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t=> t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t=>t.TicketComments)
                .OrderBy(x=>x.CreatedOn)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.ToListAsync();

            vm.MainDuration = await _context.SystemSettings
                .Where(x => x.Code == "TICKETRESOLUTIONDAYS").FirstOrDefaultAsync();




            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x=>x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }



        [Permission("TICKETS:VIEW")]
        public async Task<IActionResult> AssignedTickets(TicketViewModel vm)
        {
            var assignedStatus = await _context
            .SystemCodeDetails
            .Include(x => x.SystemCode)
            .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Assigned")
            .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(x=>x.StatusId== assignedStatus.Id)
                .OrderBy(x => x.CreatedOn)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.ToListAsync();


            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
            return View(vm);
        }



        [Permission("TICKETS:VIEW")]
        public async Task<IActionResult> ClosedTickets(TicketViewModel vm)
        {
            var closedStatus = await _context
            .SystemCodeDetails
            .Include(x => x.SystemCode)
            .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
            .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Priority)
               .Include(t => t.Status)
               .Include(t => t.TicketComments)
               .Where(x => x.StatusId == closedStatus.Id)
               .OrderBy(x => x.CreatedOn)
               .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.ToListAsync();


            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
            return View(vm);
        }



        [Permission("TICKETS:VIEW")]
        public async Task<IActionResult> ResolvedTickets(TicketViewModel vm)
        {
            var resolvedStatus = await _context
            .SystemCodeDetails
            .Include(x => x.SystemCode)
            .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Resolved")
            .FirstOrDefaultAsync();

            var alltickets = _context.Tickets
                .Include(t => t.CreatedBy)
                .Include(t => t.SubCategory)
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.TicketComments)
                .Where(x => x.StatusId == resolvedStatus.Id)
                .OrderBy(x => x.CreatedOn)
                .AsQueryable();

            if (vm != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    alltickets = alltickets.Where(x => x.Title == vm.Title);
                }

                if (!string.IsNullOrEmpty(vm.CreatedById))
                {
                    alltickets = alltickets.Where(x => x.CreatedById == vm.CreatedById);
                }

                if (vm.StatusId > 0)
                {
                    alltickets = alltickets.Where(x => x.StatusId == vm.StatusId);
                }

                if (vm.PriorityId > 0)
                {
                    alltickets = alltickets.Where(x => x.PriorityId == vm.PriorityId);
                }

                if (vm.CategoryId > 0)
                {
                    alltickets = alltickets.Where(x => x.SubCategory.CategoryId == vm.CategoryId);
                }
            }

            vm.Tickets = await alltickets.ToListAsync();


            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");
            return View(vm);
        }


        [Permission("TICKETS:VIEW")]
        public async Task<IActionResult> Details(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

             vm.TicketDetails = await _context.Tickets
                .Include(t => t.CreatedBy)
                .Include (t=> t.SubCategory)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t=>t.TicketId==id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
                   .Include(t => t.CreatedBy)
                   .Include(t => t.Ticket)
                   .Include(t => t.Status)
                   .Where(t => t.TicketId == id)
                   .ToListAsync();

            if (vm.TicketDetails == null)
            {
                return NotFound();
            }

            return View(vm);
        }


        [Permission("TICKETS:REOPEN")]
        public async Task<IActionResult> ReOpen(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }



        [Permission("TICKETS:ASSIGN")]
        public async Task<IActionResult> TicketAssignment(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
               .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");

            return View(vm);
        }


        [Permission("TICKETS:RESOLVE")]
        public async Task<IActionResult> Resolve(int? id, TicketViewModel vm)
        {
            if (id == null)
            {
                return NotFound();
            }

            vm.TicketDetails = await _context.Tickets
               .Include(t => t.CreatedBy)
               .Include(t => t.SubCategory)
               .Include(t => t.Status)
               .Include(t => t.Priority)
                .Include(t => t.AssignedTo)
               .FirstOrDefaultAsync(m => m.Id == id);

            vm.TicketComments = await _context.Comments
                .Include(t => t.CreatedBy)
                .Include(t => t.Ticket)
                .Where(t => t.TicketId == id)
                .ToListAsync();

            vm.TicketResolutions = await _context.TicketResolutions
               .Include(t => t.CreatedBy)
               .Include(t => t.Ticket)
               .Include(t => t.Status)
               .Where(t => t.TicketId == id)
               .ToListAsync();


            if (vm.TicketDetails == null)
            {
                return NotFound();
            }


            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "ResolutionStatus"), "Id", "Description");

            return View(vm);
        }

        [Permission("TICKETS:CREATE")]
        public IActionResult Create()
        {

            ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x=>x.SystemCode).Where(x=>x.SystemCode.Code== "Priority"), "Id", "Description");
            ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }


        [Permission("TICKETS:CREATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketViewModel ticketvm, IFormFile attachment)
        {
            try
            {

                if (attachment != null && attachment.Length > 0)
                {
                    var filename = "Ticket_Attachment" + DateTime.Now.ToString("yyyymmddhhmmss") + "_" + attachment.FileName;
                    var path = _configuration["FileSettings:UploadsFolder"]!;
                    var filepath = Path.Combine(path, filename);
                    var stream = new FileStream(filepath, FileMode.Create);
                    await attachment.CopyToAsync(stream);
                    ticketvm.Attachment = filename;
                }


                var pendingstatus = await _context
                    .SystemCodeDetails
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == "Status" && x.Code == "Pending")
                    .FirstOrDefaultAsync();

                Ticket ticketdetails = new();
                var ticket = _mapper.Map(ticketvm, ticketdetails);


                ticket.StatusId = pendingstatus.Id;
                var userId = User.GetUserId();
                ticket.CreatedOn = DateTime.Now;
                ticket.CreatedById = userId;
                _context.Add(ticket);
                await _context.SaveChangesAsync(userId);


                TempData["MESSAGE"] = "Ticket Details successfully Created";


                ViewData["PriorityId"] = new SelectList(_context.SystemCodeDetails.Include(x => x.SystemCode).Where(x => x.SystemCode.Code == "Priority"), "Id", "Description");

                ViewData["CategoryId"] = new SelectList(_context.TicketCategories, "Id", "Name");

                ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                TempData["Error"] = "Ticket Details could not be Created" + ex.Message;
                return View(ticketvm);
            }
            
        }


        [Permission("TICKETS:COMMENTS")]
        [HttpPost]
        public async Task<IActionResult> AddComment(int id,TicketViewModel vm)
        {
            try
            {

                //Logged In User
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Comment newcomment = new();
                newcomment.TicketId = id;
                newcomment.CreatedOn = DateTime.Now;
                newcomment.CreatedById = userId;
                newcomment.Description = vm.CommentDescription;
                _context.Add(newcomment);
                await _context.SaveChangesAsync(userId);


                TempData["MESSAGE"] = "Comments Details successfully Created";

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                TempData["Error"] = "Comment Details could not be Created" + ex.Message;
                return View(vm);
            }
        }



        [Permission("TICKETS:ASSIGN")]
        [HttpPost]
        public async Task<IActionResult> AssignedConfirmed(int id, TicketViewModel vm)
        {

            try
            {
                var reassignedstatus = await _context.SystemCodeDetails
                    .Include(x => x.SystemCode)
                    .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Assigned")
                    .FirstOrDefaultAsync();

                //Logged In User
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                TicketResolution resolution = new();
                resolution.TicketId = id;
                resolution.StatusId = reassignedstatus.Id;
                resolution.CreatedOn = DateTime.Now;
                resolution.CreatedById = userId;
                resolution.Description = "Ticket Assigned";
                _context.Add(resolution);

                var ticket = await _context.Tickets
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                ticket.StatusId = reassignedstatus.Id;
                ticket.AssignedToId = vm.AssignedToId;
                ticket.AssignedOn = DateTime.Now;
                _context.Update(ticket);

                await _context.SaveChangesAsync(userId);



                TempData["MESSAGE"] = "Ticket Re-Opened successfully";

                return RedirectToAction("Resolve", new { id = id });
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(ex);
                TempData["Error"] = "Ticket  could not be re-opened successfully" + ex.Message;

                return View(vm);
            }
        }



        [Permission("TICKETS:REOPEN")]
        [HttpPost]
        public async Task<IActionResult> ReOpenConfirmed(int id, TicketViewModel vm)
        {

            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "ReOpened")
                .FirstOrDefaultAsync();

            //Logged In User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = closedstatus.Id;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = "Ticket Re-Opened";
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            ticket.StatusId = closedstatus.Id;
            _context.Update(ticket);

            await _context.SaveChangesAsync(userId);

           

            TempData["MESSAGE"] = "Ticket Re-Opened successfully";

            return RedirectToAction("Resolve", new { id = id });
        }


        [Permission("TICKETS:CLOSE")]
        [HttpPost]
        public async Task<IActionResult> ClosedConfirmed(int id, TicketViewModel vm)
        {

            var closedstatus = await _context.SystemCodeDetails
                .Include(x => x.SystemCode)
                .Where(x => x.SystemCode.Code == "ResolutionStatus" && x.Code == "Closed")
                .FirstOrDefaultAsync();
             
            //Logged In User
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = closedstatus.Id;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = "Ticket Closed";
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            ticket.StatusId = closedstatus.Id;
            _context.Update(ticket);

            await _context.SaveChangesAsync(userId);

            

            TempData["MESSAGE"] = "Ticket Closed successfully";

            return RedirectToAction("Resolve", new { id = id });
        }



        [Permission("TICKETS:RESOLVE")]
        [HttpPost]
        public async Task<IActionResult> ResolvedConfirmed(int id, TicketViewModel vm)
        {
            //Logged In User
            var userId = User.GetUserId();
            TicketResolution resolution = new();
            resolution.TicketId = id;
            resolution.StatusId = vm.StatusId;
            resolution.CreatedOn = DateTime.Now;
            resolution.CreatedById = userId;
            resolution.Description = vm.CommentDescription;
            _context.Add(resolution);

            var ticket = await _context.Tickets
                .Where(x=>x.Id ==id)
                .FirstOrDefaultAsync();
            ticket.StatusId = vm.StatusId;
            _context.Update(ticket);

            await _context.SaveChangesAsync(userId);

           

            TempData["MESSAGE"] = "Ticket Resolution Details successfully Created";

            return RedirectToAction("Resolve", new { id = id });
        }



        [Permission("TICKETS:UPDATE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);
            return View(ticket);
        }


        [Permission("TICKETS:UPDATE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            try
            {
                var userId = User.GetUserId();
                _context.Update(ticket);
                await _context.SaveChangesAsync(userId);
                TempData["MESSAGE"] = "Ticket Details successfully Updated";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
                return RedirectToAction(nameof(Index));
            
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "FullName", ticket.CreatedById);
            return View(ticket);
        }


        [Permission("TICKETS:DELETE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        [Permission("TICKETS:DELETE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.GetUserId();

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }











    }
}
