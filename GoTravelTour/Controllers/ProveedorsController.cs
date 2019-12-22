using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ProveedorsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Proveedors
        [HttpGet]
        public IEnumerable<Proveedor> GetProveedor(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Proveedor> lista;
            if (col == "-1")
            {
                return _context.Proveedores.Include(r => r.Productos).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Proveedores.Include(r => r.Productos)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Proveedores.Include(r => r.Productos).ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/Proveedors/Count
        [Route("Count")]
        [HttpGet]
        public int GetProveedorCount()
        {
            return _context.Proveedores.Count();
        }

        // GET: api/Proveedors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProveedor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            

            if (proveedor == null)
            {
                return NotFound();
            }
            proveedor.Productos = _context.Productos.Where(p => p.ProductoId == proveedor.ProveedorId).ToList();

            return Ok(proveedor);
        }

        // PUT: api/Proveedors/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProveedor([FromRoute] int id, [FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != proveedor.ProveedorId)
            {
                return BadRequest();
            }
            
            if (_context.Proveedores.Any(c => c.Nombre == proveedor.Nombre && proveedor.ProveedorId != id))
            {
                return CreatedAtAction("GetProveedor", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
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

        // POST: api/Proveedors
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostProveedor([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (_context.Proveedores.Any(c => c.Nombre == proveedor.Nombre))
            {
                return CreatedAtAction("GetProveedor", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProveedor", new { id = proveedor.ProveedorId }, proveedor);
        }

        // DELETE: api/Proveedors/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProveedor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return Ok(proveedor);
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.ProveedorId == id);
        }
    }
}