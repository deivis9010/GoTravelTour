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
    public class ContratoProductoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ContratoProductoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ContratoProductoes
        [HttpGet]
        public IEnumerable<ContratoProducto> GetContratoProducto(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<ContratoProducto> lista;
            if (col == "-1")
            {
                return _context.ContratoProducto
                    .Include(p=>p.Producto)
                    .Include(c=>c.Contrato)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ContratoProducto
                     .Include(p => p.Producto)
                     .Include(c => c.Contrato)
                     .Where(p => (p.Contrato.Nombre.ToLower().Contains(filter.ToLower()))
                     || (p.Producto.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.ContratoProducto
                     .Include(p => p.Producto)
                     .Include(c => c.Contrato)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Contrato.Nombre);

                        }



                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Contrato.Nombre);

                        }


                    }

                    break;
            }
            return lista;
        }

        // GET: api/ContratoProductoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetContratoProductoCount()
        {
            return _context.ContratoProducto.Count();
        }


        // GET: api/ContratoProductoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContratoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contratoProducto = await _context.ContratoProducto.FindAsync(id);

            if (contratoProducto == null)
            {
                return NotFound();
            }

            return Ok(contratoProducto);
        }

        // PUT: api/ContratoProductoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContratoProducto([FromRoute] int id, [FromBody] ContratoProducto contratoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contratoProducto.ContratoProductoId)
            {
                return BadRequest();
            }

            _context.Entry(contratoProducto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContratoProductoExists(id))
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

        // POST: api/ContratoProductoes
        [HttpPost]
        public async Task<IActionResult> PostContratoProducto([FromBody] ContratoProducto contratoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContratoProducto.Add(contratoProducto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContratoProducto", new { id = contratoProducto.ContratoProductoId }, contratoProducto);
        }

        // DELETE: api/ContratoProductoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContratoProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contratoProducto = await _context.ContratoProducto.FindAsync(id);
            if (contratoProducto == null)
            {
                return NotFound();
            }

            _context.ContratoProducto.Remove(contratoProducto);
            await _context.SaveChangesAsync();

            return Ok(contratoProducto);
        }

        private bool ContratoProductoExists(int id)
        {
            return _context.ContratoProducto.Any(e => e.ContratoProductoId == id);
        }
    }
}