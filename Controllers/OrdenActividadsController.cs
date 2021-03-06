﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using GoTravelTour.Utiles;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenActividadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public OrdenActividadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/OrdenActividads
        [HttpGet]
        public IEnumerable<OrdenActividad> GetOrdenActividad(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<OrdenActividad> lista;
            if (col == "-1")
            {
                lista = _context.OrdenActividad

                    .OrderBy(a => a.NombreCliente)
                    .ToList();


                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.OrdenActividad

                    .OrderBy(a => a.NombreCliente)
                    .Where(p => (p.NombreCliente.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.OrdenActividad

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
        // GET: api/OrdenActividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetOrdenActividadsCount()
        {
            return _context.OrdenActividad.Count();
        }

        // GET: api/OrdenActividads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrdenActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenActividad = await _context.OrdenActividad.FindAsync(id);

            if (ordenActividad == null)
            {
                return NotFound();
            }

            return Ok(ordenActividad);
        }

        // PUT: api/OrdenActividads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenActividad([FromRoute] int id, [FromBody] OrdenActividad ordenActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordenActividad.OrdenActividadId)
            {
                return BadRequest();
            }

            OrdenActividad oa = _context.OrdenActividad.Single(x => x.OrdenActividadId == id);
            Orden orden = _context.Orden.Single(x => x.OrdenId == ordenActividad.OrdenId);

            orden.PrecioGeneralOrden -= oa.PrecioOrden;
            orden.PrecioGeneralOrden += ordenActividad.PrecioOrden;
            oa.PrecioOrden = ordenActividad.PrecioOrden;
            oa.ServiciosExcluidos = ordenActividad.ServiciosExcluidos;

            Cliente c = _context.Clientes.First(x => x.ClienteId == orden.ClienteId);

            //Se aplica la ganancia correspondiente
            List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY).ToList();

            foreach (Sobreprecio s in sobreprecios)
            {
                if (s.PrecioDesde <= oa.PrecioOrden && oa.PrecioOrden <= s.PrecioHasta)
                {
                    oa.Sobreprecio = s;
                    decimal valorAplica = 0;
                    if (s.ValorDinero != null)
                    {
                        valorAplica = (decimal)s.ValorDinero;
                        oa.PrecioOrden += valorAplica + ((decimal)s.ValorDinero * c.Descuento / 100);
                    }
                    else
                    {
                        valorAplica = oa.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                        oa.PrecioOrden += valorAplica + (oa.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                    }

                    oa.ValorSobreprecioAplicado = valorAplica;
                    break;
                }

            }

            _context.Entry(oa).State = EntityState.Modified;
            _context.Entry(orden).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdenActividadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(oa);
        }

        // POST: api/OrdenActividads
        [HttpPost]
        public async Task<IActionResult> PostOrdenActividad([FromBody] OrdenActividad ordenActividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OrdenActividad.Add(ordenActividad);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrdenActividad", new { id = ordenActividad.OrdenActividadId }, ordenActividad);
        }

        // DELETE: api/OrdenActividads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordenActividad = await _context.OrdenActividad.FindAsync(id);
            if (ordenActividad == null)
            {
                return NotFound();
            }

            _context.OrdenActividad.Remove(ordenActividad);
            await _context.SaveChangesAsync();

            return Ok(ordenActividad);
        }

        private bool OrdenActividadExists(int id)
        {
            return _context.OrdenActividad.Any(e => e.OrdenActividadId == id);
        }


        // Post: api/OrdenActividads/UpdateSobreprecio
        [HttpPost]
        [Route("UpdateSobreprecio")]
        public async Task<IActionResult> PostActualizarSobreprecioAplicado([FromBody] OrdenActividad oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenActividad a = _context.OrdenActividad.Single(x => x.OrdenActividadId == oa.OrdenActividadId);
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

            return CreatedAtAction("GetOrdenActividad", new { id = a.OrdenActividadId }, a);
        }

        // Post: api/OrdenActividads/UpdatePrecio
        [HttpPost]
        [Route("UpdatePrecio")]
        public async Task<IActionResult> PostActualizarPrecioModificado([FromBody] OrdenActividad oa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            OrdenActividad a = _context.OrdenActividad.Single(x => x.OrdenActividadId == oa.OrdenActividadId);

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

            return CreatedAtAction("GetOrdenVehiculo", new { id = a.OrdenActividadId }, a);
        }


    }
}