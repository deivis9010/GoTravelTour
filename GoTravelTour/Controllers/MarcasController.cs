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
    public class MarcasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public MarcasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Marcas
        [HttpGet]
        public IEnumerable<Marca> GetMarcas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Marca> lista;
            if (col == "-1")
            {
                return _context.Marcas.OrderBy(a=>a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Marcas.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Marcas.ToPagedList(pageIndex, pageSize).ToList();
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
        

        // GET: api/Marcas/Count
        [Route("Count")]
        [HttpGet]
        public int GetMarcasCount()
        {
            return _context.Marcas.Count();
        }


        // GET: api/Marcas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarca([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
            {
                return NotFound();
            }

            return Ok(marca);
        }

        // PUT: api/Marcas/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMarca([FromRoute] int id, [FromBody] Marca marca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != marca.MarcaId)
            {
                return BadRequest();
            }
            if (_context.Marcas.Any(c => c.Nombre == marca.Nombre && marca.MarcaId != id))
            {
                return CreatedAtAction("GetMarcas", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(marca).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarcaExists(id))
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

        // POST: api/Marcas
        [HttpPost]
        public async Task<IActionResult> PostMarca([FromBody] Marca marca)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Marcas.Any(c => c.Nombre == marca.Nombre ))
            {
                return CreatedAtAction("GetMarcas", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMarca", new { id = marca.MarcaId }, marca);
        }

        // DELETE: api/Marcas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMarca([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.MarcaId == id);
        }
    }
}