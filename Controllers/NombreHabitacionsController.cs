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
    public class NombreHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public NombreHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/NombreHabitacions
        [HttpGet]
        public IEnumerable<NombreHabitacion> GetNombreHabitacion(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<NombreHabitacion> lista;
            if (col == "-1")
            {
                return _context.NombreHabitacion.OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.NombreHabitacion.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.NombreHabitacion.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/NombreHabitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetNombreHabitacionsCount()
        {
            return _context.NombreHabitacion.Count();
        }

        // GET: api/NombreHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNombreHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nombreHabitacion = await _context.NombreHabitacion.FindAsync(id);

            if (nombreHabitacion == null)
            {
                return NotFound();
            }

            return Ok(nombreHabitacion);
        }

        // PUT: api/NombreHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNombreHabitacion([FromRoute] int id, [FromBody] NombreHabitacion nombreHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nombreHabitacion.NombreHabitacionId)
            {
                return BadRequest();
            }
            if (_context.NombreTemporadas.Any(c => c.Nombre == nombreHabitacion.Nombre && nombreHabitacion.NombreHabitacionId != id))
            {
                return CreatedAtAction("GetNombreHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(nombreHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NombreHabitacionExists(id))
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

        // POST: api/NombreHabitacions
        [HttpPost]
        public async Task<IActionResult> PostNombreHabitacion([FromBody] NombreHabitacion nombreHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.NombreTemporadas.Any(c => c.Nombre == nombreHabitacion.Nombre ))
            {
                return CreatedAtAction("GetNombreHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.NombreHabitacion.Add(nombreHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNombreHabitacion", new { id = nombreHabitacion.NombreHabitacionId }, nombreHabitacion);
        }

        // DELETE: api/NombreHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNombreHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nombreHabitacion = await _context.NombreHabitacion.FindAsync(id);
            if (nombreHabitacion == null)
            {
                return NotFound();
            }

            _context.NombreHabitacion.Remove(nombreHabitacion);
            await _context.SaveChangesAsync();

            return Ok(nombreHabitacion);
        }

        private bool NombreHabitacionExists(int id)
        {
            return _context.NombreHabitacion.Any(e => e.NombreHabitacionId == id);
        }
    }
}