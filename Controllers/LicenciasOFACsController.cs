using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenciasOFACsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public LicenciasOFACsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/LicenciasOFACs
        [HttpGet]
        public IEnumerable<LicenciasOFAC> GetLicenciasOFAC()
        {
            return _context.LicenciasOFAC;
        }

        // GET: api/LicenciasOFACs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLicenciasOFAC([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var licenciasOFAC = await _context.LicenciasOFAC.FindAsync(id);

            if (licenciasOFAC == null)
            {
                return NotFound();
            }

            return Ok(licenciasOFAC);
        }

        // PUT: api/LicenciasOFACs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLicenciasOFAC([FromRoute] int id, [FromBody] LicenciasOFAC licenciasOFAC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != licenciasOFAC.LicenciasOFACId)
            {
                return BadRequest();
            }

            _context.Entry(licenciasOFAC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LicenciasOFACExists(id))
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

        // POST: api/LicenciasOFACs
        [HttpPost]
        public async Task<IActionResult> PostLicenciasOFAC([FromBody] LicenciasOFAC licenciasOFAC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LicenciasOFAC.Add(licenciasOFAC);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLicenciasOFAC", new { id = licenciasOFAC.LicenciasOFACId }, licenciasOFAC);
        }

        // DELETE: api/LicenciasOFACs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLicenciasOFAC([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var licenciasOFAC = await _context.LicenciasOFAC.FindAsync(id);
            if (licenciasOFAC == null)
            {
                return NotFound();
            }

            _context.LicenciasOFAC.Remove(licenciasOFAC);
            await _context.SaveChangesAsync();

            return Ok(licenciasOFAC);
        }

        private bool LicenciasOFACExists(int id)
        {
            return _context.LicenciasOFAC.Any(e => e.LicenciasOFACId == id);
        }
    }
}