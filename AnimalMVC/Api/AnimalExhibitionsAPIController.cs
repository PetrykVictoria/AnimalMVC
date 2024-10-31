using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimalMVC.Models;

namespace AnimalMVC.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalExhibitionsAPIController : ControllerBase
    {
        private readonly DbAnimalContext _context;

        public AnimalExhibitionsAPIController(DbAnimalContext context)
        {
            _context = context;
        }

        // GET: api/AnimalExhibitionsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalExhibition>>> GetAnimalExhibitions()
        {
            return await _context.AnimalExhibitions.ToListAsync();
        }

        // GET: api/AnimalExhibitionsAPI/5
        [HttpGet("{animalId}/{exhibitionId}")]
        public async Task<ActionResult<AnimalExhibition>> GetAnimalExhibition(int animalId, int exhibitionId)
        {
            var animalExhibition = await _context.AnimalExhibitions.FindAsync(animalId, exhibitionId);

            if (animalExhibition == null)
            {
                return NotFound();
            }

            return animalExhibition;
        }

        // PUT: api/AnimalExhibitionsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{animalId}/{exhibitionId}")]
        public async Task<IActionResult> PutAnimalExhibition(int animalId, int exhibitionId, AnimalExhibition animalExhibition)
        {
            if (animalId != animalExhibition.AnimalId || exhibitionId != animalExhibition.ExhibitionId)
            {
                return BadRequest();
            }

            _context.Entry(animalExhibition).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExhibitionExists(animalId, exhibitionId))
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

        // POST: api/AnimalExhibitionsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AnimalExhibition>> PostAnimalExhibition(AnimalExhibition animalExhibition)
        {

            bool isAlreadyRegistered = await _context.AnimalExhibitions
                   .AnyAsync(ae => ae.AnimalId == animalExhibition.AnimalId && ae.ExhibitionId == animalExhibition.ExhibitionId);

            if (isAlreadyRegistered)
            {
                return BadRequest(new { message = "Ця тварина вже зареєстрована на цю виставку." });
            }
            _context.AnimalExhibitions.Add(animalExhibition);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AnimalExhibitionExists(animalExhibition.AnimalId, animalExhibition.ExhibitionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAnimalExhibition", new { id = animalExhibition.AnimalId, exhibitionId = animalExhibition.ExhibitionId }, animalExhibition);
        }

        // DELETE: api/AnimalExhibitionsAPI/5
        [HttpDelete("{animalId}/{exhibitionId}")]
        public async Task<IActionResult> DeleteAnimalExhibition(int animalId, int exhibitionId)
        {
            var animalExhibition = await _context.AnimalExhibitions.FindAsync(animalId, exhibitionId);
            if (animalExhibition == null)
            {
                return NotFound();
            }

            _context.AnimalExhibitions.Remove(animalExhibition);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalExhibitionExists(int animalId, int exhibitionId)
        {
            return _context.AnimalExhibitions.Any(e => e.AnimalId == animalId && e.ExhibitionId == exhibitionId);
        }
    }
}
