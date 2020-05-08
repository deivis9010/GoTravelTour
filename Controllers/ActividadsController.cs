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
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre && actividad.ProductoId != id))
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
        public async Task<IActionResult> PostActividad([FromBody] Actividad actividad)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Actividadess.Any(c => c.Nombre == actividad.Nombre))
            {
                return CreatedAtAction("GetActividades", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            actividad.SKU = u.GetSKUCodigo();

            actividad.Region = _context.Regiones.First(f => f.RegionId == actividad.Region.RegionId);
            List<Servicio> temp = new List<Servicio>();
            temp = actividad.ServiciosAdicionados;
            actividad.ServiciosAdicionados = null;
            _context.Actividadess.Add(actividad);
            await _context.SaveChangesAsync();
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
                if (!_context.PrecioActividad.Any(x => x.ProductoId == a.ProductoId) ||
                    !_context.RestriccionesPrecios.Any(x => x.ProductoId == a.ProductoId))
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



        // POST: api/Actividads/BuscarOrden
        [HttpPost]
        [Route("BuscarOrden")]
        public List<OrdenActividad> GetOrdenActividades([FromBody] BuscadorActividad buscador, int pageIndex = 1, int pageSize = 1)
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
            List<Actividad> actividades = _context.Actividadess.Where(x => x.IsActivo && x.PuntoInteres.PuntoInteresId == buscador.LugarActividad.PuntoInteresId
            && x.CantidadPersonas >= (buscador.CantidadAdultos + buscador.CantidadMenores)).ToList();


            if (!string.IsNullOrEmpty(buscador.NombreActividad))
            {
                actividades = actividades.Where(x => x.Nombre.Contains(buscador.NombreActividad)).ToList();
            }

            if (buscador.CantidadMenores > 0)
            {
                actividades = actividades.Where(x => x.PermiteNino).ToList();
            }

           actividades = actividades.Where(x => x.Schedule.Split(" ").Any(d=>d == diaSemana)).ToList();

            foreach (var ac in actividades)
             {
                Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                                                                                                     //Se buscan los precios correspondientes de las actividades
                List<PrecioActividad> precios = _context.PrecioActividad.Include(x => x.Temporada.ListaFechasTemporada)
                        .Include(x => x.Temporada.Contrato.Distribuidor)
                        .Where(x => x.ProductoId == ac.ProductoId ).ToList();
                     foreach (var p in precios)
                     {
                         OrdenActividad oac = new OrdenActividad();
                         if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Fecha && buscador.Fecha <= x.FechaFin))) // si la fecha buscada esta en el rango de precios
                         {
                            
                             oac.PrecioActividad = p;
                             oac.Distribuidor = p.Temporada.Contrato.Distribuidor;
                             oac.Actividad = ac;
                             oac.CantAdulto = buscador.CantidadAdultos;
                             oac.CantNino = buscador.CantidadMenores;
                             oac.FechaActividad = buscador.Fecha;                             
                             oac.PrecioOrden += p.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (p.PrecioNino);

                            //busco si la actividad tiene servicios asociados
                            ac.ServiciosAdicionados = _context.Servicio.Where(x => x.ProductoId == ac.ProductoId).ToList();
                            foreach (var serv in ac.ServiciosAdicionados)
                            {
                              PrecioServicio ps = _context.PrecioServicio.Single(x => x.ServicioId == serv.ServicioId && x.Temporada.TemporadaId == p.Temporada.TemporadaId);
                              oac.PrecioOrden += (decimal)ps.PrecioAdulto * buscador.CantidadAdultos + buscador.CantidadMenores * (decimal)(ps.PrecioNino);
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
                                        oac.PrecioOrden += valorAplica  + ((decimal)s.ValorDinero * c.Descuento / 100);
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
                           
                             lista.Add(oac);
                         }

                     }
                 



             }




            return lista.OrderByDescending(x => x.PrecioOrden).ToList();


        }


    }
}
