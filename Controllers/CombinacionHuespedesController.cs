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
    public class CombinacionHuespedesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public CombinacionHuespedesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/CombinacionHuespedes
        [HttpGet]
        public IEnumerable<CombinacionHuespedes> GetCombinacionHuespedes(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<CombinacionHuespedes> lista;
            if (col == "-1")
            {
                lista = _context.CombinacionHuespedes               
                    .Include(a => a.Habitacion)
                    .Include(a => a.Hotel)                    
                    .OrderBy(a => a.Hotel.Nombre)
                    .ToList();

                
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.CombinacionHuespedes
                    .Include(a => a.Habitacion)
                    .Include(a => a.Hotel)
                    .OrderBy(a => a.Hotel.Nombre)
                    
                    .Where(p => (p.Hotel.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.CombinacionHuespedes
                       .Include(a => a.Habitacion)
                       .Include(a => a.Hotel)
                       .OrderBy(a => a.Hotel.Nombre)
                       
                    .ToPagedList(pageIndex, pageSize).ToList();

            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Hotel.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Hotel.Nombre);

                        }
                    }

                    break;
            }
          

            return lista;
        }

        // GET: api/CombinacionHuespedes/Count
        [Route("Count")]
        [HttpGet]
        public int GetCombinacionHuespedesCount()
        {
            return _context.CombinacionHuespedes.Count();
        }

        // GET: api/CombinacionHuespedes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCombinacionHuespedes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var combinacionHuespedes = await _context.CombinacionHuespedes.FindAsync(id);

            if (combinacionHuespedes == null)
            {
                return NotFound();
            }

            return Ok(combinacionHuespedes);
        }

        // PUT: api/CombinacionHuespedes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombinacionHuespedes([FromRoute] int id, [FromBody] CombinacionHuespedes combinacionHuespedes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != combinacionHuespedes.CombinacionHuespedesId)
            {
                return BadRequest();
            }
            combinacionHuespedes.Habitacion = _context.Habitaciones.First(h => h.HabitacionId == combinacionHuespedes.Habitacion.HabitacionId);
            _context.Entry(combinacionHuespedes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CombinacionHuespedesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(combinacionHuespedes);
        }

        // POST: api/CombinacionHuespedes
        [HttpPost]
        public async Task<IActionResult> PostCombinacionHuespedes([FromBody] CombinacionHuespedes combinacionHuespedes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            combinacionHuespedes.Habitacion = _context.Habitaciones.First(h => h.HabitacionId == combinacionHuespedes.Habitacion.HabitacionId);

            _context.CombinacionHuespedes.Add(combinacionHuespedes);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCombinacionHuespedes", new { id = combinacionHuespedes.CombinacionHuespedesId }, combinacionHuespedes);
        }

        // DELETE: api/CombinacionHuespedes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombinacionHuespedes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var combinacionHuespedes = await _context.CombinacionHuespedes.FindAsync(id);
            if (combinacionHuespedes == null)
            {
                return NotFound();
            }

            _context.CombinacionHuespedes.Remove(combinacionHuespedes);
            await _context.SaveChangesAsync();

            return Ok(combinacionHuespedes);
        }

        private bool CombinacionHuespedesExists(int id)
        {
            return _context.CombinacionHuespedes.Any(e => e.CombinacionHuespedesId == id);
        }

        

        }
}