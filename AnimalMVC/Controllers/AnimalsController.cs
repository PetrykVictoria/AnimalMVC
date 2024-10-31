using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimalMVC.Models;

namespace AnimalMVC.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly DbAnimalContext _context;

        public AnimalsController(DbAnimalContext context)
        {
            _context = context;
        }

        // GET: Animals
        public async Task<IActionResult> Index()
        {
            var dbAnimalContext = _context.Animals.Include(a => a.Owner).Include(a => a.Species);
            return View(await dbAnimalContext.ToListAsync());
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.Species)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "FullName");
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "Name");
            return View();
        }

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,OwnerId,SpeciesId,Breed,Age")] Animal animal, IFormFile? Image)
        {

            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await Image.CopyToAsync(ms);
                        animal.Image = ms.ToArray(); // зберігаємо фото як байтовий масив
                    }
                }

                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "FullName", animal.OwnerId);
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "Name", animal.SpeciesId);
            return View(animal);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null || animal.Image != null)
            {
                return File(animal.Image, "image/png");
            }
            return NotFound();
        }


        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "FullName", animal.OwnerId);
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "Name", animal.SpeciesId);
            return View(animal);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,OwnerId,SpeciesId,Breed,Age,Image")] Animal animal, IFormFile? Image)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await Image.CopyToAsync(ms);
                        animal.Image = ms.ToArray(); 
                    }
                }
                else
                {
                    _context.Entry(animal).Property(a => a.Image).IsModified = false;
                }
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id))
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
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "FullName", animal.OwnerId);
            ViewData["SpeciesId"] = new SelectList(_context.Species, "Id", "Name", animal.SpeciesId);
            return View(animal);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.Species)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals
                .Include(a => a.AnimalExhibitions)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (animal != null)
            {
                _context.AnimalExhibitions.RemoveRange(animal.AnimalExhibitions);
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetSpeciesStatistics()
        {
            var speciesStats = await _context.Animals
                .Include(a => a.Species)
                .GroupBy(a => a.Species.Name)
                .Select(g => new
                {
                    Species = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Json(speciesStats);
        }

    }
}
