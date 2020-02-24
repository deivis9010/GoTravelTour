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
    public class AlojamientoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public AlojamientoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Alojamientoes
        [HttpGet]
        public IEnumerable<Alojamiento> GetAlojamientos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Alojamiento> lista;
            if (col == "-1")
            {
                lista = _context.Alojamientos
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)                    
                    .OrderBy(a => a.Nombre)
                    .ToList();

              

                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Alojamientos
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)                   
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Alojamientos
                    .Include(a => a.ListaComodidades)
                    .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    .OrderBy(a => a.Nombre)
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
        // GET: api/Alojamientoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetAlojamientoesCount()
        {
            return _context.Alojamientos.Count();
        }

        // GET: api/Alojamientoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alojamiento = await _context.Alojamientos.FindAsync(id);

            if (alojamiento == null)
            {
                return NotFound();
            }

            return Ok(alojamiento);
        }

        // PUT: api/Alojamientoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAlojamiento([FromRoute] int id, [FromBody] Alojamiento alojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alojamiento.ProductoId)
            {
                return BadRequest();
            }

            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);

            _context.Entry(alojamiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlojamientoExists(id))
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

        // POST: api/Alojamientoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAlojamiento([FromBody] Alojamiento alojamiento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            alojamiento.SKU = u.GetSKUCodigo();
            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);
            alojamiento.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == alojamiento.TipoProductoId);
            _context.Alojamientos.Add(alojamiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlojamiento", new { id = alojamiento.ProductoId }, alojamiento);
        }

        // DELETE: api/Alojamientoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAlojamiento([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var alojamiento = await _context.Alojamientos.FindAsync(id);
            if (alojamiento == null)
            {
                return NotFound();
            }

            _context.Alojamientos.Remove(alojamiento);
            await _context.SaveChangesAsync();

            return Ok(alojamiento);
        }

        private bool AlojamientoExists(int id)
        {
            return _context.Alojamientos.Any(e => e.ProductoId == id);
        }
    }
}