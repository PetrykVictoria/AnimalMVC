using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimalMVC.Models;
using Newtonsoft.Json;

namespace AnimalMVC.Controllers
{
    public class ExhibitionsController : Controller
    {
        private readonly DbAnimalContext _context;

        public ExhibitionsController(DbAnimalContext context)
        {
            _context = context;
        }

        // GET: Exhibitions
        public async Task<IActionResult> Index()
        {
            await UpdateExhibitionStatus();
            return View(await _context.Exhibitions.ToListAsync());
        }

        // GET: Exhibitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibition = _context.Exhibitions
                .Include(e => e.AnimalExhibitions)
                .ThenInclude(ae => ae.Animal)
                .FirstOrDefault(e => e.Id == id);

        
            if (exhibition == null)
            {
                return NotFound();
            }

            return View(exhibition);
        }

        [HttpGet]
        public async Task<IActionResult> GetCoordinates()
        {
            var exhibitions = await _context.Exhibitions.ToListAsync();
            var coordinates = new List<object>();

            foreach (var exhibition in exhibitions)
            {
                var address = exhibition.Location; // Ваша локація
                var response = await new HttpClient().GetStringAsync($"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json");

                var location = JsonConvert.DeserializeObject<List<NominatimResult>>(response).FirstOrDefault();

                if (location != null)
                {
                    coordinates.Add(new
                    {
                        Name = exhibition.Name,
                        Coordinates = new { lat = location.Lat, lon = location.Lon },
                        IsClosed = exhibition.IsClosed
                    });
                }
            }

            return Json(coordinates);
        }

        // Клас для десеріалізації відповіді Nominatim
        public class NominatimResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }




        // GET: Exhibitions/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> UpdateExhibitionStatus()
        {
            var exhibitions = await _context.Exhibitions.ToListAsync();
            foreach (var exhibition in exhibitions)
            {
                if (exhibition.Date <= DateTime.Now && !exhibition.IsClosed)
                {
                    exhibition.IsClosed = true;
                    _context.Update(exhibition);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Exhibitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Date,Description")] Exhibition exhibitions)
        {
            if (exhibitions.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Виставка не може бути в минулому часі.");
            }

            var overlappingExhibitions = await _context.Exhibitions
                .Where(e => e.Location == exhibitions.Location)
                .ToListAsync();

            if (overlappingExhibitions.Any(e => Math.Abs((e.Date - exhibitions.Date).TotalHours) < 2))
            {
                ModelState.AddModelError("Location", "На цій локації вже запланована виставка на цей час.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(exhibitions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exhibitions);
        }


        // GET: Exhibitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitions = await _context.Exhibitions.FindAsync(id);
            if (exhibitions == null)
            {
                return NotFound();
            }
            return View(exhibitions);
        }

        // POST: Exhibitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Date,Description")] Exhibition exhibitions)
        {
            if (id != exhibitions.Id)
            {
                return NotFound();
            }

            if (exhibitions.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Виставка не може бути в минулому часі.");
            }

            var overlappingExhibitions = await _context.Exhibitions
                .Where(e => e.Location == exhibitions.Location && e.Id != exhibitions.Id)
                .ToListAsync();

            if (overlappingExhibitions.Any(e => Math.Abs((e.Date - exhibitions.Date).TotalHours) < 2))
            {
                ModelState.AddModelError("Location", "На цій локації вже запланована виставка на цей час.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exhibitions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExhibitionsExists(exhibitions.Id))
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
            return View(exhibitions);
        }

        // GET: Exhibitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exhibitions = await _context.Exhibitions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exhibitions == null)
            {
                return NotFound();
            }

            return View(exhibitions);
        }

        // POST: Exhibitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exhibitions = await _context.Exhibitions
                .Include(e => e.AnimalExhibitions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exhibitions != null)
            {
                _context.AnimalExhibitions.RemoveRange(exhibitions.AnimalExhibitions);
                _context.Exhibitions.Remove(exhibitions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExhibitionsExists(int id)
        {
            return _context.Exhibitions.Any(e => e.Id == id);
        }
    }
}
