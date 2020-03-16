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
    public class PosibleCombinacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PosibleCombinacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PosibleCombinacions
        [HttpGet]
        public IEnumerable<PosibleCombinacion> GetPosibleCombinaciones(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PosibleCombinacion> lista;
            if (col == "-1")
            {
                lista = _context.PosibleCombinaciones                   
                    .Include(a => a.TipoHabitacion)
                    .OrderBy(a => a.Nombre)
                    .ToList();



                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PosibleCombinaciones
                    .Include(a => a.TipoHabitacion)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.PosibleCombinaciones
                    .Include(a => a.TipoHabitacion)
                    .OrderBy(a => a.Nombre)
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
        // GET: api/PosibleCombinacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetPosibleCombinacionsCount()
        {
            return _context.PosibleCombinaciones.Count();
        }

        // GET: api/PosibleCombinacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPosibleCombinacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posibleCombinacion = await _context.PosibleCombinaciones.FindAsync(id);

            if (posibleCombinacion == null)
            {
                return NotFound();
            }

            return Ok(posibleCombinacion);
        }

        // PUT: api/PosibleCombinacions/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPosibleCombinacion([FromRoute] int id, [FromBody] PosibleCombinacion posibleCombinacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != posibleCombinacion.PosibleCombinacionId)
            {
                return BadRequest();
            }
            if (_context.PosibleCombinaciones.Any(c => c.CantAdult == posibleCombinacion.CantAdult && c.CantNino == posibleCombinacion.CantNino && c.CantInfantes == posibleCombinacion.CantInfantes && id != posibleCombinacion.PosibleCombinacionId))
            {
                return CreatedAtAction("GetPosibleCombinacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(posibleCombinacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PosibleCombinacionExists(id))
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

        // POST: api/PosibleCombinacions
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostPosibleCombinacion([FromBody] PosibleCombinacion posibleCombinacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            posibleCombinacion.TipoHabitacion = _context.TipoHabitaciones.First(x => x.TipoHabitacionId == posibleCombinacion.TipoHabitacionId);

            if (_context.PosibleCombinaciones.Any(c => c.CantAdult == posibleCombinacion.CantAdult && c.CantNino == posibleCombinacion.CantNino && c.CantInfantes == posibleCombinacion.CantInfantes ))
            {
                return CreatedAtAction("GetPosibleCombinacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.PosibleCombinaciones.Add(posibleCombinacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPosibleCombinacion", new { id = posibleCombinacion.PosibleCombinacionId }, posibleCombinacion);
        }

        // DELETE: api/PosibleCombinacions/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePosibleCombinacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posibleCombinacion = await _context.PosibleCombinaciones.FindAsync(id);
            if (posibleCombinacion == null)
            {
                return NotFound();
            }

            _context.PosibleCombinaciones.Remove(posibleCombinacion);
            await _context.SaveChangesAsync();

            return Ok(posibleCombinacion);
        }

        private bool PosibleCombinacionExists(int id)
        {
            return _context.PosibleCombinaciones.Any(e => e.PosibleCombinacionId == id);
        }

        // GET: api/PosibleCombinacions/Tipo/5
        [HttpGet("{idTipoHabitacion}")]
        [Route("Tipo/{idTipoHabitacion}")]
        public async Task<IActionResult> GetSPosibleCombincacionByTipo([FromRoute] int idTipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<PosibleCombinacion> combinaciones = _context.PosibleCombinaciones.Include(x => x.TipoHabitacion).Where(a => a.TipoHabitacionId == idTipoHabitacion).ToList();

            if (combinaciones == null)
            {
                return NotFound();
            }

            return Ok(combinaciones);
        }

    }
}