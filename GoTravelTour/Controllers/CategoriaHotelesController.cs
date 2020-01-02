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
    public class CategoriaHotelesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public CategoriaHotelesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaHoteles
        [HttpGet]
        public IEnumerable<CategoriaHoteles> GetCategoriaHoteles(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<CategoriaHoteles> lista;
            if (col == "-1")
            {
                return _context.CategoriaHoteles                    
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.CategoriaHoteles                    
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.CategoriaHoteles                    
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
        // GET: api/CategoriaHoteles/Count
        [Route("Count")]
        [HttpGet]
        public int GetCategoriaHotelesCount()
        {
            return _context.CategoriaHoteles.Count();
        }

        // GET: api/CategoriaHoteles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaHoteles([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaHoteles = await _context.CategoriaHoteles.FindAsync(id);

            if (categoriaHoteles == null)
            {
                return NotFound();
            }

            return Ok(categoriaHoteles);
        }

        // PUT: api/CategoriaHoteles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoriaHoteles([FromRoute] int id, [FromBody] CategoriaHoteles categoriaHoteles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != categoriaHoteles.CategoriaHotelesId)
            {
                return BadRequest();
            }

            _context.Entry(categoriaHoteles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaHotelesExists(id))
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

        // POST: api/CategoriaHoteles
        [HttpPost]
        public async Task<IActionResult> PostCategoriaHoteles([FromBody] CategoriaHoteles categoriaHoteles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CategoriaHoteles.Add(categoriaHoteles);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoriaHoteles", new { id = categoriaHoteles.CategoriaHotelesId }, categoriaHoteles);
        }

        // DELETE: api/CategoriaHoteles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoriaHoteles([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoriaHoteles = await _context.CategoriaHoteles.FindAsync(id);
            if (categoriaHoteles == null)
            {
                return NotFound();
            }

            _context.CategoriaHoteles.Remove(categoriaHoteles);
            await _context.SaveChangesAsync();

            return Ok(categoriaHoteles);
        }

        private bool CategoriaHotelesExists(int id)
        {
            return _context.CategoriaHoteles.Any(e => e.CategoriaHotelesId == id);
        }
    }
}