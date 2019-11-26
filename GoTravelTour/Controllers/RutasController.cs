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
    public class RutasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RutasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Rutas
        [HttpGet]
        public IEnumerable<Rutas> GetRutas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Rutas> lista;
            if (col == "-1")
            {
                return _context.Rutas.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Rutas.Include(r=> r.regionOrigen).Include(rr=>rr.regionDestino)
                    .Include(a=>a.puntoOrigen).Include(aa => aa.puntoOrigen)
                    .Where(p => (p.regionDestino.Nombre.ToLower().Contains(filter.ToLower())) || (p.regionOrigen.Nombre.ToLower().Contains(filter.ToLower())) 
                    || (p.puntoDestino.Nombre.ToLower().Contains(filter.ToLower())) || (p.puntoOrigen.Nombre.ToLower().Contains(filter.ToLower())))
                    .ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Rutas.ToPagedList(pageIndex, pageSize).ToList();
            }

           

            return lista;
            
        }

        // GET: api/Rutas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRutas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rutas = await _context.Rutas.FindAsync(id);

            if (rutas == null)
            {
                return NotFound();
            }

            return Ok(rutas);
        }

        // PUT: api/Rutas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRutas([FromRoute] int id, [FromBody] Rutas rutas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rutas.RutasId)
            {
                return BadRequest();
            }

            _context.Entry(rutas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutasExists(id))
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

        // POST: api/Rutas
        [HttpPost]
        public async Task<IActionResult> PostRutas([FromBody] Rutas rutas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Rutas.Add(rutas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRutas", new { id = rutas.RutasId }, rutas);
        }

        // DELETE: api/Rutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRutas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rutas = await _context.Rutas.FindAsync(id);
            if (rutas == null)
            {
                return NotFound();
            }

            _context.Rutas.Remove(rutas);
            await _context.SaveChangesAsync();

            return Ok(rutas);
        }

        private bool RutasExists(int id)
        {
            return _context.Rutas.Any(e => e.RutasId == id);
        }
    }
}