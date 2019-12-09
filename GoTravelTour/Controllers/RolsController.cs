using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using MimeKit;
using MimeKit.Text;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RolsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Rols
        [HttpGet]
        public IEnumerable<Rol> GetRoles(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<Rol> lista;
            if (col == "-1")
            {
                return _context.Roles.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Roles.Where(p => (p.NombreRol.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Roles.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("NombreRol".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreRol);

                        }
                        
                       
                        break;
                    }

                default:
                    {
                        if ("NombreRol".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreRol);

                        }
                       

                        break;

                    }

                    break;
            }
         
            return lista;
            
        }
        // GET: api/Rols/Count
        [Route("Count")]
        [HttpGet]
        public int GetRolsCount()
        {
            return _context.Roles.Count();
        }

       

        // GET: api/Rols/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRol([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return Ok(rol);
        }

        // PUT: api/Rols/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol([FromRoute] int id, [FromBody] Rol rol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rol.RolId)
            {
                return BadRequest();
            }
            List<Rol> crol = _context.Roles.Where(c => c.NombreRol == rol.NombreRol && c.RolId != id).ToList();
            if (crol.Count > 0)
            {
                return CreatedAtAction("GetRol", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // POST: api/Rols
        [HttpPost]
        public async Task<IActionResult> PostRol([FromBody] Rol rol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            
            List<Rol> crol = _context.Roles.Where(c => c.NombreRol == rol.NombreRol).ToList();
            if (crol.Count > 0)               
            {
                return CreatedAtAction("GetRol", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRol", new { id = rol.RolId }, rol);
        }

        // DELETE: api/Rols/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return Ok(rol);
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.RolId == id);
        }
    }
}