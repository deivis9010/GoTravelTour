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
    public class HabitacionsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public HabitacionsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Habitacions
        [HttpGet]
        public IEnumerable<Habitacion> GetHabitaciones(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Habitacion> lista;
            if (col == "-1")
            {
                lista = _context.Habitaciones
                    .Include(a => a.Producto )
                    .Include(a => a.NombreHabitacion)
                    .Include(a => a.CategoriaHabitacion)
                    .Include(a => a.ListaTiposHabitaciones)
                    .Include(a => a.ListaServiciosHabitacion)
                    .Include(a => a.ListaCombinacionesDisponibles)
                    .OrderBy(a=> a.Descripcion)
                    
                    .ToList();

                

                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Habitaciones
                    .Include(a => a.Producto)
                    .Include(a => a.NombreHabitacion)
                    .Include(a => a.CategoriaHabitacion)
                    .Include(a => a.ListaTiposHabitaciones)
                    .Include(a => a.ListaServiciosHabitacion)
                    .Include(a => a.ListaCombinacionesDisponibles)
                    .OrderBy(a => a.Descripcion)
                    .Where(p => (p.Descripcion.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Habitaciones
                    .Include(a => a.Producto)
                    .Include(a => a.NombreHabitacion)
                    .Include(a => a.CategoriaHabitacion)
                    .Include(a => a.ListaTiposHabitaciones)
                    .Include(a => a.ListaServiciosHabitacion)
                    .Include(a => a.ListaCombinacionesDisponibles)
                    .OrderBy(a => a.Descripcion)
                    .ToPagedList(pageIndex, pageSize).ToList();

            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Descripcion);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Descripcion);

                        }
                    }

                    break;
            }
            

            return lista;
        }
        // GET: api/Habitacions/Count
        [Route("Count")]
        [HttpGet]
        public int GetHabitacionsCount()
        {
            return _context.Habitaciones.Count();
        }

        // GET: api/Habitacions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacion = await _context.Habitaciones.FindAsync(id);

            if (habitacion == null)
            {
                return NotFound();
            }

            return Ok(habitacion);
        }

        // PUT: api/Habitacions/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutHabitacion([FromRoute] int id, [FromBody] Habitacion habitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != habitacion.HabitacionId)
            {
                return BadRequest();
            }
            if (habitacion.NombreHabitacion != null && habitacion.NombreHabitacion.NombreHabitacionId > 0)
                habitacion.NombreHabitacion = _context.NombreHabitacion.First(x => x.NombreHabitacionId == habitacion.NombreHabitacion.NombreHabitacionId);
            if (habitacion.CategoriaHabitacion != null && habitacion.CategoriaHabitacion.CategoriaHabitacionId > 0)
                habitacion.CategoriaHabitacion = _context.CategoriaHabitacion.First(x => x.CategoriaHabitacionId == habitacion.CategoriaHabitacion.CategoriaHabitacionId);

            //Eliminando combinaciones anteriores
            List<CombinacionHuespedes> combAnteriores = _context.CombinacionHuespedes.Where(x => x.Habitacion.HabitacionId == habitacion.HabitacionId && x.ProductoId == habitacion.ProductoId).ToList();
            foreach( var item in combAnteriores)
            {
                _context.CombinacionHuespedes.Remove(item);
            }          

            if (habitacion.ListaCombinacionesDisponibles != null)
            {
                int i = 0;
                while (i < habitacion.ListaCombinacionesDisponibles.Count())
                {
                    habitacion.ListaCombinacionesDisponibles[i] = _context.CombinacionHuespedes.Find(habitacion.ListaCombinacionesDisponibles[i].CombinacionHuespedesId);
                    i++;
                }
            }

            _context.Entry(habitacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HabitacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(habitacion);
        }

        // POST: api/Habitacions
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostHabitacion([FromBody] Habitacion habitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            habitacion.SKU = u.GetSKUCodigo();
             if (habitacion.NombreHabitacion != null && habitacion.NombreHabitacion.NombreHabitacionId > 0)
            habitacion.NombreHabitacion = _context.NombreHabitacion.First(x => x.NombreHabitacionId == habitacion.NombreHabitacion.NombreHabitacionId);
            if (habitacion.CategoriaHabitacion != null && habitacion.CategoriaHabitacion.CategoriaHabitacionId > 0)
                habitacion.CategoriaHabitacion = _context.CategoriaHabitacion.First(x => x.CategoriaHabitacionId == habitacion.CategoriaHabitacion.CategoriaHabitacionId);

            if (habitacion.ListaCombinacionesDisponibles != null)
            {
                int i = 0;
                while( i < habitacion.ListaCombinacionesDisponibles.Count())
                {
                    habitacion.ListaCombinacionesDisponibles[i] = _context.CombinacionHuespedes.Find(habitacion.ListaCombinacionesDisponibles[i].CombinacionHuespedesId);
                    i++;
                }
            }
            _context.Habitaciones.Add(habitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHabitacion", new { id = habitacion.HabitacionId }, habitacion);
        }

        // DELETE: api/Habitacions/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteHabitacion([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null)
            {
                return NotFound();
            }

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();

            return Ok(habitacion);
        }

        private bool HabitacionExists(int id)
        {
            return _context.Habitaciones.Any(e => e.HabitacionId == id);
        }


        // GET: api/Habitacions/Producto/{idP}
        [HttpGet]
        [Route("Producto/{idP}")]
        public IEnumerable<Habitacion> GetHabitacionesByIdProductos([FromRoute] int idP = -1)
        {
            var habitacion = _context.Habitaciones
                .Include(x=>x.Producto)
                .Include(x => x.ListaTiposHabitaciones)
                .Include(x => x.ListaServiciosHabitacion)
                .Where(x => x.ProductoId == idP);

            return habitacion;
        }


    }
}