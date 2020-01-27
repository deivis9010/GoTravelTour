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
    public class RegionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RegionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Regions
        [HttpGet]
        public IEnumerable<Region> GetRegiones(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Region> lista;
            if (col == "-1")
            {
                return _context.Regiones.Include(r => r.PuntosDeInteres)
                    .OrderBy(a=>a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Regiones.Include(r=>r.PuntosDeInteres)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize)
                    .ToList(); ;
            }
            else
            {
                lista = _context.Regiones.Include(r => r.PuntosDeInteres)
                    .ToPagedList(pageIndex, pageSize).ToList();
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

        
        // GET: api/Regions/Count
        [Route("Count")]
        [HttpGet]
        public int GetRegionsCount()
        {
            return _context.Regiones.Count();
        }

        // GET: api/Regions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRegion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = await _context.Regiones.FindAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            return Ok(region);
        }

        // PUT: api/Regions/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRegion([FromRoute] int id, [FromBody] Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != region.RegionId)
            {
                return BadRequest();
            }
            if (_context.Regiones.Any(c => c.Nombre == region.Nombre && c.RegionId != region.RegionId))
            {
                return CreatedAtAction("GetRegions", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(region).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionExists(id))
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

        // POST: api/Regions
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostRegion([FromBody] Region region)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Regiones.Any(c => c.Nombre == region.Nombre ))
            {
                return CreatedAtAction("GetRegions", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Regiones.Add(region);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegion", new { id = region.RegionId }, region);
        }

        // DELETE: api/Regions/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRegion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var region = await _context.Regiones.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            _context.Regiones.Remove(region);
            await _context.SaveChangesAsync();

            return Ok(region);
        }

        private bool RegionExists(int id)
        {
            return _context.Regiones.Any(e => e.RegionId == id);
        }
    }
}