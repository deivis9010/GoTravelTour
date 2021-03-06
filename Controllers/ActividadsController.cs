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
using Microsoft.AspNetCore.Authorization;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ActividadsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Actividads
        [HttpGet]
        public IEnumerable<Actividad> GetActividadess(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<Actividad> lista;
            if (col == "-1")
            {
                lista = _context.Actividadess
                    //.Include(a => a.ListaComodidades)
                    //.Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                     .Include(a => a.PuntoInteres)
                    //.Include(a => a.TipoProducto)                    
                    //.Include(a => a.Region)
                    .Where(x => x.ProveedorId == idProveedor)
                    .OrderBy(a => a.Nombre)
                    .ToList();

                /* if (lista.Count() > 0)
                 {
                     foreach (var a in lista)
                     {
                         a.ServiciosAdicionados = _context.Servicio.Where(s => s.ProductoId == a.ProductoId).ToList();
                     }
                 }*/

                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Actividadess
                    //.Include(a => a.ListaComodidades)
                    // .Include(v => v.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.PuntoInteres)
                    //.Include(a => a.TipoProducto)                    
                    // .Include(a => a.Region)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Actividadess
                    //.Include(a => a.ListaComodidades)
                    .Include(a => a.Proveedor)
                    //.Include(a => a.TipoProducto)                   
                    //.Include(a => a.Region)
                    .Include(a => a.PuntoInteres)
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
            /*  if (lista.Count() > 0)
              {
                  foreach (var a in lista)
                  {
                      a.ServiciosAdicionados = _context.Servicio.Where(s => s.ProductoId == a.ProductoId).ToList();
                  }
              }*/

            return lista;
        }
        // GET: api/Actividads/Count
        [Route("Count")]
        [HttpGet]
        public int GetActividadsCount()
        {
            return _context.Actividadess.Count();
        }


        // GET: api/Actividads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*var actividad = await _context.Actividadess
                .FindAsync(id);*/

            var actividad = _context.Actividadess
                    .Include(a => a.ListaComodidades)
                    .Include(v => v.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    .Include(a => a.TipoProducto)
                    .Include(a => a.PuntoInteres)
                    .Include(a => a.Region)
                    .Single(x => x.ProductoId == id);


            if (actividad == null)
            {
                return NotFound();
            }
            actividad.ServiciosAdicionados = _context.Servicio.Where(s => s.ProductoId == id).ToList();

            return Ok(actividad);
        }

        // PUT: api/Actividads/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutActividad([FromRoute] int id, [FromBody] Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actividad.ProductoId)
            {
                return BadRequest();
            }
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre && c.ProductoId != id && c.ProveedorId == actividad.ProveedorId))
            {
                return CreatedAtAction("GetActividades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            actividad.Region = _context.Regiones.First(x => x.RegionId == actividad.Region.RegionId);
            actividad.Proveedor = _context.Proveedores.First(x => x.ProveedorId == actividad.ProveedorId);
            if (actividad.PuntoInteres != null)
                actividad.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == actividad.PuntoInteres.PuntoInteresId);
            actividad.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == actividad.TipoProductoId);

            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == actividad.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == actividad.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            if (actividad.ListaDistribuidoresProducto != null)
                foreach (var item in actividad.ListaDistribuidoresProducto)
                {
                    item.ProductoId = actividad.ProductoId;
                    _context.ProductoDistribuidores.Add(item);
                }
            if (actividad.ListaComodidades != null)
                foreach (var item in actividad.ListaComodidades)
                {
                    item.ProductoId = actividad.ProductoId;
                    _context.ComodidadesProductos.Add(item);

                }

            List<Servicio> aEliminar = _context.Servicio.Where(x => x.ProductoId == actividad.ProductoId).ToList();
            foreach (var elim in aEliminar)
            {
                _context.Servicio.Remove(elim);
            }
            if (actividad.ServiciosAdicionados != null)
                foreach (var ser in actividad.ServiciosAdicionados)
                {

                    Servicio servicio = new Servicio();
                    servicio = ser;
                    servicio.ProductoId = actividad.ProductoId;
                    _context.Servicio.Add(servicio);
                    //actividad.ServiciosAdicionados[i] = _context.Servicio.First(ser => ser.ServicioId == actividad.ServiciosAdicionados[i].ServicioId);


                }

            _context.Entry(actividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();



            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetActividad", new { id = actividad.ProductoId }, actividad);
        }

        // POST: api/Actividads
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostActividad([FromBody] Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre && c.ProveedorId == actividad.ProveedorId))
            {
                return CreatedAtAction("GetActividades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            actividad.SKU = u.GetSKUCodigo();

            actividad.Region = _context.Regiones.First(f => f.RegionId == actividad.Region.RegionId);
            if (actividad.PuntoInteres != null)
                actividad.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == actividad.PuntoInteres.PuntoInteresId);
            List<Servicio> temp = new List<Servicio>();
            temp = actividad.ServiciosAdicionados;
            actividad.ServiciosAdicionados = null;
            _context.Actividadess.Add(actividad);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }

            if (temp != null && temp.Count() > 0)
            {
                int i = 0;
                while (i < temp.Count())
                {
                    Servicio servicio = new Servicio();
                    servicio = temp[i];
                    servicio.ProductoId = actividad.ProductoId;
                    _context.Servicio.Add(servicio);
                    //actividad.ServiciosAdicionados[i] = _context.Servicio.First(ser => ser.ServicioId == actividad.ServiciosAdicionados[i].ServicioId);
                    i++;
                }

            }
            _context.SaveChanges();

            return CreatedAtAction("GetActividad", new { id = actividad.ProductoId }, actividad);
        }

        // DELETE: api/Actividads/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteActividad([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var actividad = await _context.Actividadess.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            _context.Actividadess.Remove(actividad);
            await _context.SaveChangesAsync();

            return Ok(actividad);
        }

        private bool ActividadExists(int id)
        {
            return _context.Actividadess.Any(e => e.ProductoId == id);
        }


        // GET: api/Actividads/Filtros
        [HttpGet]
        [Route("FiltrosCount")]
        public IEnumerable<Contrato> GetContratosByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos

                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);

                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                lista = _context.Contratos

               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);



                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                lista = _context.Contratos

                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);



                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                lista = _context.Contratos

                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);



                    }
                return lista;
            }


            return lista;

        }


        // GET: api/Actividads/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0, int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {
                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioActividad = _context.PrecioActividad
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.PrecioServicio.Include(x => x.Servicio)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                lista = _context.Contratos
               .Include(a => a.Distribuidor)
               .Include(a => a.Temporadas)
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
               .OrderBy(a => a.Nombre)
               .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {


                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioActividad = _context.PrecioActividad
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.PrecioServicio.Include(x => x.Servicio)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();

                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);
                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioActividad = _context.PrecioActividad
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.PrecioServicio.Include(x => x.Servicio)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if (contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioActividad = _context.PrecioActividad
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioServicioActividad = _context.PrecioServicio.Include(x => x.Servicio)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                int j = 0;
                                while (j < contrato.Temporadas[i].ListaRestricciones.Count())
                                {
                                    contrato.Temporadas[i].ListaRestricciones[j].PrecioRestriccionesProdutos = _context.RestriccionesPrecios
                                        .Where(x => x.RestriccionesId == contrato.Temporadas[i].ListaRestricciones[j].RestriccionesId).ToList();

                                    j++;
                                }
                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas
                                 .Where(x => x.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                i++;
                            }

                        }


                    }
                return lista;
            }


            return lista;

        }

        private void FiltrarProductoProveedor(int idProveedor, int idProducto, Contrato contrato, int pageIndex = 1, int pageSize = 1)
        {
            if (idProducto == 0 && idProveedor == 0)
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.Producto.ProveedorId == idProveedor)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
        }

        private int CantidadProductoProveedor(int idProveedor, int idProducto, Contrato contrato)
        {

            if (idProducto == 0 && idProveedor == 0)
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY).Count();

            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.ProductoId == idProducto).Count();

            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.Producto.ProveedorId == idProveedor).Count();

            }
            else
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto).Count();

            }
        }


        // Post: api/Actividads/Activar
        [HttpPost]
        [Route("Activar")]
        public async Task<IActionResult> PostAcivarActividads([FromBody] Actividad al)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Actividad a = _context.Actividadess.Single(x => x.ProductoId == al.ProductoId);
            if (al.IsActivo)
                if (!_context.PrecioActividad.Any(x => x.ProductoId == a.ProductoId) &&
                    !_context.RestriccionesPrecios.Any(x => x.ProductoId == a.ProductoId && x.Precio > 0))
                {

                    return CreatedAtAction("ActivarActividad", new { id = -1, error = "Este producto no está listo para activar. Revise los precios" }, new { id = -1, error = "Este producto no está listo para activar. Revise los precios" });
                }
            a.IsActivo = al.IsActivo;



            _context.Entry(a).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(a.ProductoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetActividads", new { id = a.ProductoId }, a);
        }


        // GET: api/Actividads/BuscarOrdenCount
        [HttpPost]
        [Route("BuscarOrdenCount")]
        public int GetOrdenActividadesCount([FromBody] BuscadorActividad buscador, int pageIndex = 1, int pageSize = 1000)
        {

            List<OrdenActividad> lista = new List<OrdenActividad>(); //Lista  a devolver (candidatos)
            string diaSemana = buscador.Fecha.DayOfWeek.ToString();

            switch (diaSemana)
            {
                case "Sunday":
                    {
                        diaSemana = "7";
                        break;
                    }
                case "Monday":
                    {
                        diaSemana = "1";
                        break;
                    }
                case "Tuesday":
                    {
                        diaSemana = "2";
                        break;
                    }
                case "Wednesday":
                    {
                        diaSemana = "3";
                        break;
                    }

                case "Thursday":
                    {
                        diaSemana = "4";
                        break;
                    }

                case "Friday":
                    {
                        diaSemana = "5";
                        break;
                    }
                case "Saturday":
                    {
                        diaSemana = "6";
                        break;
                    }
            }


            //Se buscan todos los alojamientos segun los parametros
            List<Actividad> actividades = _context.Actividadess.Include(d => d.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.Region.RegionId == buscador.RegionActividad.RegionId
              && x.CantidadPersonas >= (buscador.CantidadAdultos + buscador.CantidadMenores)).ToList();


            if (!string.IsNullOrEmpty(buscador.NombreActividad))
            {
                actividades = actividades.Where(x => x.Nombre.Contains(buscador.NombreActividad, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (buscador.CantidadMenores > 0 && actividades != null && actividades.Any())
            {
                actividades = actividades.Where(x => x.PermiteNino).ToList();
            }


            if (actividades != null && actividades.Any())
                actividades = actividades.Where(x => !string.IsNullOrEmpty(x.Schedule) && x.Schedule.Split(" ").Any(d => d == diaSemana)).ToList();

            foreach (var ac in actividades)
            {
                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                OrdenActividad oac = new OrdenActividad();
                oac.Actividad = ac;
                oac.CantAdulto = buscador.CantidadAdultos;
                oac.CantNino = buscador.CantidadMenores;
                oac.FechaActividad = buscador.Fecha;

                foreach (var dist in ac.ListaDistribuidoresProducto)
                {
                    //Se buscan los precios correspondientes de las actividades
                    List<PrecioActividad> precios = _context.PrecioActividad.Include(x => x.Temporada.ListaFechasTemporada)
                            .Include(x => x.Temporada.Contrato.Distribuidor)
                            .Where(x => x.ProductoId == ac.ProductoId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId).ToList();
                    foreach (var p in precios)
                    {
                        //Se obtienen las restricciones ordenadas por el valor maximo de dias para calcular precio segun cantidad de dias
                        List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == p.Temporada.TemporadaId).OrderBy(x => x.Minimo).ToList();

                        if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                        {

                            oac.PrecioActividad = p;
                            oac.Distribuidor = p.Temporada.Contrato.Distribuidor;

                            if (ac.Modalidad.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA))//Es colecctiva
                            {
                                oac.PrecioOrden += p.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }
                            else
                            {
                                Restricciones rt = new Restricciones();
                                bool encontroRangoValido = false;
                                foreach (var item in restricciones) // se evalua por restricciones el valor de la cantidad de dias
                                {
                                    rt = item;
                                    if (item.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= item.Maximo) // si coincide la cantidad de dias con el rango de una restriccion se calcula
                                    {
                                        oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio; //* buscador.CantidadAdultos;

                                        encontroRangoValido = true;
                                        break;
                                    }
                                }

                                if (!encontroRangoValido && restricciones.Any())
                                {
                                    var rtMax = restricciones.Last();
                                    oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == rtMax.RestriccionesId).Precio;//buscador.CantidadAdultos *

                                }
                                oac.PrecioOrden += buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }


                            //busco si la actividad tiene servicios asociados
                            ac.ServiciosAdicionados = _context.Servicio.Where(x => x.ProductoId == ac.ProductoId).ToList();

                            foreach (var serv in ac.ServiciosAdicionados)
                            {

                                PrecioServicio ps = _context.PrecioServicio.First(x => x.ServicioId == serv.ServicioId
                                && x.Temporada.TemporadaId == p.Temporada.TemporadaId
                                && x.Servicio.ProductoId == ac.ProductoId);



                                if (serv.Categoria.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA)) //Es Exclusivsa el servicio o sea se paga por restricciones
                                {
                                    try
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).First(x => x.ServicioId == serv.ServicioId &&
                                        x.Restricciones.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= x.Restricciones.Maximo);
                                        if (rp != null)
                                        {
                                            oac.PrecioOrden += rp.Precio;
                                        }

                                    }
                                    catch
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).Where(x => x.ServicioId == serv.ServicioId).OrderByDescending(x => x.Restricciones.Maximo).First();
                                        oac.PrecioOrden += rp.Precio;
                                    }
                                    oac.PrecioOrden += buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);

                                }
                                else
                                    oac.PrecioOrden += (decimal)ps.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);





                            }




                        }





                        //Se aplica la ganancia correspondiente
                        List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY).ToList();

                        foreach (Sobreprecio s in sobreprecios)
                        {
                            if (s.PrecioDesde <= oac.PrecioOrden && oac.PrecioOrden <= s.PrecioHasta)
                            {
                                oac.Sobreprecio = s;
                                decimal valorAplica = 0;
                                if (s.ValorDinero != null)
                                {
                                    valorAplica = (decimal)s.ValorDinero;
                                    oac.PrecioOrden += valorAplica + ((decimal)s.ValorDinero * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplica = oac.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    oac.PrecioOrden += valorAplica + (oac.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                                }

                                oac.ValorSobreprecioAplicado = valorAplica;
                                break;
                            }

                        }
                        if (oac.PrecioOrden > 0)
                            lista.Add(oac);
                    }

                }




            }



            return lista.Count();
        }
        // POST: api/Actividads/BuscarOrden
        [HttpPost]
        [Route("BuscarOrden")]
        public List<OrdenActividad> GetOrdenActividades([FromBody] BuscadorActividad buscador, int pageIndex = 1, int pageSize = 1000)
        {


            List<OrdenActividad> lista = new List<OrdenActividad>(); //Lista  a devolver (candidatos)
            string diaSemana = buscador.Fecha.DayOfWeek.ToString();

            switch (diaSemana)
            {
                case "Sunday":
                    {
                        diaSemana = "7";
                        break;
                    }
                case "Monday":
                    {
                        diaSemana = "1";
                        break;
                    }
                case "Tuesday":
                    {
                        diaSemana = "2";
                        break;
                    }
                case "Wednesday":
                    {
                        diaSemana = "3";
                        break;
                    }

                case "Thursday":
                    {
                        diaSemana = "4";
                        break;
                    }

                case "Friday":
                    {
                        diaSemana = "5";
                        break;
                    }
                case "Saturday":
                    {
                        diaSemana = "6";
                        break;
                    }
            }


            //Se buscan todos los alojamientos segun los parametros
            List<Actividad> actividades = _context.Actividadess.Include(d => d.ListaDistribuidoresProducto).Where(x => x.IsActivo && x.Region.RegionId == buscador.RegionActividad.RegionId
              && x.CantidadPersonas >= (buscador.CantidadAdultos + buscador.CantidadMenores)).ToList();


            if (!string.IsNullOrEmpty(buscador.NombreActividad))
            {
                actividades = actividades.Where(x => x.Nombre.Contains(buscador.NombreActividad, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (buscador.CantidadMenores > 0 && actividades != null && actividades.Any())
            {
                actividades = actividades.Where(x => x.PermiteNino).ToList();
            }


            if (actividades != null && actividades.Any())
                actividades = actividades.Where(x => !string.IsNullOrEmpty(x.Schedule) && x.Schedule.Split(" ").Any(d => d == diaSemana)).ToList();

            foreach (var ac in actividades)
            {
                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                OrdenActividad oac = new OrdenActividad();
                oac.Actividad = ac;
                oac.CantAdulto = buscador.CantidadAdultos;
                oac.CantNino = buscador.CantidadMenores;
                oac.FechaActividad = buscador.Fecha;

                foreach (var dist in ac.ListaDistribuidoresProducto)
                {
                    //Se buscan los precios correspondientes de las actividades
                    List<PrecioActividad> precios = _context.PrecioActividad.Include(x => x.Temporada.ListaFechasTemporada)
                            .Include(x => x.Temporada.Contrato.Distribuidor)
                            .Where(x => x.ProductoId == ac.ProductoId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId && x.Temporada.Contrato.IsActivo).ToList();
                    foreach (var p in precios)
                    {
                        //Se obtienen las restricciones ordenadas por el valor maximo de dias para calcular precio segun cantidad de dias
                        List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == p.Temporada.TemporadaId).OrderBy(x => x.Minimo).ToList();

                        if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                        {

                            oac.PrecioActividad = p;
                            oac.Distribuidor = p.Temporada.Contrato.Distribuidor;

                            if (ac.Modalidad.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA))//Es colecctiva
                            {
                                oac.PrecioOrden += p.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }
                            else
                            {
                                Restricciones rt = new Restricciones();
                                bool encontroRangoValido = false;
                                foreach (var item in restricciones) // se evalua por restricciones el valor de la cantidad de dias
                                {
                                    rt = item;
                                    if (item.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= item.Maximo) // si coincide la cantidad de dias con el rango de una restriccion se calcula
                                    {
                                        oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio; //* buscador.CantidadAdultos;

                                        encontroRangoValido = true;
                                        break;
                                    }
                                }

                                if (!encontroRangoValido && restricciones.Any())
                                {
                                    var rtMax = restricciones.Last();
                                    oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == rtMax.RestriccionesId).Precio;//buscador.CantidadAdultos *

                                }
                                oac.PrecioOrden += buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }


                            //busco si la actividad tiene servicios asociados
                            ac.ServiciosAdicionados = _context.Servicio.Where(x => x.ProductoId == ac.ProductoId).ToList();

                            foreach (var serv in ac.ServiciosAdicionados)
                            {

                                PrecioServicio ps = _context.PrecioServicio.First(x => x.ServicioId == serv.ServicioId
                                && x.Temporada.TemporadaId == p.Temporada.TemporadaId
                                && x.Servicio.ProductoId == ac.ProductoId);



                                if (serv.Categoria.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA)) //Es Exclusivsa el servicio o sea se paga por restricciones
                                {
                                    try
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).First(x => x.ServicioId == serv.ServicioId &&
                                        x.Restricciones.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= x.Restricciones.Maximo);
                                        if (rp != null)
                                        {
                                            oac.PrecioOrden += rp.Precio;
                                        }

                                    }
                                    catch
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).Where(x => x.ServicioId == serv.ServicioId).OrderByDescending(x => x.Restricciones.Maximo).First();
                                        oac.PrecioOrden += rp.Precio;
                                    }
                                    oac.PrecioOrden += buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);

                                }
                                else
                                    oac.PrecioOrden += (decimal)ps.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);





                            }




                        }





                        //Se aplica la ganancia correspondiente
                        List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY).ToList();

                        foreach (Sobreprecio s in sobreprecios)
                        {
                            if (s.PrecioDesde <= oac.PrecioOrden && oac.PrecioOrden <= s.PrecioHasta)
                            {
                                oac.Sobreprecio = s;
                                decimal valorAplica = 0;
                                if (s.ValorDinero != null)
                                {
                                    valorAplica = (decimal)s.ValorDinero;
                                    oac.PrecioOrden += valorAplica + ((decimal)s.ValorDinero * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplica = oac.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    oac.PrecioOrden += valorAplica + (oac.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                                }

                                oac.ValorSobreprecioAplicado = valorAplica;
                                break;
                            }

                        }
                       if( oac.PrecioOrden > 0)
                        {
                            lista.Add(oac);
                            oac = new OrdenActividad();
                            oac.Actividad = ac;
                            oac.CantAdulto = buscador.CantidadAdultos;
                            oac.CantNino = buscador.CantidadMenores;
                            oac.FechaActividad = buscador.Fecha; 
                        }
                        
                    }

                }




            }

            List<OrdenActividad> listatemp = new List<OrdenActividad>();
            var arr = new OrdenActividad[lista.Count];
            lista.CopyTo(arr);
            listatemp = arr.ToList();
            for (int i = 0; i < listatemp.Count(); i++)
            {
                var item = listatemp[i];
                if (listatemp.Where(x => x.Actividad.ProductoId == item.Actividad.ProductoId).Count() > 1)
                {
                    var mismoProdDifDist = listatemp.Where(x => x.Actividad.ProductoId == item.Actividad.ProductoId).OrderBy(x => x.PrecioOrden);
                    int index = 0;
                    foreach (var elem in mismoProdDifDist)
                    {
                        if (index == 0)
                        {
                            index++;
                            continue;
                        }
                        lista.Remove(elem);
                        listatemp.Remove(elem);
                        index++;
                    }

                }
            }


            return lista.OrderBy(x => x.PrecioOrden).ToList();


        }



        // POST: api/Actividads/BuscarOrden
        [HttpPost]
        [Route("CambiarPrecio")]
        public OrdenActividad GetOrdenActividadesEspecifico([FromBody] BuscadorActividad buscador, int pageIndex = 1, int pageSize = 10)
        {


            List<OrdenActividad> lista = new List<OrdenActividad>(); //Lista  a devolver (candidatos)
            string diaSemana = buscador.Fecha.DayOfWeek.ToString();

            switch (diaSemana)
            {
                case "Sunday":
                    {
                        diaSemana = "7";
                        break;
                    }
                case "Monday":
                    {
                        diaSemana = "1";
                        break;
                    }
                case "Tuesday":
                    {
                        diaSemana = "2";
                        break;
                    }
                case "Wednesday":
                    {
                        diaSemana = "3";
                        break;
                    }

                case "Thursday":
                    {
                        diaSemana = "4";
                        break;
                    }

                case "Friday":
                    {
                        diaSemana = "5";
                        break;
                    }
                case "Saturday":
                    {
                        diaSemana = "6";
                        break;
                    }
            }

            Actividad act = _context.Actividadess.Include(d => d.ListaDistribuidoresProducto).First(x => x.ProductoId == buscador.ProductoId);

            //Se buscan todos los alojamientos segun los parametros
            List<Actividad> actividades = new List<Actividad>();

            actividades.Add(act);
            if (!string.IsNullOrEmpty(buscador.NombreActividad))
            {
                actividades = actividades.Where(x => x.Nombre.Contains(buscador.NombreActividad, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (buscador.CantidadMenores > 0 && actividades != null && actividades.Any())
            {
                actividades = actividades.Where(x => x.PermiteNino).ToList();
            }


            if (actividades != null && actividades.Any())
                actividades = actividades.Where(x => !string.IsNullOrEmpty(x.Schedule) && x.Schedule.Split(" ").Any(d => d == diaSemana)).ToList();

            foreach (var ac in actividades)
            {
                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                OrdenActividad oac = new OrdenActividad();
                oac.Actividad = ac;
                oac.CantAdulto = buscador.CantidadAdultos;
                oac.CantNino = buscador.CantidadMenores;
                oac.FechaActividad = buscador.Fecha;

                foreach (var dist in ac.ListaDistribuidoresProducto)
                {
                    //Se buscan los precios correspondientes de las actividades
                    List<PrecioActividad> precios = _context.PrecioActividad.Include(x => x.Temporada.ListaFechasTemporada)
                            .Include(x => x.Temporada.Contrato.Distribuidor)
                            .Where(x => x.ProductoId == ac.ProductoId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId && x.Temporada.Contrato.IsActivo).ToList();
                    foreach (var p in precios)
                    {
                        //Se obtienen las restricciones ordenadas por el valor maximo de dias para calcular precio segun cantidad de dias
                        List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.TemporadaId == p.Temporada.TemporadaId).OrderBy(x => x.Minimo).ToList();

                        if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                        {

                            oac.PrecioActividad = p;
                            oac.Distribuidor = p.Temporada.Contrato.Distribuidor;

                            if (ac.Modalidad.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA))//Es colecctiva
                            {
                                oac.PrecioOrden += p.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }
                            else
                            {
                                Restricciones rt = new Restricciones();
                                bool encontroRangoValido = false;
                                foreach (var item in restricciones) // se evalua por restricciones el valor de la cantidad de dias
                                {
                                    rt = item;
                                    if (item.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= item.Maximo) // si coincide la cantidad de dias con el rango de una restriccion se calcula
                                    {
                                        oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == item.RestriccionesId).Precio; //* buscador.CantidadAdultos;

                                        encontroRangoValido = true;
                                        break;
                                    }
                                }

                                if (!encontroRangoValido && restricciones.Any())
                                {
                                    var rtMax = restricciones.Last();
                                    oac.PrecioOrden += _context.RestriccionesPrecios.First(x => x.ProductoId == ac.ProductoId && x.RestriccionesId == rtMax.RestriccionesId).Precio;//buscador.CantidadAdultos *

                                }
                                oac.PrecioOrden += buscador.CantidadMenores * (p.PrecioNino) + buscador.CantidadInfantes * p.PrecioInfante;
                            }


                            //busco si la actividad tiene servicios asociados
                            ac.ServiciosAdicionados = _context.Servicio.Where(x => x.ProductoId == ac.ProductoId).ToList();

                            foreach (var serv in ac.ServiciosAdicionados)
                            {

                                PrecioServicio ps = _context.PrecioServicio.First(x => x.ServicioId == serv.ServicioId
                                && x.Temporada.TemporadaId == p.Temporada.TemporadaId
                                && x.Servicio.ProductoId == ac.ProductoId);



                                if (serv.Categoria.Equals(ValoresAuxiliares.GROUP_MODE_COLECTIVA)) //Es Exclusivsa el servicio o sea se paga por restricciones
                                {
                                    try
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).First(x => x.ServicioId == serv.ServicioId &&
                                        x.Restricciones.Minimo <= buscador.CantidadAdultos && buscador.CantidadAdultos <= x.Restricciones.Maximo);
                                        if (rp != null)
                                        {
                                            oac.PrecioOrden += rp.Precio;
                                        }

                                    }
                                    catch
                                    {
                                        RestriccionesPrecio rp = _context.RestriccionesPrecios.Include(r => r.Restricciones).Where(x => x.ServicioId == serv.ServicioId).OrderByDescending(x => x.Restricciones.Maximo).First();
                                        oac.PrecioOrden += rp.Precio;
                                    }
                                    oac.PrecioOrden += buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);

                                }
                                else
                                    oac.PrecioOrden += (decimal)ps.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (decimal)(ps.PrecioNino) + buscador.CantidadInfantes * (decimal)(ps.PrecioInfante);





                            }




                        }





                        //Se aplica la ganancia correspondiente
                        List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.ACTIVITY).ToList();

                        foreach (Sobreprecio s in sobreprecios)
                        {
                            if (s.PrecioDesde <= oac.PrecioOrden && oac.PrecioOrden <= s.PrecioHasta)
                            {
                                oac.Sobreprecio = s;
                                decimal valorAplica = 0;
                                if (s.ValorDinero != null)
                                {
                                    valorAplica = (decimal)s.ValorDinero;
                                    oac.PrecioOrden += valorAplica + (valorAplica * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplica = oac.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    oac.PrecioOrden += valorAplica + (valorAplica) * c.Descuento / 100;
                                }

                                oac.ValorSobreprecioAplicado = valorAplica;
                                break;
                            }

                        }

                        lista.Add(oac);
                    }

                }




            }



            if (lista.Any())
                return lista.OrderByDescending(x => x.PrecioOrden).ToList()[0];
            else
                return new OrdenActividad();


        }



        // GET: api/Actividads/RangosRestricciones
        [HttpPost]
        [Route("RangosRestricciones")]
        public Restricciones GetRangosRestricciones(DateTime fecha)
        {
            Restricciones result = null;
            var nombreTipoProduto = ValoresAuxiliares.ACTIVITY;

            List<RangoFechas> rangosContienenFecha = _context.RangoFechas
                .Where(x => x.FechaInicio <= fecha && fecha <= x.FechaFin).ToList();

            foreach (var rangos in rangosContienenFecha)
            {
                Restricciones temporal = new Restricciones();
                List<Restricciones> restricciones = _context.Restricciones.Where(x => x.Temporada.Contrato.TipoProducto.Nombre == nombreTipoProduto && x.Temporada.TemporadaId == rangos.TemporadaId).ToList();
                if (restricciones != null && restricciones.Any())
                {
                    temporal.Minimo = restricciones.OrderBy(x => x.Minimo).First().Minimo;
                    temporal.Maximo = restricciones.OrderBy(x => x.Maximo).Last().Maximo;
                }
                else
                {
                    continue;
                }

                if (result == null)
                {
                    result = temporal;
                }
                else
                {
                    if (temporal.Minimo < result.Minimo)
                    {
                        result.Minimo = temporal.Minimo;
                    }
                    if (temporal.Maximo > result.Maximo)
                    {
                        result.Maximo = temporal.Maximo;
                    }
                }



            }


            return result;
        }

    }
}
