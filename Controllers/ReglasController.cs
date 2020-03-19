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
    public class ReglasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ReglasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Reglas
        [HttpGet]
        public IEnumerable<Reglas> GetReglas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<Reglas> lista;
            if (col == "-1")
            {
                return _context.Reglas
                    .Include(x => x.Modificador)
                    .Include(x => x.TipoHabitacion)

                    .OrderBy(a => a.TipoPersona).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Reglas
                    .Include(x => x.Modificador)
                    .Include(x => x.TipoHabitacion)
                    .Where(p => (p.TipoPersona.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Reglas
                    .Include(x => x.Modificador)
                    .Include(x => x.TipoHabitacion).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.TipoPersona);

                        }



                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.TipoPersona);

                        }


                    }

                    break;
            }
            return lista;
        }

        // GET: api/Reglas/Count
        [Route("Count")]
        [HttpGet]
        public int GetReglasCount()
        {
            return _context.Reglas.Count();
        }

        // GET: api/Reglas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReglas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reglas = await _context.Reglas.FindAsync(id);

            if (reglas == null)
            {
                return NotFound();
            }

            return Ok(reglas);
        }

        // PUT: api/Reglas/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutReglas([FromRoute] int id, [FromBody] Reglas reglas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reglas.ReglasId)
            {
                return BadRequest();
            }

            _context.Entry(reglas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReglasExists(id))
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

        // POST: api/Reglas
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReglas([FromBody] Reglas reglas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Reglas.Add(reglas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReglas", new { id = reglas.ReglasId }, reglas);
        }

        // DELETE: api/Reglas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReglas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reglas = await _context.Reglas.FindAsync(id);
            if (reglas == null)
            {
                return NotFound();
            }

            _context.Reglas.Remove(reglas);
            await _context.SaveChangesAsync();

            return Ok(reglas);
        }

        private bool ReglasExists(int id)
        {
            return _context.Reglas.Any(e => e.ReglasId == id);
        }
    }
}