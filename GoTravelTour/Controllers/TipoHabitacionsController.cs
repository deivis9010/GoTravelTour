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
    public class TipoHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoHabitacions
        [HttpGet]
        public IEnumerable<TipoHabitacion> GetTipoHabitaciones(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoHabitacion> lista;
            if (col == "-1")
            {
                return _context.TipoHabitaciones.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoHabitaciones.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoHabitaciones.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/TipoHabitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoHabitacionsCount()
        {
            return _context.TipoHabitaciones.Count();
        }

        // GET: api/TipoHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);

            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            return Ok(tipoHabitacion);
        }

        // PUT: api/TipoHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoHabitacion([FromRoute] int id, [FromBody] TipoHabitacion tipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoHabitacion.TipoHabitacionId)
            {
                return BadRequest();
            }
            List<TipoHabitacion> crol = _context.TipoHabitaciones.Where(c => c.Nombre == tipoHabitacion.Nombre && tipoHabitacion.TipoHabitacionId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetTipoHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(tipoHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoHabitacionExists(id))
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

        // POST: api/TipoHabitacions
        [HttpPost]
        public async Task<IActionResult> PostTipoHabitacion([FromBody] TipoHabitacion tipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<TipoHabitacion> crol = _context.TipoHabitaciones.Where(c => c.Nombre == tipoHabitacion.Nombre ).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetTipoHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.TipoHabitaciones.Add(tipoHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoHabitacion", new { id = tipoHabitacion.TipoHabitacionId }, tipoHabitacion);
        }

        // DELETE: api/TipoHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            _context.TipoHabitaciones.Remove(tipoHabitacion);
            await _context.SaveChangesAsync();

            return Ok(tipoHabitacion);
        }

        private bool TipoHabitacionExists(int id)
        {
            return _context.TipoHabitaciones.Any(e => e.TipoHabitacionId == id);
        }
    }
}