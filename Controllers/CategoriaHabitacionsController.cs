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
    public class CategoriaHabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public CategoriaHabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaHabitacions
        [HttpGet]
        public IEnumerable<CategoriaHabitacion> GetCategoriaHabitacion(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<CategoriaHabitacion> lista;
            if (col == "-1")
            {
                return _context.CategoriaHabitacion.OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.CategoriaHabitacion.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.CategoriaHabitacion.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/CategoriaHabitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetCategoriaHabitacionsCount()
        {
            return _context.CategoriaHabitacion.Count();
        }

        // GET: api/CategoriaHabitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaHabitacion = await _context.CategoriaHabitacion.FindAsync(id);

            if (categoriaHabitacion == null)
            {
                return NotFound();
            }

            return Ok(categoriaHabitacion);
        }

        // PUT: api/CategoriaHabitacions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaHabitacion([FromRoute] int id, [FromBody] CategoriaHabitacion categoriaHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaHabitacion.CategoriaHabitacionId)
            {
                return BadRequest();
            }
            if (_context.CategoriaHabitacion.Any(c => c.Nombre == categoriaHabitacion.Nombre && categoriaHabitacion.CategoriaHabitacionId != id))
            {
                return CreatedAtAction("GetCategoriaHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(categoriaHabitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaHabitacionExists(id))
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

        // POST: api/CategoriaHabitacions
        [HttpPost]
        public async Task<IActionResult> PostCategoriaHabitacion([FromBody] CategoriaHabitacion categoriaHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.CategoriaHabitacion.Any(c => c.Nombre == categoriaHabitacion.Nombre))
            {
                return CreatedAtAction("GetCategoriaHabitacion", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.CategoriaHabitacion.Add(categoriaHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaHabitacion", new { id = categoriaHabitacion.CategoriaHabitacionId }, categoriaHabitacion);
        }

        // DELETE: api/CategoriaHabitacions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaHabitacion = await _context.CategoriaHabitacion.FindAsync(id);
            if (categoriaHabitacion == null)
            {
                return NotFound();
            }

            _context.CategoriaHabitacion.Remove(categoriaHabitacion);
            await _context.SaveChangesAsync();

            return Ok(categoriaHabitacion);
        }

        private bool CategoriaHabitacionExists(int id)
        {
            return _context.CategoriaHabitacion.Any(e => e.CategoriaHabitacionId == id);
        }
    }
}