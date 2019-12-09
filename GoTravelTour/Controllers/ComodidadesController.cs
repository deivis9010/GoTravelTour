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
    public class ComodidadesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ComodidadesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Comodidades
        [HttpGet]
        public IEnumerable<Comodidades> GetComodidades(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Comodidades> lista;
            if (col == "-1")
            {
                return _context.Comodidades.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Comodidades.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Comodidades.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/Comodidades/Count
        [Route("Count")]
        [HttpGet]
        public int GetComodidadesCount()
        {
            return _context.Comodidades.Count();
        }

        // GET: api/Comodidades/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComodidades([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comodidades = await _context.Comodidades.FindAsync(id);

            if (comodidades == null)
            {
                return NotFound();
            }

            return Ok(comodidades);
        }

        // PUT: api/Comodidades/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComodidades([FromRoute] int id, [FromBody] Comodidades comodidades)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != comodidades.ComodidadesId)
            {
                return BadRequest();
            }
            List<Comodidades> crol = _context.Comodidades.Where(c => c.Nombre == comodidades.Nombre && comodidades.ComodidadesId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetComodidades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(comodidades).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComodidadesExists(id))
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

        // POST: api/Comodidades
        [HttpPost]
        public async Task<IActionResult> PostComodidades([FromBody] Comodidades comodidades)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<Comodidades> crol = _context.Comodidades.Where(c => c.Nombre == comodidades.Nombre).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetComodidades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Comodidades.Add(comodidades);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComodidades", new { id = comodidades.ComodidadesId }, comodidades);
        }

        // DELETE: api/Comodidades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComodidades([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comodidades = await _context.Comodidades.FindAsync(id);
            if (comodidades == null)
            {
                return NotFound();
            }

            _context.Comodidades.Remove(comodidades);
            await _context.SaveChangesAsync();

            return Ok(comodidades);
        }

        private bool ComodidadesExists(int id)
        {
            return _context.Comodidades.Any(e => e.ComodidadesId == id);
        }
    }
}