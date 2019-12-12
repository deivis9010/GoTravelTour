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
    public class NombreTemporadasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public NombreTemporadasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/NombreTemporadas
        [HttpGet]
        public IEnumerable<NombreTemporada> GetNombreTemporadas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<NombreTemporada> lista;
            if (col == "-1")
            {
                return _context.NombreTemporadas.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.NombreTemporadas.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.NombreTemporadas.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/NombreTemporadas/Count
        [Route("Count")]
        [HttpGet]
        public int GetNombreTemporadasCount()
        {
            return _context.NombreTemporadas.Count();
        }

        // GET: api/NombreTemporadas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNombreTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nombreTemporada = await _context.NombreTemporadas.FindAsync(id);

            if (nombreTemporada == null)
            {
                return NotFound();
            }

            return Ok(nombreTemporada);
        }

        // PUT: api/NombreTemporadas/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutNombreTemporada([FromRoute] int id, [FromBody] NombreTemporada nombreTemporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nombreTemporada.NombreTemporadaId)
            {
                return BadRequest();
            }
            List<NombreTemporada> crol = _context.NombreTemporadas.Where(c => c.Nombre == nombreTemporada.Nombre && nombreTemporada.NombreTemporadaId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetNombreTemporada", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(nombreTemporada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NombreTemporadaExists(id))
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

        // POST: api/NombreTemporadas
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostNombreTemporada([FromBody] NombreTemporada nombreTemporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<NombreTemporada> crol = _context.NombreTemporadas.Where(c => c.Nombre == nombreTemporada.Nombre).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetNombreTemporada", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.NombreTemporadas.Add(nombreTemporada);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNombreTemporada", new { id = nombreTemporada.NombreTemporadaId }, nombreTemporada);
        }

        // DELETE: api/NombreTemporadas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNombreTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nombreTemporada = await _context.NombreTemporadas.FindAsync(id);
            if (nombreTemporada == null)
            {
                return NotFound();
            }

            _context.NombreTemporadas.Remove(nombreTemporada);
            await _context.SaveChangesAsync();

            return Ok(nombreTemporada);
        }

        private bool NombreTemporadaExists(int id)
        {
            return _context.NombreTemporadas.Any(e => e.NombreTemporadaId == id);
        }
    }
}