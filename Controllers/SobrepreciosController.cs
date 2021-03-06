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

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SobrepreciosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public SobrepreciosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Sobreprecios
        [HttpGet]
        public IEnumerable<Sobreprecio> GetSobreprecio(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Sobreprecio> lista;
            if (col == "-1")
            {
                return _context.Sobreprecio.OrderBy(a => a.TipoProducto.Nombre).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Sobreprecio.Where(p => (p.TipoProducto.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Sobreprecio.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.TipoProducto.Nombre);

                        }
                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.TipoProducto.Nombre);

                        }
                        break;

                    }


            }

            return lista;
        }


        // GET: api/Sobreprecios/Count
        [Route("Count")]
        [HttpGet]
        public int GetSobrepreciosCount()
        {
            return _context.Sobreprecio.Count();
        }

        // GET: api/Sobreprecios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSobreprecio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sobreprecio = await _context.Sobreprecio.FindAsync(id);

            if (sobreprecio == null)
            {
                return NotFound();
            }

            return Ok(sobreprecio);
        }

        // PUT: api/Sobreprecios/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutSobreprecio([FromRoute] int id, [FromBody] Sobreprecio sobreprecio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sobreprecio.SobreprecioId)
            {
                return BadRequest();
            }
            if (_context.Sobreprecio.Any(c => c.TipoProducto.Nombre == sobreprecio.TipoProducto.Nombre && sobreprecio.SobreprecioId != id))
            {
                return CreatedAtAction("GetSobreprecio", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            if (!ValidarRango(sobreprecio))
            {
                return CreatedAtAction("IsRangoValido", new { id = -3, error = "Rango Solapado" }, new { id = -3, error = "Rango Solapado" });
            }

            _context.Entry(sobreprecio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SobreprecioExists(id))
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

        // POST: api/Sobreprecios
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostSobreprecio([FromBody] Sobreprecio sobreprecio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!ValidarRango(sobreprecio))
            {
                return CreatedAtAction("IsRangoValido", new { id = -3, error = "Rango Solapado" }, new { id = -3, error = "Rango Solapado" });
            }
            _context.Sobreprecio.Add(sobreprecio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSobreprecio", new { id = sobreprecio.SobreprecioId }, sobreprecio);
        }

        // DELETE: api/Sobreprecios/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSobreprecio([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sobreprecio = await _context.Sobreprecio.FindAsync(id);
            if (sobreprecio == null)
            {
                return NotFound();
            }

            _context.Sobreprecio.Remove(sobreprecio);
            await _context.SaveChangesAsync();

            return Ok(sobreprecio);
        }

        private bool SobreprecioExists(int id)
        {
            return _context.Sobreprecio.Any(e => e.SobreprecioId == id);
        }

        // GET: api/Sobreprecios/Tipo/5
        [HttpGet("{idTipoProducto}")]
        [Route("Tipo/{idTipoProducto}")]
        public async Task<IActionResult> GetSobreprecioByTipo([FromRoute] int idTipoProducto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Sobreprecio> sobreprecio =  _context.Sobreprecio.Include(x=>x.TipoProducto).Where(a=>a.TipoProductoId == idTipoProducto).ToList();

            if (sobreprecio == null)
            {
                return NotFound();
            }

            return Ok(sobreprecio);
        }



        /// <summary>
        /// Validar que los rangos de precio no se solapen
        /// </summary>     
        /// <param name="newRango"></param>
        /// <returns></returns>
        private bool ValidarRango(Sobreprecio newRango)
        {
            if (newRango.TipoProductoId <= 0)
            {
                return false;
            }

            List<Sobreprecio> rangos = _context.Sobreprecio.AsNoTracking().Where(x => x.TipoProductoId == newRango.TipoProductoId).ToList();
            foreach (var r in rangos)
            {
                if ((r.PrecioDesde <= newRango.PrecioDesde && newRango.PrecioDesde <= r.PrecioHasta ||
                    r.PrecioDesde <= newRango.PrecioHasta && newRango.PrecioHasta <= r.PrecioHasta) &&
                    r.SobreprecioId != newRango.SobreprecioId)
                {
                    return false;
                }
            }



            return true;
        }




    }
}