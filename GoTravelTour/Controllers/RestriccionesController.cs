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
    public class RestriccionesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RestriccionesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Restricciones
        [HttpGet]
        public IEnumerable<Restricciones> GetRestricciones(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Restricciones> lista;
            if (col == "-1")
            {
                return _context.Restricciones
                    .Include(a => a.Temporada)                  
                    
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Restricciones
                    .Include(a => a.Temporada)
                    
                    .Where(p => (p.Temporada.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Restricciones
                    .Include(a => a.Temporada)
                    
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Temporada.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Temporada.Nombre);

                        }
                    }

                    break;
            }

            return lista;
        }
        // GET: api/RestriccionesActividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetRestriccionesActividadesCount()
        {
            return _context.Restricciones.Count();
        }

        // GET: api/Restricciones/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRestricciones([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restricciones = await _context.Restricciones.FindAsync(id);

            if (restricciones == null)
            {
                return NotFound();
            }

            return Ok(restricciones);
        }

        // PUT: api/Restricciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRestricciones([FromRoute] int id, [FromBody] Restricciones restricciones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restricciones.RestriccionesId)
            {
                return BadRequest();
            }

            _context.Entry(restricciones).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestriccionesExists(id))
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

        // POST: api/Restricciones
        [HttpPost]
        public async Task<IActionResult> PostRestricciones([FromBody] Restricciones restricciones)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Restricciones.Add(restricciones);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRestricciones", new { id = restricciones.RestriccionesId }, restricciones);
        }

        // DELETE: api/Restricciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestricciones([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var restricciones = await _context.Restricciones.FindAsync(id);
            if (restricciones == null)
            {
                return NotFound();
            }

            _context.Restricciones.Remove(restricciones);
            await _context.SaveChangesAsync();

            return Ok(restricciones);
        }

        private bool RestriccionesExists(int id)
        {
            return _context.Restricciones.Any(e => e.RestriccionesId == id);
        }
    }
}