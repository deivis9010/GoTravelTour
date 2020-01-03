using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;

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

        // PUT: api/Contratoes/5
        [HttpPut("{id}")]
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
    }
}