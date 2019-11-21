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
    public class PlanesAlimenticiosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PlanesAlimenticiosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PlanesAlimenticios
        [HttpGet]
        public IEnumerable<PlanesAlimenticios> GetPlanesAlimenticios(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<PlanesAlimenticios> lista;
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PlanesAlimenticios.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.PlanesAlimenticios.ToPagedList(pageIndex, pageSize).ToList();
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

        // GET: api/PlanesAlimenticios/Count
        [Route("Count")]
        [HttpGet]
        public int GetPlanesAlimenticiosCount()
        {
            return _context.PlanesAlimenticios.Count();
        }


        // GET: api/PlanesAlimenticios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var planesAlimenticios = await _context.PlanesAlimenticios.FindAsync(id);

            if (planesAlimenticios == null)
            {
                return NotFound();
            }

            return Ok(planesAlimenticios);
        }

        // PUT: api/PlanesAlimenticios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlanesAlimenticios([FromRoute] int id, [FromBody] PlanesAlimenticios planesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != planesAlimenticios.PlanesAlimenticiosId)
            {
                return BadRequest();
            }

            _context.Entry(planesAlimenticios).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanesAlimenticiosExists(id))
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

        // POST: api/PlanesAlimenticios
        [HttpPost]
        public async Task<IActionResult> PostPlanesAlimenticios([FromBody] PlanesAlimenticios planesAlimenticios)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PlanesAlimenticios.Add(planesAlimenticios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlanesAlimenticios", new { id = planesAlimenticios.PlanesAlimenticiosId }, planesAlimenticios);
        }

        // DELETE: api/PlanesAlimenticios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanesAlimenticios([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var planesAlimenticios = await _context.PlanesAlimenticios.FindAsync(id);
            if (planesAlimenticios == null)
            {
                return NotFound();
            }

            _context.PlanesAlimenticios.Remove(planesAlimenticios);
            await _context.SaveChangesAsync();

            return Ok(planesAlimenticios);
        }

        private bool PlanesAlimenticiosExists(int id)
        {
            return _context.PlanesAlimenticios.Any(e => e.PlanesAlimenticiosId == id);
        }
    }
}