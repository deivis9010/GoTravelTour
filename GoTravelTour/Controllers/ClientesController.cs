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
    public class ClientesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public ClientesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        [Authorize]
        public IEnumerable<Cliente> GetClientes(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Cliente> lista;
            if (col == "-1")
            {
                return _context.Clientes.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Clientes.Where(p => (p.Nombre.ToLower().Contains(filter.ToLower()))|| (p.Nombre.ToLower().Contains(filter.ToLower()))||( p.Localizador.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Clientes.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Nombre);

                        }
                        else
                        if ("Localizador".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Localizador);
                        }
                        else
                        if ("Pais".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Pais);
                        }
                        else
                        if ("Estado".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Estado);
                        }
                        else
                        if ("IsPublic".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.IsPublic);
                        }



                        break;
                    }

                default:
                    {
                        if ("Nombre".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Nombre);

                        }
                        else
                       if ("Localizador".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Localizador);
                        }
                        else
                       if ("Pais".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Pais);
                        }
                        else
                       if ("Estado".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Estado);
                        }
                        else
                       if ("IsPublic".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.IsPublic);
                        }



                        break;

                    }

                    break;
            }

            return lista;
            
        }
        // GET: api/Clientes/Count
        [Route("Count")]
        [HttpGet]
        public int GetClientesCount()
        {
            return _context.Clientes.Count();
        }


        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliente([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente([FromRoute] int id, [FromBody] Cliente cliente)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cliente.ClienteId)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
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

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> PostCliente([FromBody] Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<Cliente> cl = _context.Clientes.Where(c=>c.Nombre == cliente.Nombre).ToList();
            if (cl.Count > 0)
            {
                return CreatedAtAction("GetCliente", new { id = -2, error="Ya existe" }, new { id = -2, error = "Ya existe" });
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCliente", new { id = cliente.ClienteId }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.ClienteId == id);
        }
    }
}