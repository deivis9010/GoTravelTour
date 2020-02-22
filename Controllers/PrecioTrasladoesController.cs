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
    public class PrecioTrasladoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioTrasladoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioTrasladoes
        [HttpGet]
        public IEnumerable<PrecioTraslado> GetPrecioTraslados( string col ="", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PrecioTraslado> lista;
            if (col == "-1")
            {
                return _context.PrecioTraslados
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Traslado)
                    .Include(a => a.Rutas)
                    .ToList();
    }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioTraslados
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Traslado)
                    .Include(a => a.Rutas)
                    .Where(p => (p.Traslado.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.PrecioTraslados
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Traslado)
                    .Include(a => a.Rutas)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Traslado.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Traslado.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/PrecioTrasladoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioTrasladoCount()
        {
            return _context.PrecioTraslados.Count();
        }

        // GET: api/PrecioTrasladoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioTraslado = await _context.PrecioTraslados.FindAsync(id);

            if (precioTraslado == null)
            {
                return NotFound();
            }

            return Ok(precioTraslado);
        }

        // PUT: api/PrecioTrasladoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioTraslado([FromRoute] int id, [FromBody] PrecioTraslado precioTraslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioTraslado.PrecioTrasladoId)
            {
                return BadRequest();
            }
            precioTraslado.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioTraslado.Temporada.TemporadaId);
            precioTraslado.Rutas = _context.Rutas.First(x => x.RutasId == precioTraslado.RutasId);
            _context.Entry(precioTraslado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioTrasladoExists(id))
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

        // POST: api/PrecioTrasladoes
        [HttpPost]
        public async Task<IActionResult> PostPrecioTraslado([FromBody] PrecioTraslado precioTraslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            precioTraslado.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioTraslado.Temporada.TemporadaId);
            precioTraslado.Rutas = _context.Rutas.First(x => x.RutasId == precioTraslado.RutasId);
            _context.PrecioTraslados.Add(precioTraslado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioTraslado", new { id = precioTraslado.PrecioTrasladoId }, precioTraslado);
        }

        // DELETE: api/PrecioTrasladoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioTraslado = await _context.PrecioTraslados.FindAsync(id);
            if (precioTraslado == null)
            {
                return NotFound();
            }

            _context.PrecioTraslados.Remove(precioTraslado);
            await _context.SaveChangesAsync();

            return Ok(precioTraslado);
        }

        private bool PrecioTrasladoExists(int id)
        {
            return _context.PrecioTraslados.Any(e => e.PrecioTrasladoId == id);
        }
    }
}