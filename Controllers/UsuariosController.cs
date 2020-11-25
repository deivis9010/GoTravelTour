using System;
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
using Microsoft.AspNetCore.Authorization;
using MimeKit.Utils;

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
                return _context.Usuarios.Include(c => c.cliente).Include(r => r.rol).OrderBy(a => a.Username).ToList();
            }
            if (!string.IsNullOrEmpty(filter))
            {
                lista = _context.Usuarios.Include(c => c.cliente).Include(r => r.rol)
                    .OrderBy(a => a.Username)
                    .Where(p => (p.Username.ToLower().Contains(filter.ToLower()))).ToPagedList(pageIndex, pageSize).ToList(); ;
            }
            else
            {
                lista = _context.Usuarios.Include(c => c.cliente).Include(r => r.rol)
                    .OrderBy(a => a.Username)
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

            var usuario =  _context.Usuarios.Include(c => c.cliente).Include(r => r.rol).First(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize]
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
            if (_context.Usuarios.Any(c => c.Username == usuario.Username && c.UsuarioId != id))
            {
                return CreatedAtAction("GetUsuario", new { id = -2, error = "Ya existe" }, new { id = -2, error = "Ya existe" });
            }

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
        [Authorize]
        public async Task<IActionResult> PostUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
             
            if (_context.Usuarios.Any(c => c.Username == usuario.Username))              
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
                usuario.rol = _context.Roles.Find(usuario.RolId);
                if (usuario.IsActivo)
                {
                    EnviarCorreo(usuario);
                }
                else
                {
                    EnviarCorreoSolicitarAcceso(usuario);
                }
                
            }
            catch (Exception ex)
            {

            }

            

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize]
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

                message.To.Add(new MailboxAddress(usuario.Correo));
                message.From.Add(new MailboxAddress("sales@gotravelandtours.com"));
                message.Subject = "Bienvenido a Go Travel and Tours!!!";
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = "Estimado: "+ usuario.Username + "<br>"+" gracias por registarse con nosotros." + "<br>"
                 + "Go Travel and Tours le proporciona, mediante nuestro sistema, la manera más fácil de vender el destino Cuba " + "<br/>"
                 + "Ponemos a su disposición diferentes productos de primera mano. Cada oferta, producto y precio no son públicos, " + "<br>"
                  + "sólo son para agencias de viaje y turoperadores. Visite nuestro sistema entrando a: " + "<br>"
                + "<a href='http://gotravelandtours.com'>www.gotravelandtours.com </a>"
                + "Puede escribirnos a sales@gotravelandtours.com o llamar al 786-315-8244"  + "<br>"

                + "<br><br><br><br><br><br><br>"
                + "<img src=''/> <br>"
                + "<div style='font-weight:bold'>Equipo Go Travel and Tours</div> <br>"
                + "<div style=''>17118 sw 144th ct., Miamin FL 33177 </div> <br>"
                 + "<div style=''>B2B Cuban Wholesale: gotravelandtours.com </div> <br>"
                 + "<div style=''>Skype: elilor0202 </div> <br>"
                 + "<div style=''>Whatsapp: 786-315-8244 </div> <br>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Timeout = 120000;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.Auto);

                    //emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("postmaster@gotravelandtours.com", "Gott2019conga@#$");

                    /**AGREGADO PARA  EVITAR ENVIO DE CORREOS COMO SPAM**/
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    message.MessageId = MimeUtils.GenerateMessageId("efferenthealthllc.onmicrosoft.com");
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentId = null;
                    /** FIN **/

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



        public bool EnviarCorreoSolicitarAcceso(Usuario usuario)
        {
            try
            {
                var message = new MimeMessage();

                message.To.Add(new MailboxAddress("sales@gotravelandtours.com"));
                message.From.Add(new MailboxAddress(usuario.Correo));
                message.Subject = "Nueva solicitud de acceso al sistema. Usuario: "+ usuario.Username;
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = "El usuario: " + usuario.Username + "<br>"
                 + "Está solicitando acceso al sistema. Si así lo desea puede activarlo" 

                + "<br><br><br><br><br><br><br>"
                + "<img src=''/> <br>"
                + "<div style='font-weight:bold'>Equipo Go Travel and Tours</div> <br>"
                + "<div style=''>17118 sw 144th ct., Miamin FL 33177 </div> <br>"
                 + "<div style=''>B2B Cuban Wholesale: gotravelandtours.com </div> <br>"
                 + "<div style=''>Skype: elilor0202 </div> <br>"
                 + "<div style=''>Whatsapp: 786-315-8244 </div> <br>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Timeout = 120000;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.Auto);

                    //emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("postmaster@gotravelandtours.com", "Gott2019conga@#$");

                    /**AGREGADO PARA  EVITAR ENVIO DE CORREOS COMO SPAM**/
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    message.MessageId = MimeUtils.GenerateMessageId("efferenthealthllc.onmicrosoft.com");
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentId = null;
                    /** FIN **/

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


        // GET: api/Usuarios/Mail
        [Route("Mail")]
        [HttpGet]
        public bool GetSolicitudCreacionUsuario(string email)
        {
            try
            {
                var message = new MimeMessage();

                message.To.Add(new MailboxAddress("sales@gotravelandtours.com"));
                message.From.Add(new MailboxAddress(email));
                message.Subject = "Nueva solicitud para obtener acceso al sistema. ";
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = "El usuario del correo: " + email + "<br>"
                 + "Está solicitando acceso al sistema. Debe contactarlo para confirmar la solicitud o no."

                + "<br><br><br><br><br><br><br>"
                + "<img src=''/> <br>"
                + "<div style='font-weight:bold'>Equipo Go Travel and Tours</div> <br>"
                + "<div style=''>17118 sw 144th ct., Miamin FL 33177 </div> <br>"
                 + "<div style=''>B2B Cuban Wholesale: gotravelandtours.com </div> <br>"
                 + "<div style=''>Skype: elilor0202 </div> <br>"
                 + "<div style=''>Whatsapp: 786-315-8244 </div> <br>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Timeout = 120000;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.Auto);

                    //emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("postmaster@gotravelandtours.com", "Gott2019conga@#$");

                    /**AGREGADO PARA  EVITAR ENVIO DE CORREOS COMO SPAM**/
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    message.MessageId = MimeUtils.GenerateMessageId("efferenthealthllc.onmicrosoft.com");
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentId = null;
                    /** FIN **/

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
                EnviarCorreo(usuario);
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


        [Route("C")]
        [HttpGet]
        public bool EnviarCorreoTest()
        {
            try
            {
                var message = new MimeMessage();
                //Gott2019conga@#$ mail.gotravel
                message.To.Add(new MailboxAddress("sales@gotravelandtours.com" /*usuario.Correo*/));
                message.From.Add(new MailboxAddress("sales@gotravelandtours.com"));
                message.Subject = "Bienvenido a Go Travel and Tours!!!";
                //We will say we are sending HTML. But there are options for plaintext etc. 
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = "Gracias por registarse con nosotros: " + "<br>"
                 + "Go Travel and Tours le proporciona, mediante nuestro sistema, la manera más fácil de vender el destino Cuba " + "<br/>"
                 + "Ponemos a su disposición diferentes productos de primera mano. Cada oferta, producto y precio no son públicos, " + "<br>"
                  + "sólo son para agencias de viaje y turoperadores. Visite nuestro sistema entrando a: " + "<br>"
                + "<a href='http://gotravelandtours.com'>www.gotravelandtours.com </a>"
                + "Puede escribirnos a sales@gotravelandtours.com o llamar al 786-315-8244" + "<br>"

                + "<br><br><br><br><br><br><br>"
                + "<img src=''/> <br>"
                + "<div style='font-weight:bold'>Equipo Go Travel and Tours</div> <br>"
                + "<div style=''>17118 sw 144th ct., Miamin FL 33177 </div> <br>"
                 + "<div style=''>B2B Cuban Wholesale: gotravelandtours.com </div> <br>"
                 + "<div style=''>Skype: elilor0202 </div> <br>"
                 + "<div style=''>Whatsapp: 786-315-8244 </div> <br>"
                };


                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    emailClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    emailClient.Timeout =120000;
                    //The last parameter here is to use SSL (Which you should!)
                    emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.Auto);
                  
                    //emailClient.Connect("mail.gotravelandtours.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);

                    //Remove any OAuth functionality as we won't be using it. 
                    //  emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    emailClient.Authenticate("postmaster@gotravelandtours.com", "Gott2019conga@#$");

                    /**AGREGADO PARA  EVITAR ENVIO DE CORREOS COMO SPAM**/
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentTransferEncoding = ContentEncoding.QuotedPrintable;
                    message.MessageId = MimeUtils.GenerateMessageId("efferenthealthllc.onmicrosoft.com");
                    foreach (var part in message.BodyParts.OfType<TextPart>())
                        part.ContentId = null;
                    /** FIN **/

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

    }
}