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
    public class TipoTransportesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoTransportesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoTransportes
        [HttpGet]
        public IEnumerable<TipoTransporte> GetTipoTransportes(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoTransporte> lista;
            if (col == "-1")
            {
                return _context.TipoTransportes.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoTransportes.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoTransportes.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/TipoTransportes/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoTransportesCount()
        {
            return _context.TipoTransportes.Count();
        }


        // GET: api/TipoTransportes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoTransporte([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoTransporte = await _context.TipoTransportes.FindAsync(id);

            if (tipoTransporte == null)
            {
                return NotFound();
            }

            return Ok(tipoTransporte);
        }

        // PUT: api/TipoTransportes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoTransporte([FromRoute] int id, [FromBody] TipoTransporte tipoTransporte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoTransporte.TipoTransporteId)
            {
                return BadRequest();
            }
            List<TipoTransporte> crol = _context.TipoTransportes.Where(c => c.Nombre == tipoTransporte.Nombre && tipoTransporte.TipoTransporteId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetTipoTransportes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(tipoTransporte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoTransporteExists(id))
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

        // POST: api/TipoTransportes
        [HttpPost]
        public async Task<IActionResult> PostTipoTransporte([FromBody] TipoTransporte tipoTransporte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<TipoTransporte> crol = _context.TipoTransportes.Where(c => c.Nombre == tipoTransporte.Nombre).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetTipoTransportes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.TipoTransportes.Add(tipoTransporte);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoTransporte", new { id = tipoTransporte.TipoTransporteId }, tipoTransporte);
        }

        // DELETE: api/TipoTransportes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoTransporte([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoTransporte = await _context.TipoTransportes.FindAsync(id);
            if (tipoTransporte == null)
            {
                return NotFound();
            }

            _context.TipoTransportes.Remove(tipoTransporte);
            await _context.SaveChangesAsync();

            return Ok(tipoTransporte);
        }

        private bool TipoTransporteExists(int id)
        {
            return _context.TipoTransportes.Any(e => e.TipoTransporteId == id);
        }
    }
}