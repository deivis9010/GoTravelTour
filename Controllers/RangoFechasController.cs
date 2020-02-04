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

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RangoFechasController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RangoFechasController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/RangoFechas
        [HttpGet]
        public IEnumerable<RangoFechas> GetRangoFechas(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<RangoFechas> lista;
            if (col == "-1")
            {
                return _context.RangoFechas                    
                    .Include(a => a.Temporada)                    
                    .ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.RangoFechas
                    .Include(a => a.Temporada)
                    .Where(p => (p.Temporada.Nombre.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.RangoFechas
                    .Include(a => a.Temporada)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Temporada.Nombre);

                        }

                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Temporada.Nombre);

                        }
                    }

                    break;
            }

            return lista;
        }
        // GET: api/RangoFechas/Count
        [Route("Count")]
        [HttpGet]
        public int GetRangoFechasCount()
        {
            return _context.RangoFechas.Count();
        }

        // GET: api/RangoFechas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRangoFechas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rangoFechas = await _context.RangoFechas.FindAsync(id);

            if (rangoFechas == null)
            {
                return NotFound();
            }

            return Ok(rangoFechas);
        }

        // PUT: api/RangoFechas/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRangoFechas([FromRoute] int id, [FromBody] RangoFechas rangoFechas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rangoFechas.RangoFechasId)
            {
                return BadRequest();
            }

            _context.Entry(rangoFechas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RangoFechasExists(id))
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

        // POST: api/RangoFechas
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostRangoFechas([FromBody] RangoFechas rangoFechas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RangoFechas.Add(rangoFechas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRangoFechas", new { id = rangoFechas.RangoFechasId }, rangoFechas);
        }

        // DELETE: api/RangoFechas/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRangoFechas([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rangoFechas = await _context.RangoFechas.FindAsync(id);
            if (rangoFechas == null)
            {
                return NotFound();
            }

            _context.RangoFechas.Remove(rangoFechas);
            await _context.SaveChangesAsync();

            return Ok(rangoFechas);
        }

        private bool RangoFechasExists(int id)
        {
            return _context.RangoFechas.Any(e => e.RangoFechasId == id);
        }
    }
}