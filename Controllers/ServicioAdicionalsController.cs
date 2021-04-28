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
    public class ServicioAdicionalsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ServicioAdicionalsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ServicioAdicionals
        [HttpGet]
        public IEnumerable<ServicioAdicional> GetServicioAdicional(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<ServicioAdicional> lista;
            if (col == "-1")
            {
                return _context.ServicioAdicional
                    .OrderBy(a => a.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ServicioAdicional.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.ServicioAdicional.ToPagedList(pageIndex, pageSize).ToList();
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
        // GET: api/ServicioAdicionals/Count
        [Route("Count")]
        [HttpGet]
        public int GetServicioAdicionalsCount()
        {
            return _context.ServicioAdicional.Count();
        }


        // GET: api/ServicioAdicionals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicioAdicional = await _context.ServicioAdicional.FindAsync(id);

            if (servicioAdicional == null)
            {
                return NotFound();
            }

            return Ok(servicioAdicional);
        }

        // PUT: api/ServicioAdicionals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicioAdicional([FromRoute] int id, [FromBody] ServicioAdicional servicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != servicioAdicional.ProductoId)
            {
                return BadRequest();
            }

           

            if (_context.ServicioAdicional.Any(c => c.Nombre == servicioAdicional.Nombre && c.ProductoId != id && c.ProveedorId == servicioAdicional.ProveedorId))
            {
                return CreatedAtAction("GetServicioAdicional", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Entry(servicioAdicional).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicioAdicionalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(servicioAdicional); ;
        }

        // POST: api/ServicioAdicionals
        [HttpPost]
        public async Task<IActionResult> PostServicioAdicional([FromBody] ServicioAdicional servicioAdicional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.ServicioAdicional.Any(c => c.Nombre == servicioAdicional.Nombre && c.ProveedorId == servicioAdicional.ProveedorId))
            {
                return CreatedAtAction("GetServicioAdicional", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            servicioAdicional.SKU = u.GetSKUCodigo();


            _context.ServicioAdicional.Add(servicioAdicional);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServicioAdicional", new { id = servicioAdicional.ProductoId }, servicioAdicional);
        }

        // DELETE: api/ServicioAdicionals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicioAdicional([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicioAdicional = await _context.ServicioAdicional.FindAsync(id);
            if (servicioAdicional == null)
            {
                return NotFound();
            }

            _context.ServicioAdicional.Remove(servicioAdicional);
            await _context.SaveChangesAsync();

            return Ok(servicioAdicional);
        }

        private bool ServicioAdicionalExists(int id)
        {
            return _context.ServicioAdicional.Any(e => e.ProductoId == id);
        }
    }
}