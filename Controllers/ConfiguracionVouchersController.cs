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
    public class ConfiguracionVouchersController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ConfiguracionVouchersController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/ConfiguracionVouchers
        [HttpGet]
        public IEnumerable<ConfiguracionVoucher> GetConfiguracionVoucher(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<ConfiguracionVoucher> lista;
            if (col == "-1")
            {
                return _context.ConfiguracionVoucher
                    .Include(x => x.TipoProducto)
                    .OrderBy(a => a.InfoAgente)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.ConfiguracionVoucher.Include(x => x.TipoProducto)
                    .Where(p => (p.InfoAgente.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.ConfiguracionVoucher.Include(x => x.TipoProducto).ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("InfoAgente".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.InfoAgente);

                        }

                        break;
                    }

                default:
                    {
                        if ("InfoAgente".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.InfoAgente);

                        }

                        break;

                    }


            }

            return lista;

        }
        // GET: api/ConfiguracionVouchers/Count
        [Route("Count")]
        [HttpGet]
        public int GetConfiguracionVouchersCount()
        {
            return _context.ConfiguracionVoucher.Count();
        }

        // GET: api/ConfiguracionVouchers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConfiguracionVoucher([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var configuracionVoucher = await _context.ConfiguracionVoucher.FindAsync(id);

            if (configuracionVoucher == null)
            {
                return NotFound();
            }

            return Ok(configuracionVoucher);
        }

        // PUT: api/ConfiguracionVouchers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracionVoucher([FromRoute] int id, [FromBody] ConfiguracionVoucher configuracionVoucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != configuracionVoucher.ConfiguracionVoucherId)
            {
                return BadRequest();
            }

            _context.Entry(configuracionVoucher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfiguracionVoucherExists(id))
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

        // POST: api/ConfiguracionVouchers
        [HttpPost]
        public async Task<IActionResult> PostConfiguracionVoucher([FromBody] ConfiguracionVoucher configuracionVoucher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ConfiguracionVoucher.Add(configuracionVoucher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConfiguracionVoucher", new { id = configuracionVoucher.ConfiguracionVoucherId }, configuracionVoucher);
        }

        // DELETE: api/ConfiguracionVouchers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracionVoucher([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var configuracionVoucher = await _context.ConfiguracionVoucher.FindAsync(id);
            if (configuracionVoucher == null)
            {
                return NotFound();
            }

            _context.ConfiguracionVoucher.Remove(configuracionVoucher);
            await _context.SaveChangesAsync();

            return Ok(configuracionVoucher);
        }

        private bool ConfiguracionVoucherExists(int id)
        {
            return _context.ConfiguracionVoucher.Any(e => e.ConfiguracionVoucherId == id);
        }
    }
}