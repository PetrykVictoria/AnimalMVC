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
    public class AnimalExhibitionsController : Controller
    {
        private readonly DbAnimalContext _context;

        public AnimalExhibitionsController(DbAnimalContext context)
        {
            _context = context;
        }

        // GET: AnimalExhibitions
        public async Task<IActionResult> Index()
        {
            var dbAnimalContext = _context.AnimalExhibitions.Include(a => a.Animal).Include(a => a.Exhibition);
            return View(await dbAnimalContext.ToListAsync());
        }

        // GET: AnimalExhibitions/Details/5
        public async Task<IActionResult> Details(int? animalId, int? exhibitionId)
        {
            if (animalId == null || exhibitionId == null)
            {
                return NotFound();
            }

            var animalExhibition = await _context.AnimalExhibitions
                .Include(a => a.Animal)
                .Include(a => a.Exhibition)
                .FirstOrDefaultAsync(m => m.AnimalId == animalId && m.ExhibitionId == exhibitionId);
            if (animalExhibition == null)
            {
                return NotFound();
            }

            return View(animalExhibition);
        }

        // GET: AnimalExhibitions/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name");
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibitions, "Id", "Name");
            return View();
        }

        // POST: AnimalExhibitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnimalId,ExhibitionId,RegistrationDate")] AnimalExhibition animalExhibition)
        {
            if (ModelState.IsValid)
            {
                bool isAlreadyRegistered = await _context.AnimalExhibitions
                    .AnyAsync(ae => ae.AnimalId == animalExhibition.AnimalId && ae.ExhibitionId == animalExhibition.ExhibitionId);

                if (isAlreadyRegistered)
                {
                    ModelState.AddModelError(string.Empty, "Ця тварина вже зареєстрована на цю виставку.");
                }
                else
                {
                    animalExhibition.RegistrationDate = DateTime.Now;
                    _context.Add(animalExhibition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", animalExhibition.AnimalId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibitions, "Id", "Name", animalExhibition.ExhibitionId);
            return View(animalExhibition);
        }

        // GET: AnimalExhibitions/Edit/5
        public async Task<IActionResult> Edit(int? animalId, int? exhibitionId)
        {
            if (animalId == null || exhibitionId == null)
            {
                return NotFound();
            }

            var animalExhibition = await _context.AnimalExhibitions
                .Include(a => a.Animal)
                .Include(a => a.Exhibition)
                .FirstOrDefaultAsync(m => m.AnimalId == animalId && m.ExhibitionId == exhibitionId);
            if (animalExhibition == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", animalExhibition.AnimalId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibitions, "Id", "Name", animalExhibition.ExhibitionId);
            return View(animalExhibition);
        }

        // GET: AnimalExhibitions/GetExhibitionAnimalCounts
        [HttpGet]
        public IActionResult GetExhibitionAnimalCounts()
        {
            var data = _context.AnimalExhibitions
                .GroupBy(ae => ae.Exhibition.Name)
                .Select(group => new
                {
                    ExhibitionName = group.Key,
                    AnimalCount = group.Count()
                })
                .ToList();

            return Json(data);
        }



        // POST: AnimalExhibitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? animalId, int? exhibitionId, [Bind("AnimalId,ExhibitionId,RegistrationDate")] AnimalExhibition animalExhibition)
        {
            if (animalId == null || exhibitionId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animalExhibition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExhibitionExists(animalExhibition.AnimalId, animalExhibition.ExhibitionId))
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
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", animalExhibition.AnimalId);
            ViewData["ExhibitionId"] = new SelectList(_context.Exhibitions, "Id", "Name", animalExhibition.ExhibitionId);
            return View(animalExhibition);
        }

        // GET: AnimalExhibitions/Delete/5
        public async Task<IActionResult> Delete(int? animalId, int? exhibitionId)
        {
            if (animalId == null || exhibitionId == null)
            {
                return NotFound();
            }

            var animalExhibition = await _context.AnimalExhibitions
                .Include(a => a.Animal)
                .Include(a => a.Exhibition)
                .FirstOrDefaultAsync(m => m.AnimalId == animalId && m.ExhibitionId == exhibitionId);
            if (animalExhibition == null)
            {
                return NotFound();
            }

            return View(animalExhibition);
        }

        // POST: AnimalExhibitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int animalId, int exhibitionId)
        {
            var animalExhibition = await _context.AnimalExhibitions.FindAsync(animalId, exhibitionId);
            if (animalExhibition != null)
            {
                _context.AnimalExhibitions.Remove(animalExhibition);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool AnimalExhibitionExists(int? animalId, int? exhibitionId)
        {
            return _context.AnimalExhibitions.Any(e => e.AnimalId == animalId && e.ExhibitionId == exhibitionId);
        }
    }
}
