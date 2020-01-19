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
    public class CategoriaAutoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public CategoriaAutoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaAutoes
        [HttpGet]
        public IEnumerable<CategoriaAuto> GetCategoriaAuto(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<CategoriaAuto> lista;
            if (col == "-1")
            {
                return _context.CategoriaAuto
                    .Include(x=>x.ListaVehiculos)
                    .OrderBy(a => a.Nombre)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.CategoriaAuto.Include(x => x.ListaVehiculos)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.CategoriaAuto.Include(x => x.ListaVehiculos).ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/CategoriaAutoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetCategoriaAutoCount()
        {
            return _context.CategoriaAuto.Count();
        }

        // GET: api/CategoriaAutoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaAuto = await _context.CategoriaAuto.FindAsync(id);

            if (categoriaAuto == null)
            {
                return NotFound();
            }

            return Ok(categoriaAuto);
        }

        // PUT: api/CategoriaAutoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategoriaAuto([FromRoute] int id, [FromBody] CategoriaAuto categoriaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaAuto.CategoriaAutoId)
            {
                return BadRequest();
            }
            if (_context.CategoriaAuto.Any(c => c.Nombre == categoriaAuto.Nombre && categoriaAuto.CategoriaAutoId != id))
            {
                return CreatedAtAction("GetCategoriaAuto", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(categoriaAuto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaAutoExists(id))
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

        // POST: api/CategoriaAutoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCategoriaAuto([FromBody] CategoriaAuto categoriaAuto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.CategoriaAuto.Any(c => c.Nombre == categoriaAuto.Nombre))
            {
                return CreatedAtAction("GetCategoriaAuto", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.CategoriaAuto.Add(categoriaAuto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaAuto", new { id = categoriaAuto.CategoriaAutoId }, categoriaAuto);
        }

        // DELETE: api/CategoriaAutoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoriaAuto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaAuto = await _context.CategoriaAuto.FindAsync(id);
            if (categoriaAuto == null)
            {
                return NotFound();
            }

            _context.CategoriaAuto.Remove(categoriaAuto);
            await _context.SaveChangesAsync();

            return Ok(categoriaAuto);
        }

        private bool CategoriaAutoExists(int id)
        {
            return _context.CategoriaAuto.Any(e => e.CategoriaAutoId == id);
        }
    }
}