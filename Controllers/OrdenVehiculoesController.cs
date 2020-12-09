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
    public class OrdenVehiculoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenVehiculoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenVehiculoes
        [HttpGet]
        public IEnumerable<OrdenVehiculo> GetOrdenVehiculo(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<OrdenVehiculo> lista;
            if (col == "-1")
            {
                lista = _context.OrdenVehiculo
                    .Include(x=>x.LugarEntrega)
                    .Include(x=>x.LugarRecogida)

                    .OrderBy(a => a.NombreCliente)
                    .ToList();


                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.OrdenVehiculo
                    .Include(x => x.LugarEntrega)
                    .Include(x => x.LugarRecogida)

                    .OrderBy(a => a.NombreCliente)
                    .Where(p => (p.NombreCliente.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.OrdenVehiculo
                    .Include(x => x.LugarEntrega)
                    .Include(x => x.LugarRecogida)

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
        // GET: api/OrdenVehiculoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdenVehiculoesCount()
        {
            return _context.OrdenVehiculo .Count();
        }

        // GET: api/OrdenVehiculoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenVehiculo = await _context.OrdenVehiculo.FindAsync(id);

            if (ordenVehiculo == null)
            {
                return NotFound();
            }

            return Ok(ordenVehiculo);
        }

        // PUT: api/OrdenVehiculoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenVehiculo([FromRoute] int id, [FromBody] OrdenVehiculo ordenVehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenVehiculo.OrdenVehiculoId)
            {
                return BadRequest();
            }

            _context.Entry(ordenVehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenVehiculoExists(id))
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

        // POST: api/OrdenVehiculoes
        [HttpPost]
        public async Task<IActionResult> PostOrdenVehiculo([FromBody] OrdenVehiculo ordenVehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenVehiculo.Add(ordenVehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenVehiculo", new { id = ordenVehiculo.OrdenVehiculoId }, ordenVehiculo);
        }

        // DELETE: api/OrdenVehiculoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenVehiculo =  _context.OrdenVehiculo.Include(x => x.ListaPreciosRentaAutos).First(x=>x.OrdenVehiculoId==id);


            if (ordenVehiculo == null)
            {
                return NotFound();
            }

            _context.OrdenVehiculo.Remove(ordenVehiculo);
            try
            {
                await _context.SaveChangesAsync();
            }catch(Exception es)
            {
                var a = es.Message;
            }
           

            return Ok(ordenVehiculo);
        }

        private bool OrdenVehiculoExists(int id)
        {
            return _context.OrdenVehiculo.Any(e => e.OrdenVehiculoId == id);
        }

        // Post: api/OrdenVehiculoes/UpdateSobreprecio
        [HttpPost]
        [Route("UpdateSobreprecio")]
        public async Task<IActionResult> PostActualizarSobreprecioAplicado([FromBody] OrdenVehiculo oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenVehiculo a = _context.OrdenVehiculo.Single(x => x.OrdenVehiculoId == oa.OrdenVehiculoId);
            Orden orden = _context.Orden.First(x => x.OrdenId == oa.OrdenId);
            orden.PrecioGeneralOrden -= a.PrecioOrden;
            orden.PrecioGeneralOrden += oa.PrecioOrden;

            a.ValorSobreprecioAplicado = oa.ValorSobreprecioAplicado;
            a.PrecioOrden = oa.PrecioOrden;


            _context.Entry(a).State = EntityState.Modified;
            _context.Entry(orden).State = EntityState.Modified;


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

            return CreatedAtAction("GetOrdenVehiculo", new { id = a.OrdenVehiculoId }, a);
        }


        // Post: api/OrdenVehiculoes/UpdatePrecio
        [HttpPost]
        [Route("UpdatePrecio")]
        public async Task<IActionResult> PostActualizarPrecioModificado([FromBody] OrdenVehiculo oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenVehiculo a = _context.OrdenVehiculo.Single(x => x.OrdenVehiculoId == oa.OrdenVehiculoId);
          
            a.PrecioOrden = oa.PrecioOrden;


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

            return CreatedAtAction("GetOrdenVehiculo", new { id = a.OrdenVehiculoId }, a);
        }
    }
}