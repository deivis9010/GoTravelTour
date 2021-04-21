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
    public class OrdenServicioAdicionalsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenServicioAdicionalsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenServicioAdicionals
        [HttpGet]
        public IEnumerable<OrdenServicioAdicional> GetOrdenServicioAdicional(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<OrdenServicioAdicional> lista;
            if (col == "-1")
            {
                return _context.OrdenServicioAdicional
                    .OrderBy(a => a.Descripcion).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.OrdenServicioAdicional
                    .Where(p => (p.Descripcion.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.OrdenServicioAdicional
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Descripcion".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Descripcion);

                        }

                        break;
                    }

                default:
                    {
                        if ("Descripcion".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Descripcion);

                        }

                    }

                    break;
            }

            return lista;
        }

        // GET: api/OrdenServicioAdicionals/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdenServicioAdicionalCount()
        {
            return _context.OrdenServicioAdicional.Count();
        }

        // GET: api/OrdenServicioAdicionals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenServicioAdicional = await _context.OrdenServicioAdicional.FindAsync(id);

            if (ordenServicioAdicional == null)
            {
                return NotFound();
            }

            return Ok(ordenServicioAdicional);
        }

        // PUT: api/OrdenServicioAdicionals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenServicioAdicional([FromRoute] int id, [FromBody] OrdenServicioAdicional ordenServicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenServicioAdicional.OrdenServicioAdicionalId)
            {
                return BadRequest();
            }

            _context.Entry(ordenServicioAdicional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenServicioAdicionalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ordenServicioAdicional);
        }

        // POST: api/OrdenServicioAdicionals
        [HttpPost]
        public async Task<IActionResult> PostOrdenServicioAdicional([FromBody] OrdenServicioAdicional ordenServicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenServicioAdicional.Add(ordenServicioAdicional);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenServicioAdicional", new { id = ordenServicioAdicional.OrdenServicioAdicionalId }, ordenServicioAdicional);
        }

        // DELETE: api/OrdenServicioAdicionals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenServicioAdicional = await _context.OrdenServicioAdicional.FindAsync(id);
            if (ordenServicioAdicional == null)
            {
                return NotFound();
            }

            _context.OrdenServicioAdicional.Remove(ordenServicioAdicional);
            await _context.SaveChangesAsync();

            return Ok(ordenServicioAdicional);
        }

        private bool OrdenServicioAdicionalExists(int id)
        {
            return _context.OrdenServicioAdicional.Any(e => e.OrdenServicioAdicionalId == id);
        }
    }
}