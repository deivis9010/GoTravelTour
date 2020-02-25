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
    public class ServiciosHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ServiciosHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ServiciosHabitacions
        [HttpGet]
        public IEnumerable<ServiciosHabitacion> GetServiciosHabitacion(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<ServiciosHabitacion> lista;
            if (col == "-1")
            {
                lista = _context.ServiciosHabitacion                    
                   
                    .OrderBy(a => a.Nombre)

                    .ToList();



                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ServiciosHabitacion

                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.ServiciosHabitacion

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
        // GET: api/ServiciosHabitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetServiciosHabitacionsCount()
        {
            return _context.ServiciosHabitacion.Count();
        }

        // GET: api/ServiciosHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiciosHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var serviciosHabitacion = await _context.ServiciosHabitacion.FindAsync(id);

            if (serviciosHabitacion == null)
            {
                return NotFound();
            }

            return Ok(serviciosHabitacion);
        }

        // PUT: api/ServiciosHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiciosHabitacion([FromRoute] int id, [FromBody] ServiciosHabitacion serviciosHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != serviciosHabitacion.ServiciosHabitacionId)
            {
                return BadRequest();
            }

            _context.Entry(serviciosHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiciosHabitacionExists(id))
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

        // POST: api/ServiciosHabitacions
        [HttpPost]
        public async Task<IActionResult> PostServiciosHabitacion([FromBody] ServiciosHabitacion serviciosHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ServiciosHabitacion.Add(serviciosHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiciosHabitacion", new { id = serviciosHabitacion.ServiciosHabitacionId }, serviciosHabitacion);
        }

        // DELETE: api/ServiciosHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiciosHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var serviciosHabitacion = await _context.ServiciosHabitacion.FindAsync(id);
            if (serviciosHabitacion == null)
            {
                return NotFound();
            }

            _context.ServiciosHabitacion.Remove(serviciosHabitacion);
            await _context.SaveChangesAsync();

            return Ok(serviciosHabitacion);
        }

        private bool ServiciosHabitacionExists(int id)
        {
            return _context.ServiciosHabitacion.Any(e => e.ServiciosHabitacionId == id);
        }
    }
}