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
    public class ContratoesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ContratoesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Contratoes
        [HttpGet]
        public IEnumerable<Contrato> GetContratos(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Contrato> lista;
            if (col == "-1")
            {
                lista= _context.Contratos
                    .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)                    
                    .ToList();
                if(lista.ToList().Count > 0 )
                    foreach (var item in lista)
                    {
                        List<NombreTemporada> listNtemp = new List<NombreTemporada>();
                        foreach (var itemT in item.Temporadas)
                        {
                            NombreTemporada nt = _context.NombreTemporadas.First(a => a.Nombre == itemT.Nombre);
                            listNtemp.Add(nt);
                        }
                        item.NombreTemporadas = listNtemp;
                    }
                
                return lista.OrderBy(a => a.Nombre);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Contratos
                    .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
                    .Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Contratos
                    .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
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
        // GET: api/Contratoes/Count
        [Route("Count")]
        [HttpGet]
        public int GetContratoCount()
        {
            return _context.Contratos.Count();
        }

        // GET: api/Contratoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContrato([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contrato = await _context.Contratos.FindAsync(id);

            if (contrato == null)
            {
                return NotFound();
            }

            return Ok(contrato);
        }

        private void EliminarTemporadas ( Contrato contrato)
        {
            int j = 0;
            while (j < contrato.Temporadas.Count())
            {
                Temporada temp = contrato.Temporadas[j];
                bool esta = false;

                int i = 0;
                while (i < contrato.NombreTemporadas.Count() && !esta)
                {
                    NombreTemporada nombres = contrato.NombreTemporadas[i];
                    if (nombres.Nombre == temp.Nombre)
                    {
                        esta = true;
                        break;
                    }
                    i++;
                }

                if (!esta)
                {
                    contrato.Temporadas.Remove(temp);
                    _context.Temporadas.Remove(temp);
                    j--;
                }

                j++;
            }
            _context.SaveChanges();
        }

        // PUT: api/Contratoes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutContrato([FromRoute] int id, [FromBody] Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contrato.ContratoId)
            {
                return BadRequest();
            }
            if (_context.Contratos.Any(c => c.Nombre == contrato.Nombre && contrato.ContratoId != id))
            {
                return CreatedAtAction("GetContrato", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            try
            {
                EliminarTemporadas(contrato);
            } catch(Exception ex)
            {
                return CreatedAtAction("UpdateContrato", new { id = -4, error = "Temporada en uso no puede ser eliminada" }, new { id = -4, error = "Temporada en uso no puede ser eliminada" });
            }
            
            if (contrato.NombreTemporadas.Count > 0)
            {

                foreach (NombreTemporada nt in contrato.NombreTemporadas)
                {
                    int i = 0;
                    bool existeTemporada = false;
                    while (i < contrato.Temporadas.Count() && !existeTemporada)
                    {

                        if (nt.Nombre == contrato.Temporadas[i].Nombre)
                        {
                            existeTemporada = true;
                            _context.Entry(contrato.Temporadas[i]).State = EntityState.Modified;
                        }

                        i++;
                    }
                    if (!existeTemporada)
                    {
                        Temporada t = new Temporada();
                        t.Nombre = nt.Nombre;
                        t.ContratoId = contrato.ContratoId;
                        contrato.Temporadas.Add(t);
                        _context.Temporadas.Add(t);
                    }
                   
                }
            }

            _context.Entry(contrato).State = EntityState.Modified;
          

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContratoExists(id))
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

        // POST: api/Contratoes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostContrato([FromBody] Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Contratos.Any(c => c.Nombre == contrato.Nombre))
            {
                return CreatedAtAction("GetContratos", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            if (contrato.NombreTemporadas.Count > 0)
            {
                contrato.Temporadas = new List<Temporada>();
                foreach (NombreTemporada nt in contrato.NombreTemporadas)
                {
                    Temporada t = new Temporada();
                    t.Nombre = nt.Nombre;
                    
                    contrato.Temporadas.Add(t);
                }
            }


            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();
           /* if (contrato.NombreTemporadas.Count > 0)
            {
                foreach (NombreTemporada nt in contrato.NombreTemporadas)
                {
                    Temporada t = new Temporada();
                    t.Nombre = nt.Nombre;
                    t.ContratoId = contrato.ContratoId;
                    _context.Temporadas.Add(t);
                }
            }

            await _context.SaveChangesAsync();*/
            return CreatedAtAction("GetContrato", new { id = contrato.ContratoId }, contrato);
        }

        // DELETE: api/Contratoes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteContrato([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null)
            {
                return NotFound();
            }

            _context.Contratos.Remove(contrato);
            await _context.SaveChangesAsync();

            return Ok(contrato);
        }

        private bool ContratoExists(int id)
        {
            return _context.Contratos.Any(e => e.ContratoId == id);
        }


        // GET: api/Contratoes/Filtros
        [HttpGet]
        [Route("Filtros")]
        public IEnumerable<Contrato> GetContratosByFiltros(string tipoprod = "", int idDistribuidor = -1, bool IsActivo= true)
        {
            IEnumerable<Contrato> lista = new List<Contrato>();

            if (tipoprod == "" && idDistribuidor == -1)
            {

                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
                .Where(a => a.IsActivo == IsActivo)
                .OrderBy(a => a.Nombre)
                .ToList();
              
                return lista;
            }
            else
              if (tipoprod != "" && idDistribuidor != -1)
            {

                lista = _context.Contratos
               .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
               .Where(a => a.TipoProducto.Nombre == tipoprod && a.DistribuidorId == idDistribuidor &&  a.IsActivo == IsActivo)
               .OrderBy(a => a.Nombre)
               .ToList();
               
                return lista;
            }
            else
              if (tipoprod != "" && idDistribuidor == -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
                .Where(a => a.TipoProducto.Nombre == tipoprod && a.IsActivo == IsActivo)
                .OrderBy(a => a.Nombre)
                .ToList();

               
                return lista;
            }
            else
              if (tipoprod == "" && idDistribuidor != -1)
            {
                lista = _context.Contratos
                .Include(a => a.Distribuidor)
                    .Include(a => a.Temporadas)
                    .Include(a => a.TipoProducto)
                .Where(a => a.DistribuidorId == idDistribuidor)
                .OrderBy(a => a.Nombre)
                .ToList();
                
                return lista;
            }


            return lista;

        }


    }
}