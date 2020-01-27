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
    public class PrecioRentaAutosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PrecioRentaAutosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PrecioRentaAutos
        [HttpGet]
        public IEnumerable<PrecioRentaAutos> GetPrecioRentaAutos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<PrecioRentaAutos> lista;
            if (col == "-1")
            {
                return _context.PrecioRentaAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Auto)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.PrecioRentaAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Auto)
                    .Where(p => (p.Auto.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.PrecioRentaAutos
                    .Include(a => a.Temporada)
                    .Include(a => a.Temporada.Contrato)
                    .Include(a => a.Auto)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Auto.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Auto.Nombre);

                        }




                    }

                    break;
            }

            return lista;
        }
        // GET: api/PrecioRentaAutos/Count
        [Route("Count")]
        [HttpGet]
        public int GetPrecioRentaAutosCount()
        {
            return _context.PrecioRentaAutos.Count();
        }

        // GET: api/PrecioRentaAutos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrecioRentaAutos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioRentaAutos = await _context.PrecioRentaAutos.FindAsync(id);

            if (precioRentaAutos == null)
            {
                return NotFound();
            }

            return Ok(precioRentaAutos);
        }

        // PUT: api/PrecioRentaAutos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrecioRentaAutos([FromRoute] int id, [FromBody] PrecioRentaAutos precioRentaAutos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != precioRentaAutos.PrecioRentaAutosId)
            {
                return BadRequest();
            }
            //precioRentaAutos.Contrato = _context.Contratos.First(x => x.ContratoId == precioRentaAutos.Contrato.ContratoId);
            precioRentaAutos.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioRentaAutos.Temporada.TemporadaId);
            _context.Entry(precioRentaAutos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrecioRentaAutosExists(id))
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

        // POST: api/PrecioRentaAutos
        [HttpPost]
        public async Task<IActionResult> PostPrecioRentaAutos([FromBody] PrecioRentaAutos precioRentaAutos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //precioRentaAutos.Contrato = _context.Contratos.First(x => x.ContratoId == precioRentaAutos.Contrato.ContratoId);
            precioRentaAutos.Temporada = _context.Temporadas.First(x => x.TemporadaId == precioRentaAutos.Temporada.TemporadaId);
            _context.PrecioRentaAutos.Add(precioRentaAutos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrecioRentaAutos", new { id = precioRentaAutos.PrecioRentaAutosId }, precioRentaAutos);
        }

        // DELETE: api/PrecioRentaAutos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrecioRentaAutos([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var precioRentaAutos = await _context.PrecioRentaAutos.FindAsync(id);
            if (precioRentaAutos == null)
            {
                return NotFound();
            }

            _context.PrecioRentaAutos.Remove(precioRentaAutos);
            await _context.SaveChangesAsync();

            return Ok(precioRentaAutos);
        }

        private bool PrecioRentaAutosExists(int id)
        {
            return _context.PrecioRentaAutos.Any(e => e.PrecioRentaAutosId == id);
        }
    }
}