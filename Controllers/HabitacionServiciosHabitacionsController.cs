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
    public class HabitacionServiciosHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public HabitacionServiciosHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/HabitacionServiciosHabitacions
        [HttpGet]
        public IEnumerable<HabitacionServiciosHabitacion> GetHabitacionServiciosHabitacion(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<HabitacionServiciosHabitacion> lista;
            if (col == "-1")
            {
                return _context.HabitacionServiciosHabitacion
                    .Include(pd => pd.Habitacion)
                    .Include(dp => dp.ServiciosHabitacion)
                    .OrderBy(a => a.Habitacion.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.HabitacionServiciosHabitacion
                    .Include(pd => pd.Habitacion)
                    .Include(dp => dp.ServiciosHabitacion)
                    .Where(p => (p.Habitacion.Nombre.ToLower().Contains(filter.ToLower()))
                    || p.Habitacion.Nombre.ToLower().Contains(filter.ToLower()))
                    .ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.HabitacionServiciosHabitacion
                    .Include(pd => pd.Habitacion)
                    .Include(dp => dp.ServiciosHabitacion)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Habitacion.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Habitacion.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/HabitacionServiciosHabitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetHabitacionServiciosHabitacionsCount()
        {
            return _context.HabitacionServiciosHabitacion.Count();
        }

        // GET: api/HabitacionServiciosHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHabitacionServiciosHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacionServiciosHabitacion = await _context.HabitacionServiciosHabitacion.FindAsync(id);

            if (habitacionServiciosHabitacion == null)
            {
                return NotFound();
            }

            return Ok(habitacionServiciosHabitacion);
        }

        // PUT: api/HabitacionServiciosHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHabitacionServiciosHabitacion([FromRoute] int id, [FromBody] HabitacionServiciosHabitacion habitacionServiciosHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != habitacionServiciosHabitacion.HabitacionServiciosHabitacionId)
            {
                return BadRequest();
            }

            _context.Entry(habitacionServiciosHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HabitacionServiciosHabitacionExists(id))
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

        // POST: api/HabitacionServiciosHabitacions
        [HttpPost]
        public async Task<IActionResult> PostHabitacionServiciosHabitacion([FromBody] HabitacionServiciosHabitacion habitacionServiciosHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.HabitacionServiciosHabitacion.Add(habitacionServiciosHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHabitacionServiciosHabitacion", new { id = habitacionServiciosHabitacion.HabitacionServiciosHabitacionId }, habitacionServiciosHabitacion);
        }

        // DELETE: api/HabitacionServiciosHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabitacionServiciosHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacionServiciosHabitacion = await _context.HabitacionServiciosHabitacion.FindAsync(id);
            if (habitacionServiciosHabitacion == null)
            {
                return NotFound();
            }

            _context.HabitacionServiciosHabitacion.Remove(habitacionServiciosHabitacion);
            await _context.SaveChangesAsync();

            return Ok(habitacionServiciosHabitacion);
        }

        private bool HabitacionServiciosHabitacionExists(int id)
        {
            return _context.HabitacionServiciosHabitacion.Any(e => e.HabitacionServiciosHabitacionId == id);
        }
    }
}