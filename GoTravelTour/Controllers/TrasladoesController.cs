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
    public class TrasladoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TrasladoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Trasladoes
        [HttpGet]
        public IEnumerable<Traslado> GetTraslados(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Traslado> lista;
            if (col == "-1")
            {
                return _context.Traslados
                    .OrderBy(a => a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Traslados
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Traslados.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/Trasladoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetTrasladosCount()
        {
            return _context.Traslados.Count();
        }


        // GET: api/Trasladoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var traslado = await _context.Traslados.FindAsync(id);

            if (traslado == null)
            {
                return NotFound();
            }

            return Ok(traslado);
        }

        // PUT: api/Trasladoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTraslado([FromRoute] int id, [FromBody] Traslado traslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != traslado.ProductoId)
            {
                return BadRequest();
            }
            if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre && traslado.TrasladoId != id))
            {
                return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(traslado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrasladoExists(id))
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

        // POST: api/Trasladoes
        [HttpPost]
        public async Task<IActionResult> PostTraslado([FromBody] Traslado traslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Traslados.Any(c => c.Nombre == traslado.Nombre))
            {
                return CreatedAtAction("GetTraslado", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Traslados.Add(traslado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTraslado", new { id = traslado.ProductoId }, traslado);
        }

        // DELETE: api/Trasladoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var traslado = await _context.Traslados.FindAsync(id);
            if (traslado == null)
            {
                return NotFound();
            }

            _context.Traslados.Remove(traslado);
            await _context.SaveChangesAsync();

            return Ok(traslado);
        }

        private bool TrasladoExists(int id)
        {
            return _context.Traslados.Any(e => e.ProductoId == id);
        }
    }
}