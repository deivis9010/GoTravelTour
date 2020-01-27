using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList;

namespace GoTravelTour.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiciosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ServiciosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet]
        public IEnumerable<Servicio> GetServicio(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Servicio> lista;
            if (col == "-1")
            {
                return _context.Servicio          
                    
                    .Include(a => a.Producto)
                    .OrderBy(a => a.Nombre)
                    .ToList();
               
               
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Servicio
                    .Include(a => a.Producto)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Servicio
                    .Include(a => a.Producto)
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
        // GET: api/Servicio/Count
        [Route("Count")]
        [HttpGet]
        public int GetServicioCount()
        {
            return _context.Servicio.Count();
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicio = await _context.Servicio.FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }

            return Ok(servicio);
        }

        // PUT: api/Servicios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutServicio([FromRoute] int id, [FromBody] Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != servicio.ServicioId)
            {
                return BadRequest();
            }
           /* if (_context.Servicio.Any(c => c.Nombre == servicio.Nombre && servicio.ServicioId != id))
            {
                return CreatedAtAction("GetServicio", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }*/

            _context.Entry(servicio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioExists(id))
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

        // POST: api/Servicios
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostServicio([FromBody] Servicio servicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           /* if (_context.Servicio.Any(c => c.Nombre == servicio.Nombre ))
            {
                return CreatedAtAction("GetServicio", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }*/
            _context.Servicio.Add(servicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicio", new { id = servicio.ServicioId }, servicio);
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteServicio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicio = await _context.Servicio.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            _context.Servicio.Remove(servicio);
            await _context.SaveChangesAsync();

            return Ok(servicio);
        }

        private bool ServicioExists(int id)
        {
            return _context.Servicio.Any(e => e.ServicioId == id);
        }
        // GET: api/Servicios/Producto/5
        [Route("Producto/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicioProducto([FromRoute] int id)
        {
            List<Servicio> lista = new List<Servicio>();
            lista = _context.Servicio.Where(x => x.ProductoId == id).ToList();
            

            if (lista == null)
            {
                return NotFound();
            }

            return Ok(lista);
        }

    }
}