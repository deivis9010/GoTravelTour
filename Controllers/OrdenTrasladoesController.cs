using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace GoTravelTour.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenTrasladoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenTrasladoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenTrasladoes
        [HttpGet]
        public IEnumerable<OrdenTraslado> GetOrdenTraslado(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<OrdenTraslado> lista;
            if (col == "-1")
            {
                lista = _context.OrdenTraslado

                    .OrderBy(a => a.NombreCliente)
                    .ToList();


                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.OrdenTraslado

                    .OrderBy(a => a.NombreCliente)
                    .Where(p => (p.NombreCliente.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.OrdenTraslado

                    .OrderBy(a => a.NombreCliente)
                    .ToPagedList(pageIndex, pageSize).ToList();

            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreCliente);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreCliente);

                        }
                    }

                    break;
            }


            return lista;
        }

        // GET: api/OrdenTrasladoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdenTrasladoesCount()
        {
            return _context.OrdenTraslado.Count();
        }

        // GET: api/OrdenTrasladoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenTraslado = await _context.OrdenTraslado.FindAsync(id);

            if (ordenTraslado == null)
            {
                return NotFound();
            }

            return Ok(ordenTraslado);
        }

        // PUT: api/OrdenTrasladoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenTraslado([FromRoute] int id, [FromBody] OrdenTraslado ordenTraslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenTraslado.OrdenTrasladoId)
            {
                return BadRequest();
            }

            _context.Entry(ordenTraslado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenTrasladoExists(id))
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

        // POST: api/OrdenTrasladoes
        [HttpPost]
        public async Task<IActionResult> PostOrdenTraslado([FromBody] OrdenTraslado ordenTraslado)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenTraslado.Add(ordenTraslado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenTraslado", new { id = ordenTraslado.OrdenTrasladoId }, ordenTraslado);
        }

        // DELETE: api/OrdenTrasladoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenTraslado([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenTraslado = await _context.OrdenTraslado.FindAsync(id);
            if (ordenTraslado == null)
            {
                return NotFound();
            }

            _context.OrdenTraslado.Remove(ordenTraslado);
            await _context.SaveChangesAsync();

            return Ok(ordenTraslado);
        }

        private bool OrdenTrasladoExists(int id)
        {
            return _context.OrdenTraslado.Any(e => e.OrdenTrasladoId == id);
        }

        // Post: api/OrdenTrasladoes/UpdateSobreprecio
        [HttpPost]
        [Route("UpdateSobreprecio")]
        public async Task<IActionResult> PostActualizarSobreprecioAplicado([FromBody] OrdenTraslado oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenTraslado a = _context.OrdenTraslado.Single(x => x.OrdenTrasladoId == oa.OrdenTrasladoId);

            a.ValorSobreprecioAplicado = oa.ValorSobreprecioAplicado;



            _context.Entry(a).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrdenTraslado", new { id = a.OrdenTrasladoId }, a);
        }

    }
}