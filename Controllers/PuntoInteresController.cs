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
    public class PuntoInteresController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PuntoInteresController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PuntoInteres
        [HttpGet]
        public IEnumerable<PuntoInteres> GetPuntosInteres(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PuntoInteres> lista;
            if (col == "-1")
            {
                return _context.PuntosInteres
                    .Include(r => r.Region)
                    .OrderBy(a=>a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PuntosInteres.Include(r => r.Region)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.PuntosInteres.Include(r => r.Region)
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

        
        // GET: api/PuntoInteres/Count
        [Route("Count")]
        [HttpGet]
        public int GetPuntoInteresCount()
        {
            return _context.PuntosInteres.Count();
        }

        // GET: api/PuntoInteres/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPuntoInteres([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var puntoInteres = await _context.PuntosInteres.FindAsync(id);


            if (puntoInteres == null)
            {
                return NotFound();
            }
            puntoInteres.Region = _context.Regiones.Find(puntoInteres.RegionId);

            return Ok(puntoInteres);
        }

        // PUT: api/PuntoInteres/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPuntoInteres([FromRoute] int id, [FromBody] PuntoInteres puntoInteres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != puntoInteres.PuntoInteresId)
            {
                return BadRequest();
            }
            if (_context.PuntosInteres.Any(c => c.Nombre == puntoInteres.Nombre && c.RegionId != puntoInteres.RegionId))
            {
                return CreatedAtAction("GetPuntosInteres", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(puntoInteres).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PuntoInteresExists(id))
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

        // POST: api/PuntoInteres
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostPuntoInteres([FromBody] PuntoInteres puntoInteres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (_context.PuntosInteres.Any(pi => pi.Nombre == puntoInteres.Nombre))
            {
                return CreatedAtAction("GetPuntoInteres", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.PuntosInteres.Add(puntoInteres);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPuntoInteres", new { id = puntoInteres.PuntoInteresId }, puntoInteres);
        }

        // DELETE: api/PuntoInteres/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePuntoInteres([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var puntoInteres = await _context.PuntosInteres.FindAsync(id);
            if (puntoInteres == null)
            {
                return NotFound();
            }

            _context.PuntosInteres.Remove(puntoInteres);
            await _context.SaveChangesAsync();

            return Ok(puntoInteres);
        }

        private bool PuntoInteresExists(int id)
        {
            return _context.PuntosInteres.Any(e => e.PuntoInteresId == id);
        }

        // GET: api/Servicios/PuntosByRegion/5 obtiene dada una region sus puntos de interes
        [Route("PuntosByRegion/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPuntosByRegion([FromRoute] int id)
        {
            List<PuntoInteres> lista = new List<PuntoInteres>();
            lista = _context.PuntosInteres.Where(x => x.RegionId == id).ToList();


            if (lista == null)
            {
                return NotFound();
            }

            return Ok(lista);
        }
    }
}