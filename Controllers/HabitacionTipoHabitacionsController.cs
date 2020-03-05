using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionTipoHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public HabitacionTipoHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/HabitacionTipoHabitacions
        [HttpGet]
        public IEnumerable<HabitacionTipoHabitacion> GetHabitacionTipoHabitacion()
        {
            return _context.HabitacionTipoHabitacion;
        }

        // GET: api/HabitacionTipoHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHabitacionTipoHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacionTipoHabitacion = await _context.HabitacionTipoHabitacion.FindAsync(id);

            if (habitacionTipoHabitacion == null)
            {
                return NotFound();
            }

            return Ok(habitacionTipoHabitacion);
        }

        // PUT: api/HabitacionTipoHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHabitacionTipoHabitacion([FromRoute] int id, [FromBody] HabitacionTipoHabitacion habitacionTipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != habitacionTipoHabitacion.HabitacionTipoHabitacionId)
            {
                return BadRequest();
            }

            _context.Entry(habitacionTipoHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HabitacionTipoHabitacionExists(id))
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

        // POST: api/HabitacionTipoHabitacions
        [HttpPost]
        public async Task<IActionResult> PostHabitacionTipoHabitacion([FromBody] HabitacionTipoHabitacion habitacionTipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.HabitacionTipoHabitacion.Add(habitacionTipoHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHabitacionTipoHabitacion", new { id = habitacionTipoHabitacion.HabitacionTipoHabitacionId }, habitacionTipoHabitacion);
        }

        // DELETE: api/HabitacionTipoHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabitacionTipoHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacionTipoHabitacion = await _context.HabitacionTipoHabitacion.FindAsync(id);
            if (habitacionTipoHabitacion == null)
            {
                return NotFound();
            }

            _context.HabitacionTipoHabitacion.Remove(habitacionTipoHabitacion);
            await _context.SaveChangesAsync();

            return Ok(habitacionTipoHabitacion);
        }

        private bool HabitacionTipoHabitacionExists(int id)
        {
            return _context.HabitacionTipoHabitacion.Any(e => e.HabitacionTipoHabitacionId == id);
        }
    }
}