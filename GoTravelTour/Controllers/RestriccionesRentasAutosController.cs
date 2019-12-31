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
    public class RestriccionesRentasAutosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RestriccionesRentasAutosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/RestriccionesRentasAutos
        [HttpGet]
        public IEnumerable<RestriccionesRentasAutos> GetRestriccionesRentasAutos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<RestriccionesRentasAutos> lista;
            if (col == "-1")
            {
                return _context.RestriccionesRentasAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Auto)                    
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.RestriccionesRentasAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Auto)
                    .Where(p => (p.Auto.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.RestriccionesRentasAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Contrato)
                    .Include(a => a.Auto)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Auto.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Auto.Nombre);

                        }
                    }

                    break;
            }

            return lista;
        }
        // GET: api/RestriccionesRentasAutos/Count
        [Route("Count")]
        [HttpGet]
        public int GetRestriccionesRentasAutosCount()
        {
            return _context.RestriccionesRentasAutos.Count();
        }

        // GET: api/RestriccionesRentasAutos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestriccionesRentasAutos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesRentasAutos = await _context.RestriccionesRentasAutos.FindAsync(id);

            if (restriccionesRentasAutos == null)
            {
                return NotFound();
            }

            return Ok(restriccionesRentasAutos);
        }

        // PUT: api/RestriccionesRentasAutos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestriccionesRentasAutos([FromRoute] int id, [FromBody] RestriccionesRentasAutos restriccionesRentasAutos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restriccionesRentasAutos.RestriccionesRentasAutosId)
            {
                return BadRequest();
            }

            _context.Entry(restriccionesRentasAutos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestriccionesRentasAutosExists(id))
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

        // POST: api/RestriccionesRentasAutos
        [HttpPost]
        public async Task<IActionResult> PostRestriccionesRentasAutos([FromBody] RestriccionesRentasAutos restriccionesRentasAutos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RestriccionesRentasAutos.Add(restriccionesRentasAutos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestriccionesRentasAutos", new { id = restriccionesRentasAutos.RestriccionesRentasAutosId }, restriccionesRentasAutos);
        }

        // DELETE: api/RestriccionesRentasAutos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestriccionesRentasAutos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restriccionesRentasAutos = await _context.RestriccionesRentasAutos.FindAsync(id);
            if (restriccionesRentasAutos == null)
            {
                return NotFound();
            }

            _context.RestriccionesRentasAutos.Remove(restriccionesRentasAutos);
            await _context.SaveChangesAsync();

            return Ok(restriccionesRentasAutos);
        }

        private bool RestriccionesRentasAutosExists(int id)
        {
            return _context.RestriccionesRentasAutos.Any(e => e.RestriccionesRentasAutosId == id);
        }
    }
}