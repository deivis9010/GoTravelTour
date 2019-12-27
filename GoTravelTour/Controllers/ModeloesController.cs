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
    public class ModeloesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ModeloesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Modeloes
        [HttpGet]
        public IEnumerable<Modelo> GetModelos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<Modelo> lista;
            if (col == "-1")
            {
                return _context.Modelos.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Modelos.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Modelos.ToPagedList(pageIndex, pageSize).ToList();
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

        // GET: api/Modeloes/Count
        [Route("Count")]
        [HttpGet]
        public int GetModelosCount()
        {
            return _context.Modelos.Count();
        }


        // GET: api/Modeloes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModelo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modelo = await _context.Modelos.FindAsync(id);

            if (modelo == null)
            {
                return NotFound();
            }

            return Ok(modelo);
        }

        // PUT: api/Modeloes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutModelo([FromRoute] int id, [FromBody] Modelo modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modelo.ModeloId)
            {
                return BadRequest();
            }
            if (_context.Modelos.Any(c => c.Nombre == modelo.Nombre && modelo.ModeloId != id))
            {
                return CreatedAtAction("GetModeloes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(modelo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModeloExists(id))
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

        // POST: api/Modeloes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostModelo([FromBody] Modelo modelo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Modelos.Any(c => c.Nombre == modelo.Nombre))
            {
                return CreatedAtAction("GetModeloes", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Modelos.Add(modelo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModelo", new { id = modelo.ModeloId }, modelo);
        }

        // DELETE: api/Modeloes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteModelo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo == null)
            {
                return NotFound();
            }

            _context.Modelos.Remove(modelo);
            await _context.SaveChangesAsync();

            return Ok(modelo);
        }

        private bool ModeloExists(int id)
        {
            return _context.Modelos.Any(e => e.ModeloId == id);
        }
    }
}