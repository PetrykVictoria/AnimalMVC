using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using AnimalMVC.Models;

namespace AnimalMVC.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitionsAPIController : ControllerBase
    {
        private readonly DbAnimalContext _context;
        private readonly IMemoryCache _cache;

        public ExhibitionsAPIController(DbAnimalContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/ExhibitionsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exhibition>>> GetExhibitions()
        {
            const string cacheKey = "exhibitionsCache";
            if (!_cache.TryGetValue(cacheKey, out List<Exhibition> exhibitions))
            {
                exhibitions = await _context.Exhibitions.ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)) 
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));  

                _cache.Set(cacheKey, exhibitions, cacheOptions);
            }

            return exhibitions;
        }

        // GET: api/ExhibitionsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exhibition>> GetExhibition(int id)
        {
            string cacheKey = $"exhibition_{id}";
            if (!_cache.TryGetValue(cacheKey, out Exhibition exhibition))
            {
                exhibition = await _context.Exhibitions.FindAsync(id);

                if (exhibition == null)
                {
                    return NotFound();
                }

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, exhibition, cacheOptions);
            }

            return exhibition;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutExhibition(int id, Exhibition exhibition)
        {
            if (id != exhibition.Id)
            {
                return BadRequest();
            }

            if (exhibition.Date < DateTime.Now)
            {
                return BadRequest(new { message = "Виставка не може бути в минулому часі." });
            }

            var overlappingExhibitions = await _context.Exhibitions
                .Where(e => e.Location == exhibition.Location && e.Id != exhibition.Id)
                .ToListAsync();

            if (overlappingExhibitions.Any(e => Math.Abs((e.Date - exhibition.Date).TotalHours) < 2))
            {
                return BadRequest(new { message = "На цій локації вже запланована виставка на цей час" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(exhibition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _cache.Remove("exhibitionsCache"); 
                _cache.Remove($"exhibition_{id}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExhibitionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Exhibition>> PostExhibition(Exhibition exhibition)
        {
            if (exhibition.Date < DateTime.Now)
            {
                return BadRequest(new { message = "Виставка не може бути в минулому часі." });
            }

            var overlappingExhibitions = await _context.Exhibitions
                .Where(e => e.Location == exhibition.Location && e.Id != exhibition.Id)
                .ToListAsync();

            if (overlappingExhibitions.Any(e => Math.Abs((e.Date - exhibition.Date).TotalHours) < 2))
            {
                return BadRequest(new { message = "На цій локації вже запланована виставка на цей час" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Exhibitions.Add(exhibition);
            await _context.SaveChangesAsync();
            _cache.Remove("exhibitionsCache"); 

            return CreatedAtAction("GetExhibition", new { id = exhibition.Id }, exhibition);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExhibition(int id)
        {
            var exhibition = await _context.Exhibitions.FindAsync(id);
            if (exhibition == null)
            {
                return NotFound();
            }

            _context.Exhibitions.Remove(exhibition);
            await _context.SaveChangesAsync();
            _cache.Remove("exhibitionsCache"); 
            _cache.Remove($"exhibition_{id}"); 

            return NoContent();
        }

        private bool ExhibitionExists(int id)
        {
            return _context.Exhibitions.Any(e => e.Id == id);
        }
    }
}
