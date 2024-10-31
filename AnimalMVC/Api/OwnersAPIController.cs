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
    public class OwnersAPIController : ControllerBase
    {
        private readonly DbAnimalContext _context;

        public OwnersAPIController(DbAnimalContext context)
        {
            _context = context;
        }

        // GET: api/OwnersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Owner>>> GetOwners()
        {
            return await _context.Owners.ToListAsync();
        }

        // GET: api/OwnersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Owner>> GetOwner(int id)
        {
            var owner = await _context.Owners.FindAsync(id);

            if (owner == null)
            {
                return NotFound();
            }

            return owner;
        }

        // PUT: api/OwnersAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOwner(int id, Owner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest();
            }

            if (_context.Owners.Any(o => o.Email == owner.Email && o.Id != owner.Id))
            {
                return BadRequest(new { message = "Власник з такою електронною поштою вже існує." });
            }

            if (_context.Owners.Any(o => o.Phone == owner.Phone && o.Id != owner.Id))
            {
                return BadRequest(new { message = "Власник з таким номером телефону вже існує." } );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(owner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(id))
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

        // POST: api/OwnersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Owner>> PostOwner(Owner owner)
        {
            if (_context.Owners.Any(o => o.Email == owner.Email && o.Id != owner.Id))
            {
                return BadRequest(new { message = "Власник з такою електронною поштою вже існує." });
            }

            if (_context.Owners.Any(o => o.Phone == owner.Phone && o.Id != owner.Id))
            {
                return BadRequest(new { message = "Власник з таким номером телефону вже існує." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOwner", new { id = owner.Id }, owner);
        }

        // DELETE: api/OwnersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.Id == id);
        }
    }
}
