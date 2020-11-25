using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreciosOrdenModificadosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public PreciosOrdenModificadosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/PreciosOrdenModificados
        [HttpGet]
        public IEnumerable<PreciosOrdenModificados> GetPreciosOrdenModificados()
        {
            return _context.PreciosOrdenModificados;
        }

        // GET: api/PreciosOrdenModificados/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPreciosOrdenModificados([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preciosOrdenModificados = await _context.PreciosOrdenModificados.FindAsync(id);

            if (preciosOrdenModificados == null)
            {
                return NotFound();
            }

            return Ok(preciosOrdenModificados);
        }

        // PUT: api/PreciosOrdenModificados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreciosOrdenModificados([FromRoute] int id, [FromBody] PreciosOrdenModificados preciosOrdenModificados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != preciosOrdenModificados.PreciosOrdenModificadosId)
            {
                return BadRequest();
            }

            _context.Entry(preciosOrdenModificados).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreciosOrdenModificadosExists(id))
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

        // POST: api/PreciosOrdenModificados
        [HttpPost]
        public async Task<IActionResult> PostPreciosOrdenModificados([FromBody] PreciosOrdenModificados preciosOrdenModificados)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PreciosOrdenModificados.Add(preciosOrdenModificados);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPreciosOrdenModificados", new { id = preciosOrdenModificados.PreciosOrdenModificadosId }, preciosOrdenModificados);
        }

        // DELETE: api/PreciosOrdenModificados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreciosOrdenModificados([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var preciosOrdenModificados = await _context.PreciosOrdenModificados.FindAsync(id);
            if (preciosOrdenModificados == null)
            {
                return NotFound();
            }

            _context.PreciosOrdenModificados.Remove(preciosOrdenModificados);
            await _context.SaveChangesAsync();

            return Ok(preciosOrdenModificados);
        }

        private bool PreciosOrdenModificadosExists(int id)
        {
            return _context.PreciosOrdenModificados.Any(e => e.PreciosOrdenModificadosId == id);
        }


        // GET: api/PreciosOrdenModificados/Vehiculo
        [HttpGet]
        [Route("Vehiculo")]
        public List<PreciosOrdenModificados> GetPreciosModificadosVehiculosByIdOrdenVehidulo(int id)
        {
            return _context.PreciosOrdenModificados.Where(x => x.OrdenVehiculoId == id).ToList();

        }

        // GET: api/PreciosOrdenModificados/Alojamiento
        [HttpGet]
        [Route("Alojamiento")]
        public List<PreciosOrdenModificados> GetPreciosModificadosAlojamientoByIdOrdenAlojamiento(int id)
        {
            return _context.PreciosOrdenModificados.Where(x => x.OrdenAlojamientoId == id).ToList();

        }

        // GET: api/PreciosOrdenModificados/Actividad
        [HttpGet]
        [Route("Actividad")]
        public List<PreciosOrdenModificados> GetPreciosModificadosActividadByIdOrdenActividad(int id)
        {
            return _context.PreciosOrdenModificados.Where(x => x.OrdenActividadId == id).ToList();

        }

        // GET: api/PreciosOrdenModificados/Traslados
        [HttpGet]
        [Route("Traslados")]
        public List<PreciosOrdenModificados> GetPreciosModificadosTrasladosByIdOrdenTraslados(int id)
        {
            return _context.PreciosOrdenModificados.Where(x => x.OrdenTrasladoId == id).ToList();

        }


    }
}