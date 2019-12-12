using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaComodidadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public CategoriaComodidadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaComodidads
        [HttpGet]
        public IEnumerable<CategoriaComodidad> GetCategoriaComodidad(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<CategoriaComodidad> lista;
            if (col == "-1")
            {
                return _context.CategoriaComodidades.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.CategoriaComodidades.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); 
            }
            else
            {
                lista = _context.CategoriaComodidades.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/CategoriaComodidads/Count
        [Route("Count")]
        [HttpGet]
        public int GetCategoriaComodidadCount()
        {
            return _context.CategoriaComodidades.Count();
        }

        // GET: api/CategoriaComodidads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaComodidad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaComodidad = await _context.CategoriaComodidades.FindAsync(id);

            if (categoriaComodidad == null)
            {
                return NotFound();
            }

            return Ok(categoriaComodidad);
        }

        // PUT: api/CategoriaComodidads/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategoriaComodidad([FromRoute] int id, [FromBody] CategoriaComodidad categoriaComodidad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaComodidad.CategoriaComodidadId)
            {
                return BadRequest();
            }
            List<CategoriaComodidad> crol = _context.CategoriaComodidades.Where(c => c.Nombre == categoriaComodidad.Nombre && categoriaComodidad.CategoriaComodidadId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetCategoriaComodidad", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(categoriaComodidad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaComodidadExists(id))
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

        // POST: api/CategoriaComodidads
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCategoriaComodidad([FromBody] CategoriaComodidad categoriaComodidad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<CategoriaComodidad> crol = _context.CategoriaComodidades.Where(c => c.Nombre == categoriaComodidad.Nombre).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetCategoriaComodidad", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.CategoriaComodidades.Add(categoriaComodidad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaComodidad", new { id = categoriaComodidad.CategoriaComodidadId }, categoriaComodidad);
        }

        // DELETE: api/CategoriaComodidads/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoriaComodidad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaComodidad = await _context.CategoriaComodidades.FindAsync(id);
            if (categoriaComodidad == null)
            {
                return NotFound();
            }

            _context.CategoriaComodidades.Remove(categoriaComodidad);
            await _context.SaveChangesAsync();

            return Ok(categoriaComodidad);
        }

        private bool CategoriaComodidadExists(int id)
        {
            return _context.CategoriaComodidades.Any(e => e.CategoriaComodidadId == id);
        }
    }
}