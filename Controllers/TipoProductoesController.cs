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
    public class TipoProductoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoProductoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoProductoes
        [HttpGet]
        public IEnumerable<TipoProducto> GetTipoProductos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoProducto> lista;
            if (col == "-1")
            {
                return _context.TipoProductos
                    .OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoProductos.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoProductos.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/TipoProductoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoProductoesCount()
        {
            return _context.TipoProductos.Count();
        }


        // GET: api/TipoProductoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoProducto = await _context.TipoProductos.FindAsync(id);

            if (tipoProducto == null)
            {
                return NotFound();
            }

            return Ok(tipoProducto);
        }

        // PUT: api/TipoProductoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTipoProducto([FromRoute] int id, [FromBody] TipoProducto tipoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoProducto.TipoProductoId)
            {
                return BadRequest();
            }
            
            if (_context.TipoProductos.Any(c => c.Nombre == tipoProducto.Nombre && tipoProducto.TipoProductoId != id))
            {
                return CreatedAtAction("GetTipoProductos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(tipoProducto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoProductoExists(id))
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

        // POST: api/TipoProductoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostTipoProducto([FromBody] TipoProducto tipoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (_context.TipoProductos.Any(c => c.Nombre == tipoProducto.Nombre))
            {
                return CreatedAtAction("GetTipoProductos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.TipoProductos.Add(tipoProducto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoProducto", new { id = tipoProducto.TipoProductoId }, tipoProducto);
        }

        // DELETE: api/TipoProductoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTipoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoProducto = await _context.TipoProductos.FindAsync(id);
            if (tipoProducto == null)
            {
                return NotFound();
            }

            _context.TipoProductos.Remove(tipoProducto);
            await _context.SaveChangesAsync();

            return Ok(tipoProducto);
        }

        private bool TipoProductoExists(int id)
        {
            return _context.TipoProductos.Any(e => e.TipoProductoId == id);
        }
    }
}