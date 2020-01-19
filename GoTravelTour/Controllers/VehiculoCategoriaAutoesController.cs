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
    public class VehiculoCategoriaAutoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public VehiculoCategoriaAutoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/VehiculoCategoriaAutoes
        [HttpGet]
        public IEnumerable<VehiculoCategoriaAuto> GetVehiculoCategoriaAuto()
        {
            return _context.VehiculoCategoriaAuto;
        }

        // GET: api/VehiculoCategoriaAutoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehiculoCategoriaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculoCategoriaAuto = await _context.VehiculoCategoriaAuto.FindAsync(id);

            if (vehiculoCategoriaAuto == null)
            {
                return NotFound();
            }

            return Ok(vehiculoCategoriaAuto);
        }

        // PUT: api/VehiculoCategoriaAutoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculoCategoriaAuto([FromRoute] int id, [FromBody] VehiculoCategoriaAuto vehiculoCategoriaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehiculoCategoriaAuto.VehiculoCategoriaAutoId)
            {
                return BadRequest();
            }

            _context.Entry(vehiculoCategoriaAuto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoCategoriaAutoExists(id))
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

        // POST: api/VehiculoCategoriaAutoes
        [HttpPost]
        public async Task<IActionResult> PostVehiculoCategoriaAuto([FromBody] VehiculoCategoriaAuto vehiculoCategoriaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.VehiculoCategoriaAuto.Add(vehiculoCategoriaAuto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehiculoCategoriaAuto", new { id = vehiculoCategoriaAuto.VehiculoCategoriaAutoId }, vehiculoCategoriaAuto);
        }

        // DELETE: api/VehiculoCategoriaAutoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculoCategoriaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculoCategoriaAuto = await _context.VehiculoCategoriaAuto.FindAsync(id);
            if (vehiculoCategoriaAuto == null)
            {
                return NotFound();
            }

            _context.VehiculoCategoriaAuto.Remove(vehiculoCategoriaAuto);
            await _context.SaveChangesAsync();

            return Ok(vehiculoCategoriaAuto);
        }

        private bool VehiculoCategoriaAutoExists(int id)
        {
            return _context.VehiculoCategoriaAuto.Any(e => e.VehiculoCategoriaAutoId == id);
        }
    }
}