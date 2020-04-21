﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using PagedList;
using GoTravelTour.Utiles;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public VehiculoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Vehiculoes
        [HttpGet]
        public IEnumerable<Vehiculo> GetVehiculos (string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1, int idProveedor=0)
        {
            IEnumerable<Vehiculo> lista;
            if (col == "-1")
            {
                lista= _context.Vehiculos
                    //.Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                   //  .Include(v => v.TipoProducto)
                   //  .Include(v => v.ListaDistribuidoresProducto)
                   .Where(x => x.ProveedorId == idProveedor)
                    .OrderBy(a => a.Nombre)
                    .ToList();
               
                foreach (Vehiculo v in  lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Vehiculos
                    //.Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                  //  .Include(v => v.TipoProducto)
                    //.Include(v => v.ListaDistribuidoresProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
                foreach (Vehiculo v in lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
            }
            else
            {
                lista = _context.Vehiculos
                   // .Include(v => v.Marca)
                   .Include(v => v.Modelo)
                    .Include(v => v.Proveedor)
                   // .Include(v => v.TipoProducto)
                 //   .Include(v => v.ListaDistribuidoresProducto)
                    .ToPagedList(pageIndex, pageSize).ToList();
                foreach (Vehiculo v in lista)
                {
                    v.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == v.ProductoId).ToList();
                }
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
        // GET: api/Vehiculoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetVehiculoCount()
        {
            return _context.Vehiculos.Count();
        }

        // GET: api/Vehiculoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*  var vehiculo = await _context.Vehiculos
                  .FindAsync(id);*/
            var vehiculo =  _context.Vehiculos
               .Include(v => v.Marca)
               .Include(v => v.Modelo)
               .Include(v => v.Proveedor)
               .Include(v => v.TipoProducto)
               .Include(v => v.ListaDistribuidoresProducto)
           .Single(x=>x.ProductoId == id);

            vehiculo.ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();

            if (vehiculo == null)
            {
                return NotFound();
            }

            return Ok(vehiculo);
        }

        // PUT: api/Vehiculoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutVehiculo([FromRoute] int id, [FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehiculo.ProductoId)
            {
                return BadRequest();
            }
            List<VehiculoCategoriaAuto> ListaCategorias = _context.VehiculoCategoriaAuto.Where(x => x.ProductoId == id).ToList();
            int i = 0;
            while (i < ListaCategorias.Count())
            {
                VehiculoCategoriaAuto c = ListaCategorias[i];
                bool esta = false;
                foreach(VehiculoCategoriaAuto c2 in vehiculo.ListaCategorias)
                {
                    if(c.VehiculoCategoriaAutoId == c2.VehiculoCategoriaAutoId)
                    {
                        esta = true;
                        break;
                    }
                   
                }
                if (!esta && c.VehiculoCategoriaAutoId != 0)
                {
                    _context.VehiculoCategoriaAuto.Remove(c);
                    ListaCategorias.Remove(c);
                    i--;
                }
               
                    i++;
               
                
            }
            await _context.SaveChangesAsync();
            i = 0;
            while (i < vehiculo.ListaCategorias.Count)
            {
                VehiculoCategoriaAuto c = vehiculo.ListaCategorias[i];
                if (c.VehiculoCategoriaAutoId == 0)
                {
                    c.ProductoId = vehiculo.ProductoId;
                    _context.VehiculoCategoriaAuto.Add(c);
                    await _context.SaveChangesAsync();
                }
                else
                {
                   /* _context.Entry(c).State = EntityState.Modified;
                    await _context.SaveChangesAsync();*/

                }
                i++;
            }
            List<ComodidadesProductos> comodidades = _context.ComodidadesProductos.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();
            foreach (var item in comodidades)
            {
                _context.ComodidadesProductos.Remove(item);
            }

            List<ProductoDistribuidor> distribuidors = _context.ProductoDistribuidores.Where(x => x.ProductoId == vehiculo.ProductoId).ToList();
            foreach (var item in distribuidors)
            {
                _context.ProductoDistribuidores.Remove(item);
            }
            if (vehiculo.ListaDistribuidoresProducto != null)
            foreach (var item in vehiculo.ListaDistribuidoresProducto)
            {
                item.ProductoId = vehiculo.ProductoId;
                _context.ProductoDistribuidores.Add(item);
            }
            if (vehiculo.ListaComodidades != null)
                foreach (var item in vehiculo.ListaComodidades)
            {
                item.ProductoId = vehiculo.ProductoId;
                _context.ComodidadesProductos.Add(item);
            }

            vehiculo.Proveedor = _context.Proveedores.First(x => x.ProveedorId == vehiculo.ProveedorId);
            if (vehiculo.PuntoInteres != null)
                vehiculo.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == vehiculo.PuntoInteres.PuntoInteresId);
            vehiculo.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == vehiculo.TipoProductoId);
            vehiculo.Marca = _context.Marcas.First(x => x.MarcaId == vehiculo.MarcaId);
            vehiculo.Modelo = _context.Modelos.First(x => x.ModeloId == vehiculo.ModeloId);
            


            _context.Entry(vehiculo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // POST: api/Vehiculoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostVehiculo([FromBody] Vehiculo vehiculo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Utiles.Utiles u = new Utiles.Utiles(_context);
            vehiculo.SKU = u.GetSKUCodigo();
            vehiculo.Proveedor = _context.Proveedores.First(x => x.ProveedorId == vehiculo.ProveedorId);
            if (vehiculo.PuntoInteres != null)
                vehiculo.PuntoInteres = _context.PuntosInteres.First(x => x.PuntoInteresId == vehiculo.PuntoInteres.PuntoInteresId);
            vehiculo.TipoProducto = _context.TipoProductos.First(x => x.TipoProductoId == vehiculo.TipoProductoId);
            vehiculo.Marca = _context.Marcas.First(x => x.MarcaId == vehiculo.MarcaId);
            vehiculo.Modelo = _context.Modelos.First(x => x.ModeloId == vehiculo.ModeloId);


            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();
            
            if(vehiculo.ListaCategorias.Count() > 0)
            {
                foreach (VehiculoCategoriaAuto vca in vehiculo.ListaCategorias)
                {
                    VehiculoCategoriaAuto temp = new VehiculoCategoriaAuto();
                    temp.CategoriaAutoId = vca.CategoriaAutoId;
                    temp.ProductoId = vehiculo.ProductoId;
                    _context.VehiculoCategoriaAuto.Add(temp);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetVehiculo", new { id = vehiculo.ProductoId }, vehiculo);
        }

        // DELETE: api/Vehiculoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehiculo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return Ok(vehiculo);
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.ProductoId == id);
        }


        // GET: api/Vehiculoes/FiltrosCount
        [HttpGet]
        [Route("FiltrosCount")]
        public IEnumerable<Contrato> GetContratosByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                lista = _context.Contratos
               
                .Where(a => a.TipoProducto.Nombre == "Vehicle")
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
              
               .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Vehicle")
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
              
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == "Vehicle")
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
               
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Vehicle")
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


        // GET: api/Vehiculoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(int idContrato = -1, int idDistribuidor = -1, int idProveedor=0, int idProducto=0, int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Contrato> lista= new  List<Contrato>();            

            if (idContrato == -1 && idDistribuidor == -1)
            {
                
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a =>a.TipoProducto.Nombre == "Vehicle")
                .OrderBy(a => a.Nombre)
                .ToList();
                if (lista.Count() > 0)
                    foreach (var contrato in lista)
                    {

                        contrato.CantidadProductosTotal = CantidadProductoProveedor(idProveedor, idProducto, contrato);
                        FiltrarProductoProveedor(idProveedor, idProducto, contrato, pageIndex, pageSize);

                        if ( contrato.Temporadas != null && contrato.Temporadas.Count() > 0)
                        {
                            int i = 0;
                            while (i < contrato.Temporadas.Count())
                            {
                                contrato.Temporadas[i].ListaRestricciones = _context.Restricciones
                                    .Where(x => x.Temporada.TemporadaId == contrato.Temporadas[i].TemporadaId).ToList();
                                ;
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
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
                .Where(a => a.ContratoId == idContrato && a.DistribuidorId == idDistribuidor &&  a.TipoProducto.Nombre == "Vehicle")
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
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
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
              if (idContrato != -1 && idDistribuidor == -1 )
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.ContratoId == idContrato && a.TipoProducto.Nombre == "Vehicle")
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
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
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
              if (idContrato == -1 && idDistribuidor != -1 )
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                .Include(a => a.Temporadas)
                .Where(a => a.DistribuidorId == idDistribuidor && a.TipoProducto.Nombre == "Vehicle")
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
                                contrato.Temporadas[i].ListaPrecioAutos = _context.PrecioRentaAutos
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
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle")
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.Producto.ProveedorId == idProveedor)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
        }

        private int CantidadProductoProveedor(int idProveedor, int idProducto, Contrato contrato)
        {
            
            if (idProducto == 0 && idProveedor == 0)
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle").Count();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                return _context.ProductoDistribuidores
                .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.ProductoId == idProducto).Count();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                return _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.Producto.ProveedorId == idProveedor).Count();
            }
            else
            {
                return _context.ProductoDistribuidores
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == "Vehicle" && x.Producto.ProveedorId == idProveedor && x.ProductoId == idProducto).Count();
            }
        }

               


        // GET: api/Vehiculoes/BuscarOrden
        [HttpGet]
        [Route("BuscarOrden")]
        public List<OrdenVehiculo> GetOrdenVehiculos(BuscadorVehiculo buscador)
        {
            // TODO agregar el calculo de la orden teniendo en  cuenta
            /// restriccionen
            /// surcharger
            /// surcharge cliente
            /// deposito seguro extra
            /// 
            List<OrdenVehiculo> lista = new List<OrdenVehiculo>();

            List<Vehiculo> vehiculos =  _context.Vehiculos.Where(x => x.ListaCategorias.Any(y => y.CategoriaAuto.CategoriaAutoId == buscador.CategoriaAuto.CategoriaAutoId)
            && x.TipoTransmision == buscador.TipoTransmision).ToList();
           
            foreach (var v in vehiculos)
            {
                List<PrecioRentaAutos> precios = _context.PrecioRentaAutos.Include(x=>x.Temporada.ListaFechasTemporada)
                    .Include(x => x.Temporada.Contrato.Distribuidor)
                    .Where(x => x.ProductoId == v.ProductoId).ToList();
                foreach(var p in precios)
                {
                    OrdenVehiculo ov = new OrdenVehiculo();
                    if (p.Temporada.ListaFechasTemporada.Any(x=>(x.FechaInicio <= buscador.FechaRecogida && buscador.FechaRecogida <= x.FechaFin) || 
                     (x.FechaFin >= buscador.FechaEntrega && buscador.FechaEntrega >= x.FechaInicio)))
                    {
                        ov.PrecioRentaAutos = p;
                        ov.Distribuidor = p.Temporada.Contrato.Distribuidor;
                        ov.Vehiculo = v;
                        
                        lista.Add(ov);
                    }

                }
            }


            return lista.OrderByDescending(x=>x.PrecioRentaAutos.Deposito).ToList();

        }



    }
}