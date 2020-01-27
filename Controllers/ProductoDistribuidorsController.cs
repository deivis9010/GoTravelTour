using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;

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
        public IEnumerable<ProductoDistribuidor> GetProductoDistribuidores(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<ProductoDistribuidor> lista;
            if (col == "-1")
            {
                return _context.ProductoDistribuidores
                    .Include( pd => pd.Distribuidor)
                    .Include (dp => dp.Producto)
                    .OrderBy(a => a.Producto.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ProductoDistribuidores
                    .Include(pd => pd.Distribuidor)
                    .Include(dp => dp.Producto)
                    .Where(p => (p.Distribuidor.Nombre.ToLower().Contains(filter.ToLower())) 
                    || p.Producto.Nombre.ToLower().Contains(filter.ToLower()))
                    .ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.ProductoDistribuidores
                    .Include(pd => pd.Distribuidor)
                    .Include(dp => dp.Producto)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Producto.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Producto.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/ProductoDistribuidors/Count
        [Route("Count")]
        [HttpGet]
        public int GetProductoDistribuidorsCount()
        {
            return _context.ProductoDistribuidores.Count();
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