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
    public class PrecioPlanesAlimenticiosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioPlanesAlimenticiosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioPlanesAlimenticios
        [HttpGet]
        public IEnumerable<PrecioPlanesAlimenticios> GetPrecioPlanesAlimenticios(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<PrecioPlanesAlimenticios> lista;
            if (col == "-1")
            {
                return _context.PrecioPlanesAlimenticios
                    .Include(x=>x.Hotel)
                    .Include(x => x.Temporada)

                    .OrderBy(a => a.Hotel.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioPlanesAlimenticios
                    .Include(x => x.Hotel)
                    .Include(x => x.Temporada)
                    .Where(p => (p.Hotel.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.PrecioPlanesAlimenticios
                    .Include(x => x.Hotel)
                    .Include(x => x.Temporada).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Hotel.Nombre);

                        }



                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Hotel.Nombre);

                        }


                    }

                    break;
            }
            return lista;
        }

        // GET: api/PrecioPlanesAlimenticios/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioPlanesAlimenticiosCount()
        {
            return _context.PlanesAlimenticios.Count();
        }

        // GET: api/PrecioPlanesAlimenticios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioPlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioPlanesAlimenticios = await _context.PrecioPlanesAlimenticios.FindAsync(id);

            if (precioPlanesAlimenticios == null)
            {
                return NotFound();
            }

            return Ok(precioPlanesAlimenticios);
        }

        // PUT: api/PrecioPlanesAlimenticios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioPlanesAlimenticios([FromRoute] int id, [FromBody] PrecioPlanesAlimenticios precioPlanesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioPlanesAlimenticios.PrecioPlanesAlimenticiosId)
            {
                return BadRequest();
            }

            _context.Entry(precioPlanesAlimenticios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioPlanesAlimenticiosExists(id))
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

        // POST: api/PrecioPlanesAlimenticios
        [HttpPost]
        public async Task<IActionResult> PostPrecioPlanesAlimenticios([FromBody] PrecioPlanesAlimenticios precioPlanesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PrecioPlanesAlimenticios.Add(precioPlanesAlimenticios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioPlanesAlimenticios", new { id = precioPlanesAlimenticios.PrecioPlanesAlimenticiosId }, precioPlanesAlimenticios);
        }

        // DELETE: api/PrecioPlanesAlimenticios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioPlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioPlanesAlimenticios = await _context.PrecioPlanesAlimenticios.FindAsync(id);
            if (precioPlanesAlimenticios == null)
            {
                return NotFound();
            }

            _context.PrecioPlanesAlimenticios.Remove(precioPlanesAlimenticios);
            await _context.SaveChangesAsync();

            return Ok(precioPlanesAlimenticios);
        }

        private bool PrecioPlanesAlimenticiosExists(int id)
        {
            return _context.PrecioPlanesAlimenticios.Any(e => e.PrecioPlanesAlimenticiosId == id);
        }
    }
}