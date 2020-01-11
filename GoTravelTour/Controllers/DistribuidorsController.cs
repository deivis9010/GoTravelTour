using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistribuidorsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public DistribuidorsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Distribuidors
        [HttpGet]
        public IEnumerable<Distribuidor> GetDistribuidor(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Distribuidor> lista;
            if (col == "-1")
            {
                return _context.Distribuidores
                    .OrderBy(a=>a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Distribuidores
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Distribuidores
                    .ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/Distribuidors/Count
        [Route("Count")]
        [HttpGet]
        public int GetDistribuidorsCount()
        {
            return _context.Distribuidores.Count();
        }

        // GET: api/Distribuidors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDistribuidor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var distribuidor = await _context.Distribuidores.FindAsync(id);

            if (distribuidor == null)
            {
                return NotFound();
            }

            return Ok(distribuidor);
        }

        // PUT: api/Distribuidors/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutDistribuidor([FromRoute] int id, [FromBody] Distribuidor distribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != distribuidor.DistribuidorId)
            {
                return BadRequest();
            }
            
            if (_context.Distribuidores.Any(c => c.Nombre == distribuidor.Nombre && distribuidor.DistribuidorId != id))
            {
                return CreatedAtAction("GetDistribuidor", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(distribuidor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DistribuidorExists(id))
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

        // POST: api/Distribuidors
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostDistribuidor([FromBody] Distribuidor distribuidor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Distribuidores.Any(c => c.Nombre == distribuidor.Nombre ))
            {
                return CreatedAtAction("GetDistribuidor", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Distribuidores.Add(distribuidor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDistribuidor", new { id = distribuidor.DistribuidorId }, distribuidor);
        }

        // DELETE: api/Distribuidors/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDistribuidor([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var distribuidor = await _context.Distribuidores.FindAsync(id);
            if (distribuidor == null)
            {
                return NotFound();
            }

            _context.Distribuidores.Remove(distribuidor);
            await _context.SaveChangesAsync();

            return Ok(distribuidor);
        }

        private bool DistribuidorExists(int id)
        {
            return _context.Distribuidores.Any(e => e.DistribuidorId == id);
        }
    }
}