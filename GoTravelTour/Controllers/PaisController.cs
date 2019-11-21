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
    public class PaisController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PaisController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Pais
        [HttpGet]
        public IEnumerable<Pais> GetPaises(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<Pais> lista;
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Paises.Where(p => (p.NombreLargo.ToLower().Contains(filter.ToLower()) || p.NombreCorto.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Paises.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("NombreLargo".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreLargo);

                        }else if ("NombreCorto".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreCorto);

                        }


                        break;
                    }

                default:
                    {
                        if ("NombreLargo".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreLargo);

                        }
                        else if ("NombreCorto".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreCorto);

                        }

                    }

                    break;
            }

            
                return lista;
        }

        // GET: api/Pais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPais([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pais = await _context.Paises.FindAsync(id);

            if (pais == null)
            {
                return NotFound();
            }

            return Ok(pais);
        }
        // GET: api/Pais/Count
        [Route("Count")]
        [HttpGet]
        public int GetPaisCount()
        {
            return _context.Paises.Count();
        }

        // PUT: api/Pais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPais([FromRoute] int id, [FromBody] Pais pais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pais.PaisId)
            {
                return BadRequest();
            }

            _context.Entry(pais).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaisExists(id))
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

        // POST: api/Pais
        [HttpPost]
        public async Task<IActionResult> PostPais([FromBody] Pais pais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Paises.Add(pais);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPais", new { id = pais.PaisId }, pais);
        }

        // DELETE: api/Pais/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePais([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pais = await _context.Paises.FindAsync(id);
            if (pais == null)
            {
                return NotFound();
            }

            _context.Paises.Remove(pais);
            await _context.SaveChangesAsync();

            return Ok(pais);
        }

        private bool PaisExists(int id)
        {
            return _context.Paises.Any(e => e.PaisId == id);
        }
    }
}