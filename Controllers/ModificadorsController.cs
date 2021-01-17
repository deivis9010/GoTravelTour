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
    public class ModificadorsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ModificadorsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Modificadors
        [HttpGet]
        public IEnumerable<Modificador> GetModificadores(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Modificador> lista;
            if (col == "-1")
            {
                lista = _context.Modificadores
                      .Include(a => a.Contrato)
                      .Include(a => a.Contrato.Distribuidor)
                      .Include(a => a.Contrato.Temporadas)

                       .Include(a => a.ListaHoteles)
                       .Include(a => a.ListaReglas)
                       .Include(a => a.Proveedor)
                    .OrderBy(a => a.IdentificadorModificador)
                    .ToList();

                if (lista.Count() > 0)
                    foreach (Modificador mod in lista)
                    {
                        mod.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                             .Include(x => x.Producto).Where(b => b.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && b.DistribuidorId == mod.Contrato.Distribuidor.DistribuidorId).ToList();
                    }


                return lista;
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Modificadores
                      .Include(a => a.Contrato)
                       .Include(a => a.Contrato.Distribuidor)
                   .Include(a => a.Contrato.Temporadas)
                   .Include(a => a.ListaHoteles)
                   .Include(a => a.ListaReglas)
                   .Include(a => a.Proveedor)

                    .OrderBy(a => a.IdentificadorModificador)

                    .Where(p => (p.IdentificadorModificador.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
                if (lista.Count() > 0)
                    foreach (Modificador mod in lista)
                    {
                        mod.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto).Where(b => b.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && b.DistribuidorId == mod.Contrato.Distribuidor.DistribuidorId).ToList();
                    }
            }
            else
            {

                lista = _context.Modificadores
                      .Include(a => a.Contrato)
                       .Include(a => a.Contrato.Distribuidor)
                       .Include(a => a.Contrato.Temporadas)
                       .Include(a => a.ListaHoteles)
                       .Include(a => a.ListaReglas)
                       .Include(a => a.Proveedor)
                    .OrderBy(a => a.IdentificadorModificador)

                    .ToPagedList(pageIndex, pageSize).ToList();

                if (lista.Count() > 0)
                    foreach (Modificador mod in lista)
                    {
                        mod.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores
                            .Include(x => x.Producto).Where(b => b.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && b.DistribuidorId == mod.Contrato.Distribuidor.DistribuidorId).ToList();
                    }

            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.IdentificadorModificador);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.IdentificadorModificador);

                        }
                    }

                    break;
            }


            return lista;
        }

      

        // GET: api/Modificadors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModificador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificador =  _context.Modificadores           
                  .Include(a => a.Contrato)
                  .Include(a => a.Contrato.Distribuidor)
                  .Include(a => a.Contrato.Temporadas)
                  .Include(a => a.ListaHoteles)
                  .Include(a => a.Proveedor)
                  .Include(a => a.ListaTemporadasAfectadas)
                  .Where(a=>a.ModificadorId == id)
                  
                  .First();
            if (modificador == null)
            {
                return NotFound();
            }

            modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                   .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION ).ToList();
                
                modificador.ListaReglas = _context.Reglas
                   .Include(x => x.TipoHabitacion)
                  .Where(b => b.ModificadorId == modificador.ModificadorId)
                  .ToList();
            

           

            return Ok(modificador);
        }

        // PUT: api/Modificadors/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutModificador([FromRoute] int id, [FromBody] Modificador modificador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != modificador.ModificadorId)
            {
                return BadRequest();
            }

            modificador.Proveedor = _context.Proveedores.First(x => x.ProveedorId == modificador.Proveedor.ProveedorId);
            modificador.Contrato = _context.Contratos
               .Include(x => x.Distribuidor)
               .Single(x => x.ContratoId == modificador.Contrato.ContratoId);
            modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                .Where(b => b.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && b.DistribuidorId == modificador.Contrato.Distribuidor.DistribuidorId)
                .ToList();

            //Eliminando reglas anteriores
            List<Reglas> regAnteriores = _context.Reglas.Where(x => x.ModificadorId == modificador.ModificadorId).ToList();
            foreach (var item in regAnteriores)
            {
                _context.Reglas.Remove(item);
            }

            if (modificador.ListaReglas != null)
            {
                int i = 0;
                while (i < modificador.ListaReglas.Count())
                {
                    _context.Reglas.Add(modificador.ListaReglas[i]);
                    i++;
                }
            }
            //Eliminando productos anteriores
            List<ModificadorProductos> prodAnteriores = _context.ModificadorProductos.Where(x => x.ModificadorId == modificador.ModificadorId).ToList();
            foreach (var item in prodAnteriores)
            {
                _context.ModificadorProductos.Remove(item);
            }

            if (modificador.ListaHoteles != null)
            {
                int i = 0;
                while (i < modificador.ListaHoteles.Count())
                {
                    _context.ModificadorProductos.Add(modificador.ListaHoteles[i]);
                    i++;
                }
            }
            _context.Entry(modificador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModificadorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(modificador);
        }

        // POST: api/Modificadors
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostModificador([FromBody] Modificador modificador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            modificador.Proveedor = _context.Proveedores.First(x => x.ProveedorId == modificador.Proveedor.ProveedorId);
            modificador.Contrato = _context.Contratos
                .Include(x => x.Distribuidor)
                .Single(x => x.ContratoId == modificador.Contrato.ContratoId);

            modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                .Where(b => b.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && b.DistribuidorId == modificador.Contrato.Distribuidor.DistribuidorId)
                .ToList();




            _context.Modificadores.Add(modificador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModificador", new { id = modificador.ModificadorId }, modificador);
        }

        // DELETE: api/Modificadors/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteModificador([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var modificador = await _context.Modificadores.FindAsync(id);
            if (modificador == null)
            {
                return NotFound();
            }

            var reglas = _context.Reglas.Where(x => x.ModificadorId == modificador.ModificadorId).ToList();
            if (reglas != null)
                foreach (var item in reglas)
                {
                    _context.Reglas.Remove(item);

                }
            var prodmod = _context.ModificadorProductos.Where(x => x.ModificadorId == modificador.ModificadorId).ToList();
            if (prodmod != null)
                foreach (var item in prodmod)
                {
                    _context.ModificadorProductos.Remove(item);

                }

            var tempmod = _context.ModificadorTemporada.Where(x => x.ModificadorId == modificador.ModificadorId).ToList();
            if (tempmod != null)
                foreach (var item in tempmod)
                {
                    _context.ModificadorTemporada.Remove(item);

                }
            _context.SaveChanges();
            _context.Modificadores.Remove(modificador);
            await _context.SaveChangesAsync();

            return Ok(modificador);
        }

        private bool ModificadorExists(int id)
        {
            return _context.Modificadores.Any(e => e.ModificadorId == id);
        }


        // GET: api/Modificadors/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Modificador> GetModificadorByFiltros(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Modificador> lista = new List<Modificador>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
                

                .Include(a => a.Proveedor)
               
                .Where(x => x.Proveedor.ProveedorId == idProveedor)
                .OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                {
                    lista = _context.Modificadores
               

                .OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }

               


                return lista;
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
               .Include(a => a.Contrato)
              
               .Include(a => a.Proveedor)
              
               .Where(a => a.Contrato.ContratoId == idContrato && a.Contrato.Distribuidor.DistribuidorId == idDistribuidor
               && a.Proveedor.ProveedorId == idProveedor)
               .OrderBy(a => a.IdentificadorModificador)
               .ToList();
                }
                else
                    lista = _context.Modificadores
                   .Include(a => a.Contrato)
                   .Include(a => a.Contrato.Distribuidor)
                  
                   
                   .Where(a => a.Contrato.ContratoId == idContrato && a.Contrato.Distribuidor.DistribuidorId == idDistribuidor)
                   .OrderBy(a => a.IdentificadorModificador)
                   .ToList();

               

            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
                .Include(a => a.Contrato)
                
                .Include(a => a.Proveedor)
                 
                 .Where(a => a.Contrato.ContratoId == idContrato && a.Proveedor.ProveedorId == idProveedor)
                .OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                    lista = _context.Modificadores
                    .Include(a => a.Contrato)
                    

                     .Where(a => a.Contrato.ContratoId == idContrato)
                    .OrderBy(a => a.IdentificadorModificador)
                    .ToList();


              

                return lista;
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
               
               .Include(a => a.Contrato.Distribuidor)
             
               .Include(a => a.Proveedor)
               
                .Where(a => a.Contrato.DistribuidorId == idDistribuidor && idProveedor == a.Proveedor.ProveedorId)
                .OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                    lista = _context.Modificadores
                  
                   .Include(a => a.Contrato.Distribuidor)
                 
                    .Where(a => a.Contrato.DistribuidorId == idDistribuidor)
                    .OrderBy(a => a.IdentificadorModificador)
                    .ToList();
              

                return lista;
            }


            return lista;

        }





        // GET: api/Modificadors/Filtros
        [HttpGet]
        [Route("Count")]
        public int GetModificadorByFiltrosCount(int idContrato = -1, int idDistribuidor = -1, int idProveedor = 0, int idProducto = 0)
        {
            IEnumerable<Modificador> lista = new List<Modificador>();

            if (idContrato == -1 && idDistribuidor == -1)
            {

                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
                //.Include(a => a.Contrato)
                //.Include(a => a.Contrato.Distribuidor)
                //.Include(a => a.Contrato.Temporadas)
                //.Include(a => a.ListaHoteles)

                .Include(a => a.Proveedor)
                //.Include(a => a.ListaTemporadasAfectadas)
                .Where(x => x.Proveedor.ProveedorId == idProveedor)
               // .OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                {
                    lista = _context.Modificadores
                //.Include(a => a.Contrato)
                //.Include(a => a.Contrato.Distribuidor)
                //.Include(a => a.Contrato.Temporadas)
                //.Include(a => a.ListaHoteles)

                //.Include(a => a.Proveedor)
                //.Include(a => a.ListaTemporadasAfectadas)

                //.OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }

                //foreach (Modificador modificador in lista)
                //{

                //    if (idProducto == 0)
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor).ToList();
                //    }
                //    else
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                //    }


                //    modificador.ListaReglas = _context.Reglas
                //        .Include(x => x.TipoHabitacion)
                //       .Where(b => b.ModificadorId == modificador.ModificadorId)
                //       .ToList();
                //}



                return lista.Count();
            }
            else
              if (idContrato != -1 && idDistribuidor != -1)
            {

                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
               //.Include(a => a.Contrato)
               .Include(a => a.Contrato.Distribuidor)
               //.Include(a => a.Contrato.Temporadas)
               //.Include(a => a.ListaHoteles)
               .Include(a => a.Proveedor)
               //.Include(a => a.ListaTemporadasAfectadas)
               .Where(a => a.Contrato.ContratoId == idContrato && a.Contrato.Distribuidor.DistribuidorId == idDistribuidor
               && a.Proveedor.ProveedorId == idProveedor)
               //.OrderBy(a => a.IdentificadorModificador)
               .ToList();
                }
                else
                    lista = _context.Modificadores
                  // .Include(a => a.Contrato)
                   .Include(a => a.Contrato.Distribuidor)
                 //  .Include(a => a.Contrato.Temporadas)
                 ///  .Include(a => a.ListaHoteles)
                   .Include(a => a.Proveedor)
                  // .Include(a => a.ListaTemporadasAfectadas)
                   .Where(a => a.Contrato.ContratoId == idContrato && a.Contrato.Distribuidor.DistribuidorId == idDistribuidor)
                 //  .OrderBy(a => a.IdentificadorModificador)
                   .ToList();

                //foreach (Modificador modificador in lista)
                //{
                //    if (idProducto == 0)
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor).ToList();
                //    }
                //    else
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                //    }
                //    modificador.ListaReglas = _context.Reglas
                //       .Include(x => x.TipoHabitacion)
                //      .Where(b => b.ModificadorId == modificador.ModificadorId)
                //      .ToList();
                //}

            }
            else
              if (idContrato != -1 && idDistribuidor == -1)
            {
                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
                .Include(a => a.Contrato)
                // .Include(a => a.Contrato.Distribuidor)
                //.Include(a => a.Contrato.Temporadas)
                //.Include(a => a.ListaHoteles)

                .Include(a => a.Proveedor)
                 //.Include(a => a.ListaTemporadasAfectadas)

                 .Where(a => a.Contrato.ContratoId == idContrato && a.Proveedor.ProveedorId == idProveedor)
                //.OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                    lista = _context.Modificadores
                    .Include(a => a.Contrato)
                    // .Include(a => a.Contrato.Distribuidor)
                    //.Include(a => a.Contrato.Temporadas)
                    //.Include(a => a.ListaHoteles)

                    //.Include(a => a.Proveedor)
                    // .Include(a => a.ListaTemporadasAfectadas)

                     .Where(a => a.Contrato.ContratoId == idContrato)
                    //.OrderBy(a => a.IdentificadorModificador)
                    .ToList();


                //foreach (Modificador modificador in lista)
                //{
                //    if (idProducto == 0)
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor).ToList();
                //    }
                //    else
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                //    }
                //    modificador.ListaReglas = _context.Reglas
                //       .Include(x => x.TipoHabitacion)
                //      .Where(b => b.ModificadorId == modificador.ModificadorId)
                //      .ToList();
                //}

                return lista.Count();
            }
            else
              if (idContrato == -1 && idDistribuidor != -1)
            {
                if (idProveedor != 0)
                {
                    lista = _context.Modificadores
               //.Include(a => a.Contrato)
               .Include(a => a.Contrato.Distribuidor)
              //.Include(a => a.Contrato.Temporadas)
              // .Include(a => a.ListaHoteles)

               .Include(a => a.Proveedor)
                //.Include(a => a.ListaTemporadasAfectadas)
                .Where(a => a.Contrato.DistribuidorId == idDistribuidor && idProveedor == a.Proveedor.ProveedorId)
                //.OrderBy(a => a.IdentificadorModificador)
                .ToList();
                }
                else
                    lista = _context.Modificadores
                   //.Include(a => a.Contrato)
                   .Include(a => a.Contrato.Distribuidor)
                  //.Include(a => a.Contrato.Temporadas)
                  // .Include(a => a.ListaHoteles)

                  // .Include(a => a.Proveedor)
                    .Include(a => a.ListaTemporadasAfectadas)
                    .Where(a => a.Contrato.DistribuidorId == idDistribuidor)
                    //.OrderBy(a => a.IdentificadorModificador)
                    .ToList();
                //foreach (Modificador modificador in lista)
                //{
                //    if (idProducto == 0)
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor).ToList();
                //    }
                //    else
                //    {
                //        modificador.Contrato.Distribuidor.ListaProductosDistribuidos = _context.ProductoDistribuidores.Include(x => x.Producto)
                //       .Where(x => x.DistribuidorId == modificador.Contrato.DistribuidorId && x.Producto.TipoProducto.Nombre == ValoresAuxiliares.ACCOMMODATION && x.Producto.ProveedorId == idProveedor && x.Producto.ProductoId == idProducto).ToList();
                //    }
                //    modificador.ListaReglas = _context.Reglas
                //       .Include(x => x.TipoHabitacion)
                //      .Where(b => b.ModificadorId == modificador.ModificadorId)
                //      .ToList();
                //}

                return lista.Count();
            }


            return lista.Count();

        }
    }
}