using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ProductoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Productoes
        [HttpGet]
        public IEnumerable<Producto> GetProductos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Producto> lista;
            if (col == "-1")
            {
                return _context.Productos.OrderBy(a=>a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Productos.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Productos.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/Productoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetProductoCount()
        {
            return _context.Productos.Count();
        }

        // GET: api/Productoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        // PUT: api/Productoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProducto([FromRoute] int id, [FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != producto.ProductoId)
            {
                return BadRequest();
            }
            if (_context.Productos.Any(c => c.Nombre == producto.Nombre && producto.ProductoId != id))
            {
                return CreatedAtAction("GetProductos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostProducto([FromBody] Producto producto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Productos.Any(c => c.Nombre == producto.Nombre ))
            {
                return CreatedAtAction("GetProductos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.ProductoId }, producto);
        }

        // DELETE: api/Productoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}