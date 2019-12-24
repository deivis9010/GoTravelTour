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
    public class ProductoDistribuidorsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ProductoDistribuidorsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ProductoDistribuidors
        [HttpGet]
        public IEnumerable<ProductoDistribuidor> GetProductoDistribuidores()
        {
            return _context.ProductoDistribuidores;
        }

        // GET: api/ProductoDistribuidors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoDistribuidor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productoDistribuidor = await _context.ProductoDistribuidores.FindAsync(id);

            if (productoDistribuidor == null)
            {
                return NotFound();
            }

            return Ok(productoDistribuidor);
        }

        // PUT: api/ProductoDistribuidors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductoDistribuidor([FromRoute] int id, [FromBody] ProductoDistribuidor productoDistribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productoDistribuidor.ProductoDistribuidorId)
            {
                return BadRequest();
            }

            _context.Entry(productoDistribuidor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoDistribuidorExists(id))
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

        // POST: api/ProductoDistribuidors
        [HttpPost]
        public async Task<IActionResult> PostProductoDistribuidor([FromBody] ProductoDistribuidor productoDistribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductoDistribuidores.Add(productoDistribuidor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductoDistribuidor", new { id = productoDistribuidor.ProductoDistribuidorId }, productoDistribuidor);
        }

        // DELETE: api/ProductoDistribuidors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductoDistribuidor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productoDistribuidor = await _context.ProductoDistribuidores.FindAsync(id);
            if (productoDistribuidor == null)
            {
                return NotFound();
            }

            _context.ProductoDistribuidores.Remove(productoDistribuidor);
            await _context.SaveChangesAsync();

            return Ok(productoDistribuidor);
        }

        private bool ProductoDistribuidorExists(int id)
        {
            return _context.ProductoDistribuidores.Any(e => e.ProductoDistribuidorId == id);
        }
    }
}