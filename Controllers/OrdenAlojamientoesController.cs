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
    public class OrdenAlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenAlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenAlojamientoes
        [HttpGet]
        public IEnumerable<OrdenAlojamiento> GetOrdenAlojamiento(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<OrdenAlojamiento> lista;
            if (col == "-1")
            {
                lista = _context.OrdenAlojamiento

                    .OrderBy(a => a.NombreCliente)
                    .ToList();


                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.OrdenAlojamiento

                    .OrderBy(a => a.NombreCliente)
                    .Where(p => (p.NombreCliente.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.OrdenAlojamiento

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
        // GET: api/OrdenAlojamientoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdenAlojamientoesCount()
        {
            return _context.OrdenAlojamiento.Count();
        }

        // GET: api/OrdenAlojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenAlojamiento = await _context.OrdenAlojamiento.FindAsync(id);

            if (ordenAlojamiento == null)
            {
                return NotFound();
            }

            return Ok(ordenAlojamiento);
        }

        // PUT: api/OrdenAlojamientoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenAlojamiento([FromRoute] int id, [FromBody] OrdenAlojamiento ordenAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenAlojamiento.OrdenAlojamientoId)
            {
                return BadRequest();
            }

            _context.Entry(ordenAlojamiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenAlojamientoExists(id))
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

        // POST: api/OrdenAlojamientoes
        [HttpPost]
        public async Task<IActionResult> PostOrdenAlojamiento([FromBody] OrdenAlojamiento ordenAlojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenAlojamiento.Add(ordenAlojamiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenAlojamiento", new { id = ordenAlojamiento.OrdenAlojamientoId }, ordenAlojamiento);
        }

        // DELETE: api/OrdenAlojamientoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenAlojamiento = await _context.OrdenAlojamiento.FindAsync(id);
            if (ordenAlojamiento == null)
            {
                return NotFound();
            }

            _context.OrdenAlojamiento.Remove(ordenAlojamiento);
            await _context.SaveChangesAsync();

            return Ok(ordenAlojamiento);
        }

        private bool OrdenAlojamientoExists(int id)
        {
            return _context.OrdenAlojamiento.Any(e => e.OrdenAlojamientoId == id);
        }



        // Post: api/OrdenAlojamientoes/UpdateSobreprecio
        [HttpPost]
        [Route("UpdateSobreprecio")]
        public async Task<IActionResult> PostActualizarSobreprecioAplicado([FromBody] OrdenAlojamiento oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenAlojamiento a = _context.OrdenAlojamiento.Single(x => x.OrdenAlojamientoId == oa.OrdenAlojamientoId);
           
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

            return CreatedAtAction("GetOerdenAlojamiento", new { id = a.OrdenAlojamientoId }, a);
        }

    }
}