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
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION)
                .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto != 0 && idProveedor == 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.ProductoId == idProducto)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else if ((idProducto == 0 && idProveedor != 0))
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
               .Where(x => x.DistribuidorId == contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor)
               .ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto).ThenInclude(x => x.TipoProducto)
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
                if (!_context.PrecioActividad.Any(x => x.ProductoId == a.ProductoId) ||
                    !_context.RestriccionesPrecios.Any(x => x.ProductoId == a.ProductoId))
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

    }
}