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
    public class PrecioComodidadesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioComodidadesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioComodidades
        [HttpGet]
        public IEnumerable<PrecioComodidades> GetPrecioComodidades(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PrecioComodidades> lista;
            if (col == "-1")
            {
                return _context.PrecioComodidades
                    .Include(a => a.Comodidad)
                    .Include(a => a.Producto)
                   
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioComodidades
                    .Include(a => a.Comodidad)
                    .Include(a => a.Producto)
                    .Where(p => (p.Comodidad.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.PrecioComodidades
                    .Include(a => a.Comodidad)
                    .Include(a => a.Producto)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Comodidad.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Comodidad.Nombre);

                        }
                    }

                    break;
            }

            return lista;
        }
        // GET: api/PrecioComodidades/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioComodidadesCount()
        {
            return _context.PrecioComodidades.Count();
        }

        // GET: api/PrecioComodidades/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioComodidades([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioComodidades = await _context.PrecioComodidades.FindAsync(id);

            if (precioComodidades == null)
            {
                return NotFound();
            }

            return Ok(precioComodidades);
        }

        // PUT: api/PrecioComodidades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioComodidades([FromRoute] int id, [FromBody] PrecioComodidades precioComodidades)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioComodidades.PrecioComodidadesId)
            {
                return BadRequest();
            }

            _context.Entry(precioComodidades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioComodidadesExists(id))
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

        // POST: api/PrecioComodidades
        [HttpPost]
        public async Task<IActionResult> PostPrecioComodidades([FromBody] PrecioComodidades precioComodidades)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PrecioComodidades.Add(precioComodidades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioComodidades", new { id = precioComodidades.PrecioComodidadesId }, precioComodidades);
        }

        // DELETE: api/PrecioComodidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioComodidades([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioComodidades = await _context.PrecioComodidades.FindAsync(id);
            if (precioComodidades == null)
            {
                return NotFound();
            }

            _context.PrecioComodidades.Remove(precioComodidades);
            await _context.SaveChangesAsync();

            return Ok(precioComodidades);
        }

        private bool PrecioComodidadesExists(int id)
        {
            return _context.PrecioComodidades.Any(e => e.PrecioComodidadesId == id);
        }
    }
}