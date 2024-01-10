using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eTermini.Data;
using eTermini.Models;
using Microsoft.AspNetCore.Authorization;

namespace eTermini.Controllers
{
    
    public class FieldsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FieldsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,User")]
        // GET: Fields
        public async Task<IActionResult> Index()
        {
              return _context.Fields != null ? 
                          View(await _context.Fields.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Fields'  is null.");
        }


        [Authorize(Roles = "Admin,User")]
        // GET: Fields/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var fieldsEntity = await _context.Fields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fieldsEntity == null)
            {
                return NotFound();
            }

            return View(fieldsEntity);
        }

        [Authorize(Roles = "Admin")]
        // GET: Fields/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Fields/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FieldName,Location,Price")] FieldsEntity fieldsEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fieldsEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fieldsEntity);
        }

        [Authorize(Roles = "Admin")]
        // GET: Fields/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var fieldsEntity = await _context.Fields.FindAsync(id);
            if (fieldsEntity == null)
            {
                return NotFound();
            }
            return View(fieldsEntity);
        }

        [Authorize(Roles = "Admin")]
        // POST: Fields/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FieldName,Location,Price")] FieldsEntity fieldsEntity)
        {
            if (id != fieldsEntity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fieldsEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FieldsEntityExists(fieldsEntity.Id))
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
            return View(fieldsEntity);
        }

        [Authorize(Roles = "Admin")]
        // GET: Fields/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var fieldsEntity = await _context.Fields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fieldsEntity == null)
            {
                return NotFound();
            }

            return View(fieldsEntity);
        }

        [Authorize(Roles = "Admin")]
        // POST: Fields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fields == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Fields'  is null.");
            }
            var fieldsEntity = await _context.Fields.FindAsync(id);
            if (fieldsEntity != null)
            {
                _context.Fields.Remove(fieldsEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FieldsEntityExists(int id)
        {
          return (_context.Fields?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
