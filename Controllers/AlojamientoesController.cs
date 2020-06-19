﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using Microsoft.AspNetCore.Authorization;
using GoTravelTour.Utiles;

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
        public IEnumerable<Alojamiento> GetAlojamientos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor = 0)
        {
            IEnumerable<Alojamiento> lista;
            if (col == "-1")
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                    //.Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    .Include(a => a.PuntoInteres)
                    // .Include(a => a.PuntoInteres)
                    // .Include(a => a.CategoriaHoteles)
                    // .Include(a => a.ListaPlanesAlimenticios)
                    .Where(x => x.ProveedorId == idProveedor)
                    .OrderBy(a => a.Nombre)
                    .ToList();



                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                    // .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    .Include(a => a.PuntoInteres)
                    // .Include(a => a.PuntoInteres)
                    //.Include(a => a.CategoriaHoteles)
                    // .Include(a => a.ListaPlanesAlimenticios)
                    .OrderBy(a => a.Nombre)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;

            }
            else
            {
                lista = _context.Alojamientos
                    //.Include(a => a.ListaComodidades)
                    // .Include(a => a.ListaDistribuidoresProducto)
                    .Include(a => a.Proveedor)
                    // .Include(a => a.TipoProducto)
                    .Include(a => a.TipoAlojamiento)
                    .Include(a => a.PuntoInteres)
                    //.Include(a => a.PuntoInteres)
                    //.Include(a => a.CategoriaHoteles)
                    // .Include(a => a.ListaPlanesAlimenticios)
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

            /*var alojamiento = await _context.Alojamientos
                .FindAsync(id);*/
            var alojamiento = _context.Alojamientos
             .Include(a => a.ListaComodidades)
                .Include(a => a.ListaDistribuidoresProducto)
                .Include(a => a.Proveedor)
                .Include(a => a.TipoProducto)
                .Include(a => a.TipoAlojamiento)
                .Include(a => a.PuntoInteres)
                .Include(a => a.CategoriaHoteles)
                .Include(a => a.ListaPlanesAlimenticios).Single(x => x.ProductoId == id);

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
            if (_context.Alojamientos.Any(x => x.Nombre.Trim() == alojamiento.Nombre.Trim() && alojamiento.ProductoId != id))
            {
                return CreatedAtAction("GetAlojamiento", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            if (alojamiento.PuntoInteres != null && alojamiento.PuntoInteres.PuntoInteresId > 0)
                alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);
            alojamiento.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == alojamiento.TipoProductoId);
            if (alojamiento.CategoriaHotelesId > 0)
                alojamiento.CategoriaHoteles = _context.CategoriaHoteles.First(x => x.CategoriaHotelesId == alojamiento.CategoriaHotelesId);

            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            List<AlojamientosPlanesAlimenticios> planes = _context.AlojamientosPlanesAlimenticios.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
            foreach (var item in planes)
            {
                _context.AlojamientosPlanesAlimenticios.Remove(item);
            }
            if (alojamiento.ListaDistribuidoresProducto != null)
                foreach (var item in alojamiento.ListaDistribuidoresProducto)
                {
                    item.ProductoId = alojamiento.ProductoId;
                    _context.ProductoDistribuidores.Add(item);
                }
            if (alojamiento.ListaComodidades != null)
                foreach (var item in alojamiento.ListaComodidades)
                {
                    item.ProductoId = alojamiento.ProductoId;
                    _context.ComodidadesProductos.Add(item);

                }
            if (alojamiento.ListaPlanesAlimenticios != null)
                foreach (var item in alojamiento.ListaPlanesAlimenticios)
                {
                    item.ProductoId = alojamiento.ProductoId;
                    _context.AlojamientosPlanesAlimenticios.Add(item);

                }
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

            return Ok(alojamiento);
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
            if (_context.Alojamientos.Any(x => x.Nombre.Trim() == alojamiento.Nombre.Trim()))
            {
                return CreatedAtAction("GetAlojamiento", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            alojamiento.SKU = u.GetSKUCodigo();
            alojamiento.Proveedor = _context.Proveedores.First(x => x.ProveedorId == alojamiento.ProveedorId);
            if (alojamiento.PuntoInteres != null && alojamiento.PuntoInteres.PuntoInteresId > 0)
                alojamiento.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == alojamiento.PuntoInteres.PuntoInteresId);
            alojamiento.TipoAlojamiento = _context.TipoAlojamientos.First(x => x.TipoAlojamientoId == alojamiento.TipoAlojamientoId);
            alojamiento.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == alojamiento.TipoProductoId);
            if (alojamiento.CategoriaHotelesId > 0)
                alojamiento.CategoriaHoteles = _context.CategoriaHoteles.First(x => x.CategoriaHotelesId == alojamiento.CategoriaHotelesId);
            _context.Alojamientos.Add(alojamiento);
            await _context.SaveChangesAsync();
            alojamiento.ListaPlanesAlimenticios = _context.AlojamientosPlanesAlimenticios.Include(x => x.PlanesAlimenticios).Where(x => x.ProductoId == alojamiento.ProductoId).ToList();
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




        // GET: api/Alojamientoes/FiltrosCount
        [HttpGet]
        [Route("FiltrosCount")]
        public IEnumerable<Contrato> GetContratosByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos

                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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

               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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

                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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

                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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


        // GET: api/Alojamientoes/Filtros
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
                .Where(a => a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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

                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId && x.ContratoDelPrecio.ContratoId == contrato.ContratoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
                                    }



                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas.Include(x => x.Producto)
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
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                           .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId && x.ContratoDelPrecio.ContratoId == contrato.ContratoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
                                    }

                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas.Include(x => x.Producto)
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
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                   .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId && x.ContratoDelPrecio.ContratoId == contrato.ContratoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }
                                    }

                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas.Include(x => x.Producto)
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
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
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
                                contrato.Temporadas[i].ListaPrecioAlojamientos = _context.PrecioAlojamiento
                                    .Include(x => x.Habitacion).Include(x => x.TipoHabitacion)
                                   .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioPlanes = new List<PrecioPlanesAlimenticios>();
                                if (contrato.Distribuidor.ListaProductosDistribuidos != null && contrato.Distribuidor.ListaProductosDistribuidos.Count() > 0)
                                    foreach (var item in contrato.Distribuidor.ListaProductosDistribuidos)
                                    {
                                        if (_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == item.ProductoId))
                                        {
                                            List<PrecioPlanesAlimenticios> temp = new List<PrecioPlanesAlimenticios>();
                                            temp = _context.PrecioPlanesAlimenticios.Where(x => x.ProductoId == item.ProductoId && x.ContratoDelPrecio.ContratoId == contrato.ContratoId).ToList();
                                            contrato.Temporadas[i].ListaPrecioPlanes.AddRange(temp);
                                        }


                                    }

                                contrato.Temporadas[i].ListaFechasTemporada = _context.RangoFechas.Include(x => x.Producto)
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
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                    .ThenInclude(x => x.TipoProducto).Include(x => x.Producto.ListaPlanesAlimenticios)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
                    .Include(x => x.Producto.ListaPlanesAlimenticios)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
                    .Include(x => x.Producto.ListaPlanesAlimenticios)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
                    .Include(x => x.Producto.ListaPlanesAlimenticios)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
        }

        public int CantidadProductoProveedor(int idProveedor, int idProducto, Contrato contrato)
        {

            if (idProducto == 0 && idProveedor == 0)
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION).Count();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.ProductoId == idProducto).Count();

            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor).Count();

            }
            else
            {
                return _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto).Count();

            }
        }


        // Post: api/Alojamientoes/Activar
        [HttpPost]
        [Route("Activar")]
        public async Task<IActionResult> PostAcivarAlojamientoes([FromBody] Alojamiento al)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            Alojamiento a = _context.Alojamientos.Where(x => x.ProductoId == al.ProductoId).Single();
            if (al.IsActivo)
                if (!_context.PrecioAlojamiento.Any(x => x.ProductoId == a.ProductoId) ||
                    !_context.PrecioPlanesAlimenticios.Any(x => x.ProductoId == a.ProductoId))
                {

                    return CreatedAtAction("ActivarAlojamiento", new { id = -1, error = "Este producto no está listo para activar. Revise los precios" }, new { id = -1, error = "Este producto no está listo para activar. Revise los precios" });
                }

            a.IsActivo = al.IsActivo;

            _context.Entry(a).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlojamientoExists(a.ProductoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAlojamoiento", new { id = a.ProductoId }, a);
        }




        // POST: api/Alojamientoes/BuscarOrden
        [HttpPost]
        [Route("BuscarOrden")]
        public List<Alojamiento> GetBuscarAlojamientosByFiltros([FromBody] BuscadorAlojamiento buscador)
        {


            //Se buscan todos los alojamientos segun los parametros
            List<Alojamiento> alojamientos = _context.Alojamientos.Include(x => x.PuntoInteres).Where(x => x.IsActivo && x.PuntoInteres.RegionId == buscador.Region.RegionId).ToList();

            //Se filtra segun los parametros pasados
            if (!string.IsNullOrEmpty(buscador.NombreHotel))
            {
                alojamientos = alojamientos.Where(x => x.Nombre.Contains(buscador.NombreHotel, StringComparison.OrdinalIgnoreCase)).ToList();
            }



            if (buscador.CantidadEstrellas > 0 && alojamientos != null && alojamientos.Any())
            {
                alojamientos = alojamientos.Where(x => x.NumeroEstrellas == buscador.CantidadEstrellas).ToList();
            }

            if (buscador.TipoAlojamiento != null && alojamientos != null && alojamientos.Any())
            {
                alojamientos = alojamientos.Where(x => x.TipoAlojamientoId == buscador.TipoAlojamiento.TipoAlojamientoId).ToList();
            }


            foreach (var a in alojamientos)
            {


                //Se buscan los precios correspondientes 
                List<PrecioAlojamiento> precios = _context.PrecioAlojamiento.Include(x => x.Temporada.ListaFechasTemporada)

                .Where(x => x.ProductoId == a.ProductoId && x.Temporada.ListaFechasTemporada.Any(xx => (xx.FechaInicio <= buscador.Entrada && buscador.Entrada <= xx.FechaFin) ||
                   ((xx.FechaInicio <= buscador.Salida && buscador.Salida <= xx.FechaFin)))).ToList();
                a.PrecioInicial = 0;
                var i = 0;
                foreach (var p in precios)
                {

                    //   if (p.Temporada.ListaFechasTemporada.Any(x => (x.FechaInicio <= buscador.Entrada && buscador.Entrada <= x.FechaFin) ||
                    //   ((x.FechaInicio <= buscador.Salida && buscador.Salida <= x.FechaFin)))) // si la fecha buscada esta en el rango de precios
                    //  {
                    if (i == 0)
                    {
                        i = 1;
                        a.PrecioInicial = p.Precio;
                        continue;
                    }

                    if (a.PrecioInicial > p.Precio)
                    {
                        a.PrecioInicial = p.Precio;
                    }

                    //     }

                }

            }
            if (buscador.OrdenarAsc)
            {
                return alojamientos.OrderBy(x => x.PrecioInicial).ToList();
            }
            else
            {
                return alojamientos.OrderByDescending(x => x.PrecioInicial).ToList();
            }

        }




        // POST: api/Alojamientoes/BuscarOrdenPrecio
        [HttpPost]
        [Route("BuscarOrdenPrecio")]
        public List<OrdenAlojamiento> GetPrecioOrdenAlojamientos([FromBody] BuscadorAlojamientoV2 buscador)
        {


            List<OrdenAlojamiento> lista = new List<OrdenAlojamiento>(); //Lista  a devolver (candidatos)


            //Se buscan todos los alojamientos segun los parametros
            Alojamiento alojamiento = _context.Alojamientos.Include(x => x.ListaDistribuidoresProducto).First(x => x.ProductoId == buscador.Alojamiento.ProductoId);

            foreach (var dist in alojamiento.ListaDistribuidoresProducto)
            {


                //Se buscan los precios correspondientes 
                List<PrecioAlojamiento> preciosTemp = _context.PrecioAlojamiento.Include(x => x.Temporada.ListaFechasTemporada)
                .Include(x => x.Temporada.Contrato.Distribuidor)
                .Include(x => x.Habitacion)
                .Include(x => x.TipoHabitacion)
                .Where(x => x.ProductoId == alojamiento.ProductoId && x.Temporada.Contrato.DistribuidorId == dist.DistribuidorId &&
                x.Temporada.ListaFechasTemporada.Any(xx => (xx.FechaInicio <= buscador.Entrada && buscador.Entrada <= xx.FechaFin) ||
                    ((xx.FechaInicio <= buscador.Salida && buscador.Salida <= xx.FechaFin)))).ToList();


                if (buscador.TipoHabitacion != null && preciosTemp.Any())
                {
                    preciosTemp = preciosTemp.Where(x => x.TipoHabitacion.TipoHabitacionId == buscador.TipoHabitacion.TipoHabitacionId).ToList();
                }

                if (!string.IsNullOrEmpty(buscador.NombreHabitacion) && preciosTemp.Any())
                {
                    preciosTemp = preciosTemp.Where(x => x.Habitacion.Nombre.Contains(buscador.NombreHabitacion, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                //Se buscan las habitaciones que tiene el hotel para buscar luego los precios disponibles para ese tipo y poder iterar sobre esos precios
                //y obtener un posible precio de varias temporadas para una a=habitacion
                List<Habitacion> habitacionesEnHotel = _context.Habitaciones.Where(x => x.ProductoId == alojamiento.ProductoId).ToList();

                foreach (var hab in habitacionesEnHotel)
                {
                    OrdenAlojamiento ov = new OrdenAlojamiento();
                    ov.ListaPrecioAlojamientos = new List<OrdenAlojamientoPrecioAlojamiento>();

                    Cliente c = _context.Clientes.First(x => x.ClienteId == buscador.Cliente.ClienteId); //Cliente que hace la peticion para calcularle su descuento o sobrecargar
                    ov.Alojamiento = alojamiento;
                    ov.Checkin = buscador.Entrada;
                    ov.Checkout = buscador.Salida;
                    ov.FechaInicio = buscador.Entrada;
                    ov.FechaFin = buscador.Salida;
                    int cantDiasGenenarl = (buscador.Salida - buscador.Entrada).Days; //Cant. de dias a reservar
                    int DiasRestantes = cantDiasGenenarl; // para saber que cantidad de dias son extra a las restricciones
                    int cantDias = 0; // auxilar para rangos
                    bool agregarOrden = true;
                    decimal precioBase = 0;

                    List<PrecioAlojamiento> precios = new List<PrecioAlojamiento>();
                    precios = preciosTemp.Where(x => x.Habitacion.HabitacionId == hab.HabitacionId).ToList();


                    if (!precios.Any())
                        continue;
                    foreach (var p in precios)
                    {

                        OrdenAlojamientoPrecioAlojamiento ovpra = new OrdenAlojamientoPrecioAlojamiento();

                        ovpra.PrecioAlojamiento = p;
                        ov.ListaPrecioAlojamientos.Add(ovpra);

                        ov.Distribuidor = p.Temporada.Contrato.Distribuidor;


                        //Variables para ir viendo que 
                        int cantAdultosAux = buscador.CantidadAdultos;
                        int cantNinoAux = buscador.CantidadMenores;
                        int cantInfanteAux = buscador.CantidadInfantes;

                        //Se buscan los modficadores de precio para el alojamineto
                        List<Modificador> modificadores = GetModificadores(alojamiento, p);

                        precioBase = p.Precio;
                        try
                        {

                            switch (p.Temporada.Contrato.FormaCobro)// 2-por dia 1- PrimeraTemp 3-UltimaTemp                           
                            {
                                case 2:
                                    {
                                        Met_CalcularPrecioAlojamientoPorDia(buscador, p, ov, cantDiasGenenarl, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, alojamiento, ref DiasRestantes);
                                        break;
                                    }
                                case 1:
                                    {
                                        Met_CalcularPrecioAlojamientoPrimeraTemporada(buscador, p, ov, cantDiasGenenarl, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, alojamiento, ref DiasRestantes);

                                        break;
                                    }
                                case 3:
                                    {
                                        Met_CalcularPrecioAlojamientoPorSegundaTemporada(buscador, p, ov, cantDiasGenenarl, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, alojamiento, ref DiasRestantes);

                                        break;
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }

                        }
                        catch (Exception e)
                        {
                            agregarOrden = false;
                            break;
                        }

                        if (!agregarOrden)
                            continue;





                    }

                    if (DiasRestantes > 0)
                    {
                        ov.PrecioOrden += DiasRestantes * precioBase * (buscador.CantidadAdultos + buscador.CantidadMenores + buscador.CantidadInfantes);

                    }



                    List<PrecioPlanesAlimenticios> preciosPlanesAlimen = _context.PrecioPlanesAlimenticios.OrderByDescending(x => x.Precio).Where(x => x.ProductoId == alojamiento.ProductoId &&
                        x.PlanesAlimenticiosId == buscador.PlanAlimenticio.PlanesAlimenticiosId).ToList();

                    if (preciosPlanesAlimen != null && preciosPlanesAlimen.Any())
                        ov.PrecioOrden += preciosPlanesAlimen.Sum(x => x.Precio)*(buscador.CantidadAdultos + buscador.CantidadMenores + buscador.CantidadInfantes);

                    //Se aplica la ganancia correspondiente
                    List<Sobreprecio> sobreprecios = _context.Sobreprecio.Where(x => x.TipoProducto.Nombre == ValoresAuxiliares.TRANSPORTATION).ToList();
                    foreach (var s in sobreprecios)
                    {
                        if (s.PrecioDesde <= ov.PrecioOrden && ov.PrecioOrden <= s.PrecioHasta)
                        {
                            ov.Sobreprecio = s;
                            decimal valorAplicado = 0;
                            if (s.PagoPorDia)
                            {
                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = cantDiasGenenarl * (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + ((decimal)s.ValorDinero * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = cantDiasGenenarl * ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                                }

                            }
                            else
                            {

                                if (s.ValorDinero != null)
                                {
                                    valorAplicado = (decimal)s.ValorDinero;
                                    ov.PrecioOrden += valorAplicado + ((decimal)s.ValorDinero * c.Descuento / 100);
                                }
                                else
                                {
                                    valorAplicado = ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100);
                                    ov.PrecioOrden += valorAplicado + (ov.PrecioOrden * ((decimal)s.ValorPorCiento / 100) * c.Descuento / 100);
                                }

                            }
                            ov.ValorSobreprecioAplicado = valorAplicado;
                            break;
                        }

                    }


                    if (agregarOrden)
                        lista.Add(ov);
                }

            }




            return lista.OrderByDescending(x => x.PrecioOrden).ToList();

        }

        private static void Met_CalcularPrecioAlojamientoPrimeraTemporada(BuscadorAlojamientoV2 buscador, PrecioAlojamiento p, OrdenAlojamiento ov, int cantDiasGenenarl, ref int cantDias, ref int cantAdultosAux, ref int cantNinoAux, ref int cantInfanteAux, List<Modificador> modificadores, Alojamiento a, ref int DiasRestantes)
        {

            int i = 0;
            bool seCalcValor = false; //para saber si se calculo el valor
            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.Where(x => x.Producto.ProductoId == a.ProductoId).OrderBy(x => x.FechaInicio).ToList();
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count)
            {
                RangoFechas rf = listaRangos[i];
                Modificador md = new Modificador();// Se usara para saber si aplica la modificacion
                decimal precioBase = p.Precio; // Precio base de la habitacion
                if (rf.FechaInicio <= buscador.Entrada && buscador.Entrada <= rf.FechaFin)
                {
                    //Si el el rago de la reserva cae completamente en un rango con la cantidad de dias general se calcula el precio
                    cantDias = cantDiasGenenarl;
                    DiasRestantes -= cantDias;
                    GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);
                    seCalcValor = true;
                }




                i++;
            }
            if (!seCalcValor) throw new Exception();
        }

        private static void Met_CalcularPrecioAlojamientoPorSegundaTemporada(BuscadorAlojamientoV2 buscador, PrecioAlojamiento p, OrdenAlojamiento ov, int cantDiasGenenarl, ref int cantDias, ref int cantAdultosAux, ref int cantNinoAux, ref int cantInfanteAux, List<Modificador> modificadores, Alojamiento a, ref int DiasRestantes)
        {

            int i = 0;
            bool seCalcValor = false; //para saber si se calculo el valor
            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.Where(x => x.Producto.ProductoId == a.ProductoId).OrderBy(x => x.FechaInicio).ToList();
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count)
            {
                RangoFechas rf = listaRangos[i];
                Modificador md = new Modificador();// Se usara para saber si aplica la modificacion
                decimal precioBase = p.Precio; // Precio base de la habitacion
                if (rf.FechaInicio <= buscador.Salida && buscador.Salida <= rf.FechaFin)
                {
                    //Si el el rago de la reserva cae completamente en un rango con la cantidad de dias general se calcula el precio
                    cantDias = cantDiasGenenarl;
                    DiasRestantes -= cantDias;
                    GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);
                    seCalcValor = true;
                }



                i++;
            }
            if (!seCalcValor) throw new Exception();
        }


        private static void Met_CalcularPrecioAlojamientoPorDia(BuscadorAlojamientoV2 buscador, PrecioAlojamiento p, OrdenAlojamiento ov, int cantDiasGenenarl, ref int cantDias, ref int cantAdultosAux, ref int cantNinoAux, ref int cantInfanteAux, List<Modificador> modificadores, Alojamiento a, ref int DiasRestantes)
        {
            int i = 0;

            List<RangoFechas> listaRangos = p.Temporada.ListaFechasTemporada.Where(x => x.Producto != null && x.Producto.ProductoId == a.ProductoId).OrderBy(x => x.FechaInicio).ToList();
            decimal precioBase = p.Precio; // Precio base de la habitacion
            //Recorro todas los rangos de fecha para ir calculando precio total
            while (i < listaRangos.Count && cantDiasGenenarl > 0)
            {
                RangoFechas rf = listaRangos[i];
                Modificador md = new Modificador();// Se usara para saber si aplica la modificacion


                if (rf.FechaInicio <= buscador.Entrada && buscador.Entrada <= rf.FechaFin &&
                  rf.FechaFin >= buscador.Salida && buscador.Salida >= rf.FechaInicio)
                {
                    //Si el el rago de la reserva cae completamente en un rango con la cantidad de dias general se calcula el precio
                    cantDias = cantDiasGenenarl;
                    DiasRestantes -= cantDias;
                    GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);
                    cantDiasGenenarl = 0;
                    break;
                }
                else
                {

                    if (buscador.Entrada < rf.FechaInicio && rf.FechaFin < buscador.Salida)
                    {
                        //Si el rango esta incluido en el rango de entrada y salida la cantidad de dias sera la diferencia del rango de fecha
                        cantDias = (rf.FechaFin - rf.FechaInicio).Days + 1;
                        GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);
                        cantDiasGenenarl -= cantDias;
                        DiasRestantes -= cantDias;

                    }
                    else
                   if (rf.FechaInicio <= buscador.Entrada && buscador.Entrada < rf.FechaFin)
                    {
                        //Si solo la fecha de recogida cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                        cantDias = (rf.FechaFin - buscador.Entrada).Days;
                        GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);
                        cantDiasGenenarl -= cantDias;
                        DiasRestantes -= cantDias;

                    }
                    else
                   if (rf.FechaFin >= buscador.Salida && buscador.Salida >= rf.FechaInicio)
                    {
                        //Si solo la fecha de Entrega cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                        cantDias = (buscador.Salida - rf.FechaInicio).Days + 1;
                        GetPrecioAlojamientoSegunModificadores(buscador, ov, ref cantDias, ref cantAdultosAux, ref cantNinoAux, ref cantInfanteAux, modificadores, ref md, precioBase, rf);

                        cantDiasGenenarl -= cantDias;
                        DiasRestantes -= cantDias;
                    }


                }






                i++;
            }

            ov.Habitacion = p.Habitacion;
            ov.TipoHabitacion = p.TipoHabitacion;

            /*if (cantDiasGenenarl > 0)
            {
                ov.PrecioOrden += cantDiasGenenarl * precioBase * (buscador.CantidadAdultos + buscador.CantidadMenores + buscador.CantidadInfantes);

            }*/

        }

        private static void GetPrecioAlojamientoSegunModificadores(BuscadorAlojamientoV2 buscador, OrdenAlojamiento ov, ref int cantDias, ref int cantAdultosAux, ref int cantNinoAux, ref int cantInfanteAux, List<Modificador> modificadores, ref Modificador md, decimal precioBase, RangoFechas rf)
        {
            bool encontroMod = false;
            int cantTotaldiasResta = cantDias; //esta variable es para llevar la cantidad de dias que ya se han calculado
            //Por ej si el mod solo aplica a 2 dias dentro del rango se calculan 2 dias y con esta var el resto
            foreach (var item in modificadores) // se evalua por modificadores
            {
                md = item;
                if (md.CantAdult == buscador.CantidadAdultos && md.CantNino == buscador.CantidadMenores
                    && md.CantNino == buscador.CantidadInfantes) // si coincide la cantidad de dias con el rango de una restriccion se calcula
                {

                    if (md.FechaI != null && md.FechaF != null)
                    {
                        if (buscador.Entrada <= md.FechaI && md.FechaI <= buscador.Salida &&
                                              buscador.Salida >= md.FechaF && md.FechaF >= buscador.Entrada)
                        {
                            //Si el el rago de la reserva cae completamente en un rango con la cantidad de dias general se calcula el precio
                            cantDias = ((TimeSpan)(md.FechaF - md.FechaI)).Days;
                            cantTotaldiasResta -= cantDias;
                            encontroMod = true;
                        }
                        else
                        {
                            if (buscador.Entrada < md.FechaI && md.FechaF < buscador.Salida)
                            {
                                //Si el rango esta incluido en el rango de entrada y salida la cantidad de dias sera la diferencia del rango de fecha
                                cantDias = ((TimeSpan)(md.FechaF - md.FechaI)).Days + 1;
                                cantTotaldiasResta -= cantDias;
                                encontroMod = true;
                            }
                            else
                            if (md.FechaI <= buscador.Entrada && buscador.Entrada < md.FechaF)
                            {
                                //Si solo la fecha de recogida cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                                cantDias = ((DateTime)md.FechaF - buscador.Entrada).Days;
                                cantTotaldiasResta -= cantDias;
                                encontroMod = true;
                            }
                            else
                            if (md.FechaF >= buscador.Salida && buscador.Salida >= md.FechaI)
                            {
                                //Si solo la fecha de Entrega cae en rango la cantidad de dias sera la diferencia respecto al fin del rango
                                cantDias = (buscador.Salida - (DateTime)md.FechaI).Days;
                                cantTotaldiasResta -= cantDias;
                                encontroMod = true;
                            }
                        }
                    }
                    if (encontroMod)
                    {
                        List<Reglas> reglas = md.ListaReglas;
                        foreach (var r in reglas)
                        {
                            if (r.TipoPersona.Equals(ValoresAuxiliares.MODFICADOR_TIPOPERSONA_ADULTO) && cantAdultosAux > 0)
                            {
                                cantAdultosAux--;
                                if (r.IsActivo)
                                {
                                    if (r.PrecioPorCiento != 0)
                                    {

                                        ov.PrecioOrden += cantDias * precioBase * r.PrecioPorCiento / 100;
                                    }
                                    else
                                    {
                                        ov.PrecioOrden += cantDias * r.PrecioFijo;
                                    }

                                }
                                else
                                {
                                    ov.PrecioOrden += cantDias * precioBase;
                                }
                                continue;
                            }

                            if (r.TipoPersona.Equals(ValoresAuxiliares.MODFICADOR_TIPOPERSONA_NINO) && cantNinoAux > 0)
                            {
                                cantNinoAux--;
                                if (r.IsActivo)
                                {
                                    if (r.PrecioPorCiento != 0)
                                    {
                                        ov.PrecioOrden += cantDias * precioBase * r.PrecioPorCiento / 100;
                                    }
                                    else
                                    {
                                        ov.PrecioOrden += cantDias * r.PrecioFijo;
                                    }

                                }
                                else
                                {
                                    ov.PrecioOrden += cantDias * precioBase;
                                }
                                continue;
                            }

                            if (r.TipoPersona.Equals(ValoresAuxiliares.MODFICADOR_TIPOPERSONA_INFANTE) && cantInfanteAux > 0)
                            {
                                cantInfanteAux--;
                                if (r.IsActivo)
                                {
                                    if (r.PrecioPorCiento != 0)
                                    {
                                        ov.PrecioOrden += cantDias * precioBase * r.PrecioPorCiento / 100;
                                    }
                                    else
                                    {
                                        ov.PrecioOrden += cantDias * r.PrecioFijo;
                                    }

                                }
                                else
                                {
                                    ov.PrecioOrden += cantDias * precioBase;
                                }
                                continue;
                            }

                        }
                    }


                    break;
                }
            }
            if (!encontroMod)
                ov.PrecioOrden += cantTotaldiasResta * precioBase * (cantNinoAux + cantInfanteAux + cantAdultosAux);


        }

        private List<Modificador> GetModificadores(Alojamiento a, PrecioAlojamiento p)
        {
            List<Modificador> res = new List<Modificador>();
            res = _context.Modificadores.Include(x => x.ListaReglas)
                        .Where(x => x.IsActivo && x.ListaHoteles.Any(e => e.ProductoId == a.ProductoId) &&
                        x.ListaTemporadasAfectadas.Any(e => e.TemporadaId == p.Temporada.TemporadaId)).ToList();


            return res;
        }

       



    }
}