﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using PagedList;
using GoTravelTour.Seguridad;
using MimeKit;
using MimeKit.Text;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public UsuariosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public IEnumerable<Usuario> GetUsuarios(string col = "", string filter = "", string sortDirection = "asc", int pageIndex = 1, int pageSize = 1)
        {
            IEnumerable<Usuario> lista;
            if (col == "-1")
            {
                return _context.Usuarios.ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Usuarios.Include(c => c.cliente).Include(r => r.rol)
                    .Where(p => (p.Username.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Usuarios.Include(c => c.cliente).Include(r => r.rol)
                    .ToPagedList(pageIndex, pageSize).ToList();
            }

            switch (sortDirection)
            {
                case "desc":
                    {
                        if ("Username".Equals(col))
                        {
                            lista = lista.OrderByDescending(l => l.Username);

                        }
                        else
                        if ("Correo".Equals(col))                        
                        {
                            lista = lista.OrderByDescending(l => l.Correo);
                        }
                        


                        break;
                    }

                default:
                    {
                        if ("Username".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Username);

                        }
                        else
                        if ("Correo".Equals(col))
                        {
                            lista = lista.OrderBy(l => l.Correo);
                        }



                        break;

                    }

                    break;
            }

            return lista;
           
        }
        // GET: api/Usuarios/Count
        [Route("Count")]
        [HttpGet]
        public int GetUsuariosCount()
        {
            return _context.Usuarios.Count();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario([FromRoute] int id, [FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuario.UsuarioId)
            {
                return BadRequest();
            }
            EncriptarPass encriptador = new EncriptarPass();
            string passNoEnc = usuario.Password;
            string passEnc = encriptador.Encripta(passNoEnc);
            usuario.Password = passEnc;


            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // POST: api/Usuarios
        [HttpPost]
        public async Task<IActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<Usuario> usu = _context.Usuarios.Where(c => c.Username == usuario.Username).ToList();
            if (usu.Count > 0)              
            {
                return CreatedAtAction("GetUsuario", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }
            EncriptarPass encriptador = new EncriptarPass();
            string passNoEnc = usuario.Password;
            string passEnc = encriptador.Encripta(passNoEnc);
            usuario.Password = passEnc;
            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
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
                    Text = "Se ha registrado un usuario nuevo con tal informacion " + "<br>"
                 + "Por favor haga click en el " + "<br>"
                + "siguiente enlace para activarlo <a href='http://gotravelands.com/activar'" + usuario.Correo + "> Activar</a>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("mail.gaybook.us", 587, MailKit.Security.SecureSocketOptions.Auto);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("mainaccount@gaybook.us", "gottcuba2019");

                    emailClient.Send(message);

                    emailClient.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {

                return false;

            }

        }

        // GET: api/Usuarios/Activar
        [Route("Activar")]
        [HttpGet]
        public async Task<IActionResult> GetUsuariosActivar(int id)
        {
            

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.IsActivo = true;
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(usuario);
            
        }
    }
}