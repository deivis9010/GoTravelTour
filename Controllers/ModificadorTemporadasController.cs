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
    public class ModificadorTemporadasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ModificadorTemporadasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ModificadorTemporadas
        [HttpGet]
        public IEnumerable<ModificadorTemporada> GetModificadorTemporada(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<ModificadorTemporada> lista;
            if (col == "-1")
            {
                return _context.ModificadorTemporada
                    .Include(x=>x.Modificador)
                    .Include(x=>x.Temporada)
                    
                    .ToList();
    }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ModificadorTemporada
                    .Include(x => x.Modificador)
                    .Include(x => x.Temporada)
                   
                    .Where(p => (p.Modificador.IdentificadorModificador.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
}
            else
            {
                lista = _context.ModificadorTemporada.Include(x => x.Modificador)
                    .Include(x => x.Temporada)
                    .OrderBy(a => a.Modificador.IdentificadorModificador).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Modificador.IdentificadorModificador);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Modificador.IdentificadorModificador);

                        }

                        break;

                    }


            }

            return lista;

        }
        // GET: api/ModificadorTemporadas/Count
        [Route("Count")]
        [HttpGet]
        public int GetModificadorTemporadasCount()
        {
            return _context.ModificadorTemporada.Count();
        }

        // GET: api/ModificadorTemporadas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModificadorTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificadorTemporada = await _context.ModificadorTemporada.FindAsync(id);

            if (modificadorTemporada == null)
            {
                return NotFound();
            }

            return Ok(modificadorTemporada);
        }

        // PUT: api/ModificadorTemporadas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModificadorTemporada([FromRoute] int id, [FromBody] ModificadorTemporada modificadorTemporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modificadorTemporada.ModificadorTemporadaId)
            {
                return BadRequest();
            }

            _context.Entry(modificadorTemporada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModificadorTemporadaExists(id))
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

        // POST: api/ModificadorTemporadas
        [HttpPost]
        public async Task<IActionResult> PostModificadorTemporada([FromBody] ModificadorTemporada modificadorTemporada)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ModificadorTemporada.Add(modificadorTemporada);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModificadorTemporada", new { id = modificadorTemporada.ModificadorTemporadaId }, modificadorTemporada);
        }

        // DELETE: api/ModificadorTemporadas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModificadorTemporada([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificadorTemporada = await _context.ModificadorTemporada.FindAsync(id);
            if (modificadorTemporada == null)
            {
                return NotFound();
            }

            _context.ModificadorTemporada.Remove(modificadorTemporada);
            await _context.SaveChangesAsync();

            return Ok(modificadorTemporada);
        }

        private bool ModificadorTemporadaExists(int id)
        {
            return _context.ModificadorTemporada.Any(e => e.ModificadorTemporadaId == id);
        }
    }
}