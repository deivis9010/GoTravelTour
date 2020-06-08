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
    public class OrdenVehiculoPrecioRentaAutoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenVehiculoPrecioRentaAutoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenVehiculoPrecioRentaAutoes
        [HttpGet]
        public IEnumerable<OrdenVehiculoPrecioRentaAuto> GetOrdenVehiculoPrecioRentaAuto()
        {
            return _context.OrdenVehiculoPrecioRentaAuto;
        }

        // GET: api/OrdenVehiculoPrecioRentaAutoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenVehiculoPrecioRentaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenVehiculoPrecioRentaAuto = await _context.OrdenVehiculoPrecioRentaAuto.FindAsync(id);

            if (ordenVehiculoPrecioRentaAuto == null)
            {
                return NotFound();
            }

            return Ok(ordenVehiculoPrecioRentaAuto);
        }

        // PUT: api/OrdenVehiculoPrecioRentaAutoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenVehiculoPrecioRentaAuto([FromRoute] int id, [FromBody] OrdenVehiculoPrecioRentaAuto ordenVehiculoPrecioRentaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenVehiculoPrecioRentaAuto.OrdenVehiculoPrecioRentaAutoId)
            {
                return BadRequest();
            }

            _context.Entry(ordenVehiculoPrecioRentaAuto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenVehiculoPrecioRentaAutoExists(id))
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

        // POST: api/OrdenVehiculoPrecioRentaAutoes
        [HttpPost]
        public async Task<IActionResult> PostOrdenVehiculoPrecioRentaAuto([FromBody] OrdenVehiculoPrecioRentaAuto ordenVehiculoPrecioRentaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenVehiculoPrecioRentaAuto.Add(ordenVehiculoPrecioRentaAuto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenVehiculoPrecioRentaAuto", new { id = ordenVehiculoPrecioRentaAuto.OrdenVehiculoPrecioRentaAutoId }, ordenVehiculoPrecioRentaAuto);
        }

        // DELETE: api/OrdenVehiculoPrecioRentaAutoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenVehiculoPrecioRentaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenVehiculoPrecioRentaAuto = await _context.OrdenVehiculoPrecioRentaAuto.FindAsync(id);
            if (ordenVehiculoPrecioRentaAuto == null)
            {
                return NotFound();
            }

            _context.OrdenVehiculoPrecioRentaAuto.Remove(ordenVehiculoPrecioRentaAuto);
            await _context.SaveChangesAsync();

            return Ok(ordenVehiculoPrecioRentaAuto);
        }

        private bool OrdenVehiculoPrecioRentaAutoExists(int id)
        {
            return _context.OrdenVehiculoPrecioRentaAuto.Any(e => e.OrdenVehiculoPrecioRentaAutoId == id);
        }
    }
}