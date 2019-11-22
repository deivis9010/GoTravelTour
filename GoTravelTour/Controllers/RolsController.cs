﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using MimeKit;
using MimeKit.Text;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolsController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public RolsController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Rols
        [HttpGet]
        public IEnumerable<Rol> GetRoles(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {

            IEnumerable<Rol> lista;
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Roles.Where(p => (p.NombreRol.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList();
            }
            else
            {
                lista = _context.Roles.ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("NombreRol".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.NombreRol);

                        }
                        
                       
                        break;
                    }

                default:
                    {
                        if ("NombreRol".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.NombreRol);

                        }
                       

                        break;

                    }

                    break;
            }
            EnviarCorreo(new Usuario());
            return lista;
            
        }
        // GET: api/Rols/Count
        [Route("Count")]
        [HttpGet]
        public int GetRolsCount()
        {
            return _context.Roles.Count();
        }

        public bool EnviarCorreo(Usuario usuario)
        {
            try
            {
                var message = new MimeMessage();

                message.To.Add(new MailboxAddress("deivis9010@gmail.com"));
                message.From.Add(new MailboxAddress("deivis9010@gmail.com"));
                message.Subject = "Usuario nuevo en el sistema";
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = "Gracias por registarse con nosotros " + "<br>"
                 + "Por favor haga click en el " + "<br>"
                + "siguiente enlace para <a href='http://setvmas.com/emailconfirm'" + usuario.Correo + "> registrarse</a>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("a2plcpnl0550.prod.iad2.secureserver.net",465, MailKit.Security.SecureSocketOptions.Auto);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("mainaccount@gaybook.us", "Gaybook2015");

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;

            }

        }

        // GET: api/Rols/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRol([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound();
            }

            return Ok(rol);
        }

        // PUT: api/Rols/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol([FromRoute] int id, [FromBody] Rol rol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rol.RolId)
            {
                return BadRequest();
            }

            _context.Entry(rol).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolExists(id))
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

        // POST: api/Rols
        [HttpPost]
        public async Task<IActionResult> PostRol([FromBody] Rol rol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Rol crol = _context.Roles.Single(c => c.NombreRol == rol.NombreRol);
            if (crol != null)
            {
                return CreatedAtAction("GetRol", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRol", new { id = rol.RolId }, rol);
        }

        // DELETE: api/Rols/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return Ok(rol);
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.RolId == id);
        }
    }
}