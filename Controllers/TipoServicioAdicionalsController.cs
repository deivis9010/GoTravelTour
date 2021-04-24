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
    public class TipoServicioAdicionalsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public TipoServicioAdicionalsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/TipoServicioAdicionals
        [HttpGet]
        public IEnumerable<TipoServicioAdicional> GetTipoServicioAdicional(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<TipoServicioAdicional> lista;
            if (col == "-1")
            {
                return _context.TipoServicioAdicional
                    .OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.TipoServicioAdicional.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.TipoServicioAdicional.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/TipoServicioAdicionals/Count
        [Route("Count")]
        [HttpGet]
        public int GetTipoServicioAdicionalsCount()
        {
            return _context.TipoServicioAdicional.Count();
        }


        // GET: api/TipoServicioAdicionals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoServicioAdicional = await _context.TipoServicioAdicional.FindAsync(id);

            if (tipoServicioAdicional == null)
            {
                return NotFound();
            }

            return Ok(tipoServicioAdicional);
        }

        // PUT: api/TipoServicioAdicionals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoServicioAdicional([FromRoute] int id, [FromBody] TipoServicioAdicional tipoServicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tipoServicioAdicional.TipoServicioAdicionalId)
            {
                return BadRequest();
            }

            _context.Entry(tipoServicioAdicional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoServicioAdicionalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tipoServicioAdicional);
        }

        // POST: api/TipoServicioAdicionals
        [HttpPost]
        public async Task<IActionResult> PostTipoServicioAdicional([FromBody] TipoServicioAdicional tipoServicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TipoServicioAdicional.Add(tipoServicioAdicional);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoServicioAdicional", new { id = tipoServicioAdicional.TipoServicioAdicionalId }, tipoServicioAdicional);
        }

        // DELETE: api/TipoServicioAdicionals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tipoServicioAdicional = await _context.TipoServicioAdicional.FindAsync(id);
            if (tipoServicioAdicional == null)
            {
                return NotFound();
            }

            _context.TipoServicioAdicional.Remove(tipoServicioAdicional);
            await _context.SaveChangesAsync();

            return Ok(tipoServicioAdicional);
        }

        private bool TipoServicioAdicionalExists(int id)
        {
            return _context.TipoServicioAdicional.Any(e => e.TipoServicioAdicionalId == id);
        }
    }
}