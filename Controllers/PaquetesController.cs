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
    public class PaquetesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PaquetesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Paquetes
        [HttpGet]
        public IEnumerable<Paquete> GetPaquete(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Paquete> lista;
            if (col == "-1")
            {
                return _context.Paquete                    
                    .OrderBy(a => a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Paquete
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Paquete.ToPagedList(pageIndex, pageSize).ToList();
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

                        break;

                    }


            }

            return lista;

        }
        // GET: api/Paquetes/Count
        [Route("Count")]
        [HttpGet]
        public int GetPaquetesCount()
        {
            return _context.Paquete.Count();
        }

        // GET: api/Paquetes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaquete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paquete = await _context.Paquete.FindAsync(id);

            if (paquete == null)
            {
                return NotFound();
            }

            return Ok(paquete);
        }

        // PUT: api/Paquetes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaquete([FromRoute] int id, [FromBody] Paquete paquete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paquete.PaqueteId)
            {
                return BadRequest();
            }

            _context.Entry(paquete).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaqueteExists(id))
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

        // POST: api/Paquetes
        [HttpPost]
        public async Task<IActionResult> PostPaquete([FromBody] Paquete paquete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Paquete.Add(paquete);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPaquete", new { id = paquete.PaqueteId }, paquete);
        }

        // DELETE: api/Paquetes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaquete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paquete = await _context.Paquete.FindAsync(id);
            if (paquete == null)
            {
                return NotFound();
            }

            _context.Paquete.Remove(paquete);
            await _context.SaveChangesAsync();

            return Ok(paquete);
        }

        private bool PaqueteExists(int id)
        {
            return _context.Paquete.Any(e => e.PaqueteId == id);
        }
    }
}