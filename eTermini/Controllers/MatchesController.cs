using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eTermini.Data;
using eTermini.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace eTermini.Controllers
{
    public class MatchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatchesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Matches
        public async Task<IActionResult> Index()
        {
            var matchesList = await _context.Matches.Include(m => m.Organizer).ToListAsync();

            // Fetch LocationName based on LocationID
            if (matchesList != null && matchesList.Any())
            {
                // Retrieve all field IDs present in matches
                var fieldIds = matchesList.Select(m => m.LocationID).Distinct().ToList();

                // Fetch corresponding field names for the IDs
                var fieldNames = await _context.Fields
                    .Where(f => fieldIds.Contains(f.Id))
                    .ToDictionaryAsync(f => f.Id, f => f.FieldName);

                // Update LocationName in MatchesEntity with corresponding field names
                foreach (var match in matchesList)
                {
                    if (fieldNames.ContainsKey(match.LocationID))
                    {
                        match.LocationName = fieldNames[match.LocationID];
                    }
                }
            }

            return View(matchesList);
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Matches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var matchesEntity = await _context.Matches
                .FirstOrDefaultAsync(m => m.ID == id);
            if (matchesEntity == null)
            {
                return NotFound();
            }

            return View(matchesEntity);
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Matches/Create
        public IActionResult Create()
        {
            List<FieldsEntity> fieldsList = _context.Fields.ToList();

            ViewBag.FieldsList = fieldsList;

            return View();
        }


        [Authorize(Roles = "Admin,User")]
        // POST: Matches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrganizerID,Date,LocationID,PlayerCount,LocationName")] MatchesEntity matchesEntity)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    // Set the OrganizerID to the logged-in user's ID
                    matchesEntity.OrganizerID = currentUser.Id; // Assuming OrganizerID is of type string
                    _context.Add(matchesEntity);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Handle case when the user is not found
                    return RedirectToAction("Login", "Account"); // Redirect to login page or handle accordingly
                }

            }
            return View(matchesEntity);
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var matchesEntity = await _context.Matches.FindAsync(id);
            if (matchesEntity == null)
            {
                return NotFound();
            }

            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the logged-in user is the organizer of the match
            if (matchesEntity.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized
            }

            return View(matchesEntity);
        }

        [Authorize(Roles = "Admin,User")]
        // POST: Matches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrganizerID,Date,LocationID,PlayerCount,LocationName")] MatchesEntity matchesEntity)
        {
            if (id != matchesEntity.ID)
            {
                return NotFound();
            }

            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the logged-in user is the organizer of the match
            if (matchesEntity.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matchesEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchesEntityExists(matchesEntity.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(matchesEntity);
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Matches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var matchesEntity = await _context.Matches.FirstOrDefaultAsync(m => m.ID == id);
            if (matchesEntity == null)
            {
                return NotFound();
            }

            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the logged-in user is the organizer of the match
            if (matchesEntity.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized
            }

            return View(matchesEntity);
        }

        [Authorize(Roles = "Admin,User")]
        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Matches == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Matches'  is null.");
            }

            var matchesEntity = await _context.Matches.FindAsync(id);
            if (matchesEntity != null)
            {
                // Get the currently logged-in user
                var currentUser = await _userManager.GetUserAsync(User);

                // Check if the logged-in user is the organizer of the match
                if (matchesEntity.OrganizerID != currentUser.Id)
                {
                    return Forbid(); // Return a forbidden result if the user is not authorized
                }

                _context.Matches.Remove(matchesEntity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchesEntityExists(int id)
        {
          return (_context.Matches?.Any(e => e.ID == id)).GetValueOrDefault();
        }






        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> SendJoinRequest(int matchId)
        {
            // Get the current user sending the request
            var currentUser = await _userManager.GetUserAsync(User);

            // Ensure the user exists
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not found
            }

            // Check if the match exists
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null)
            {
                return NotFound(); // Handle case when the match is not found
            }

            // Check if a request from the current user for this match already exists
            var existingRequest = await _context.JoinRequests
                .FirstOrDefaultAsync(jr => jr.UserId == currentUser.Id && jr.MatchId == matchId);

            if (existingRequest != null)
            {
                // Handle case when a request already exists for this user and match
                // You can display a message or redirect to prevent duplicate requests
                return RedirectToAction("Details", new { id = matchId });
            }

            // Create a new join request
            var joinRequest = new JoinRequest
            {
                UserId = currentUser.Id,
                MatchId = matchId,
                Status = "Pending" // Set the status as Pending by default
            };

            _context.JoinRequests.Add(joinRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = matchId });
        }

        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ManageJoinRequests(int matchId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Ensure the user exists
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not found
            }

            // Retrieve the match
            var match = await _context.Matches
                .Include(m => m.JoinRequests)
                .FirstOrDefaultAsync(m => m.ID == matchId);

            // Ensure the match exists
            if (match == null)
            {
                return NotFound(); // Handle case when the match is not found
            }

            // Check if the current user is the organizer of the match
            if (match.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized to manage requests
            }

            return View(match);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> AcceptJoinRequest(int requestId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Ensure the user exists
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not found
            }

            // Retrieve the request
            var request = await _context.JoinRequests.FindAsync(requestId);

            // Ensure the request exists
            if (request == null)
            {
                return NotFound(); // Handle case when the request is not found
            }

            // Check if the current user is the organizer of the match related to this request
            var relatedMatch = await _context.Matches.FindAsync(request.MatchId);
            if (relatedMatch.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized to accept requests
            }

            // Update the status of the request to Accepted
            request.Status = "Accepted";
            await _context.SaveChangesAsync();

            // Implement further logic such as updating the participant list, sending notifications, etc.

            return RedirectToAction("ManageJoinRequests", new { matchId = relatedMatch.ID });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> RejectJoinRequest(int requestId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Ensure the user exists
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not found
            }

            // Retrieve the request
            var request = await _context.JoinRequests.FindAsync(requestId);

            // Ensure the request exists
            if (request == null)
            {
                return NotFound(); // Handle case when the request is not found
            }

            // Check if the current user is the organizer of the match related to this request
            var relatedMatch = await _context.Matches.FindAsync(request.MatchId);
            if (relatedMatch.OrganizerID != currentUser.Id)
            {
                return Forbid(); // Return a forbidden result if the user is not authorized to reject requests
            }

            // Update the status of the request to Rejected
            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            // Implement further logic such as sending notifications, etc.

            return RedirectToAction("ManageJoinRequests", new { matchId = relatedMatch.ID });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var joinRequest = await _context.JoinRequests.FindAsync(requestId);

            if (joinRequest != null)
            {
                // Update the status of the join request to 'Accepted'
                joinRequest.Status = "Accepted";
                _context.Update(joinRequest);
                await _context.SaveChangesAsync();
            }

            // Redirect back to the join requests view
            return RedirectToAction("JoinRequests", new { id = joinRequest.MatchId });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var joinRequest = await _context.JoinRequests.FindAsync(requestId);

            if (joinRequest != null)
            {
                // Update the status of the join request to 'Rejected'
                joinRequest.Status = "Rejected";
                _context.Update(joinRequest);
                await _context.SaveChangesAsync();
            }

            // Redirect back to the join requests view
            return RedirectToAction("JoinRequests", new { id = joinRequest.MatchId });
        }






    }
}
