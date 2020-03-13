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
    public class AlojamientosPlanesAlimenticiosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public AlojamientosPlanesAlimenticiosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/AlojamientosPlanesAlimenticios
        [HttpGet]
        public IEnumerable<AlojamientosPlanesAlimenticios> GetAlojamientosPlanesAlimenticios(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<AlojamientosPlanesAlimenticios> lista;
            if (col == "-1")
            {
                return _context.AlojamientosPlanesAlimenticios
                    .Include(x => x.Producto)
                    .Include(x => x.PlanesAlimenticios)
                    .OrderBy(a => a.PlanesAlimenticios.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.AlojamientosPlanesAlimenticios.Include(x => x.PlanesAlimenticios).Include(x => x.Producto)
                    .Where(p => (p.PlanesAlimenticios.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.AlojamientosPlanesAlimenticios.Include(x => x.PlanesAlimenticios)
                    .Include(x => x.Producto).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.PlanesAlimenticios.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.PlanesAlimenticios.Nombre);

                        }

                        break;

                    }


            }

            return lista;

        }
        // GET: api/AlojamientosPlanesAlimenticios/Count
        [Route("Count")]
        [HttpGet]
        public int GetAlojamientosPlanesAlimenticiosCount()
        {
            return _context.AlojamientosPlanesAlimenticios.Count();
        }

        // GET: api/AlojamientosPlanesAlimenticios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlojamientosPlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alojamientosPlanesAlimenticios = await _context.AlojamientosPlanesAlimenticios.FindAsync(id);

            if (alojamientosPlanesAlimenticios == null)
            {
                return NotFound();
            }

            return Ok(alojamientosPlanesAlimenticios);
        }

        // PUT: api/AlojamientosPlanesAlimenticios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlojamientosPlanesAlimenticios([FromRoute] int id, [FromBody] AlojamientosPlanesAlimenticios alojamientosPlanesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alojamientosPlanesAlimenticios.AlojamientosPlanesAlimenticiosId)
            {
                return BadRequest();
            }

            _context.Entry(alojamientosPlanesAlimenticios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlojamientosPlanesAlimenticiosExists(id))
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

        // POST: api/AlojamientosPlanesAlimenticios
        [HttpPost]
        public async Task<IActionResult> PostAlojamientosPlanesAlimenticios([FromBody] AlojamientosPlanesAlimenticios alojamientosPlanesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AlojamientosPlanesAlimenticios.Add(alojamientosPlanesAlimenticios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlojamientosPlanesAlimenticios", new { id = alojamientosPlanesAlimenticios.AlojamientosPlanesAlimenticiosId }, alojamientosPlanesAlimenticios);
        }

        // DELETE: api/AlojamientosPlanesAlimenticios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlojamientosPlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alojamientosPlanesAlimenticios = await _context.AlojamientosPlanesAlimenticios.FindAsync(id);
            if (alojamientosPlanesAlimenticios == null)
            {
                return NotFound();
            }

            _context.AlojamientosPlanesAlimenticios.Remove(alojamientosPlanesAlimenticios);
            await _context.SaveChangesAsync();

            return Ok(alojamientosPlanesAlimenticios);
        }

        private bool AlojamientosPlanesAlimenticiosExists(int id)
        {
            return _context.AlojamientosPlanesAlimenticios.Any(e => e.AlojamientosPlanesAlimenticiosId == id);
        }
    }
}