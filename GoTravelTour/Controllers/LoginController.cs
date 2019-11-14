using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GoTravelTour.Models;
using GoTravelTour.Models.Seguridad;
using GoTravelTour.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly GoTravelDBContext _context;

        


        // TRAEMOS EL OBJETO DE CONFIGURACIÓN (appsettings.json)
        // MEDIANTE INYECCIÓN DE DEPENDENCIAS.
        public LoginController(IConfiguration configuration, GoTravelDBContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        // POST: api/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UsuarioLogin usuarioLogin)
        {
            var _userInfo = await AutenticarUsuarioAsync(usuarioLogin.Usuario, usuarioLogin.Password);
            if (_userInfo != null)
            {
                return Ok(new { token = GenerarTokenJWT(_userInfo) });
            }
            else
            {
                return Unauthorized();
            }
        }

        // COMPROBAMOS SI EL USUARIO EXISTE EN LA BASE DE DATOS 
        private async Task<UsuarioInfo> AutenticarUsuarioAsync(string usuario, string password)
        {
            // AQUÍ LA LÓGICA DE AUTENTICACIÓN //

            //Encriptando pass para comparar en BD
            EncriptarPass encriptador = new EncriptarPass();
            string passNoEnc = password;
            string passEnc = encriptador.Encripta(passNoEnc);
            password = passEnc;

            // Supondremos que el Usuario existe en la Base de Datos.
            // Retornamos un objeto del tipo UsuarioInfo, con toda
            // la información del usuario necesaria para el Token.
            Usuario user = _context.Usuarios.Include(uu => uu.rol)
                .SingleOrDefault(u => u.Username.Trim().ToLower() == usuario.Trim().ToLower() && u.Password == password)
                ;
            if (user != null)
            {
                /*return new UsuarioInfo()
                {
                    // Id del Usuario en el Sistema de Información (BD)
                    Id = new Guid("B5D233F0-6EC2-4950-8CD7-F44D16EC878F"),
                    Nombre = "Nombre Usuario",
                    //Apellidos = "Apellidos Usuario",
                    Email = "email.usuario@dominio.com",
                    Rol = "Administrador"
                };*/
                
                return new UsuarioInfo()
                {
                    // Id del Usuario en el Sistema de Información (BD)
                    Id = user.UsuarioId.ToString(),
                    Nombre = user.Username,
                    //Apellidos = "Apellidos Usuario",
                    Email = user.Correo,
                    Rol = user.rol.NombreRol
                };
            }
            else
            {
                return null;
            }
            

            // Supondremos que el Usuario NO existe en la Base de Datos.
            // Retornamos NULL.
            //return null;
        }

        // GENERAMOS EL TOKEN CON LA INFORMACIÓN DEL USUARIO
        private string GenerarTokenJWT(UsuarioInfo usuarioInfo)
        {
            // CREAMOS EL HEADER //
            var _symmetricSecurityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWT:ClaveSecreta"])
                );
            var _signingCredentials = new SigningCredentials(
                    _symmetricSecurityKey, SecurityAlgorithms.HmacSha256
                );
            var _Header = new JwtHeader(_signingCredentials);

            // CREAMOS LOS CLAIMS //
            var _Claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, usuarioInfo.Id.ToString()),
                new Claim("nombre", usuarioInfo.Nombre),
               // new Claim("apellidos", usuarioInfo.Apellidos),
                new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email),
                new Claim(ClaimTypes.Role, usuarioInfo.Rol)
            };

            // CREAMOS EL PAYLOAD //
            var _Payload = new JwtPayload(
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    claims: _Claims,
                    notBefore: DateTime.UtcNow,
                    // Exipra a la 24 horas.
                    expires: DateTime.UtcNow.AddHours(24)
                );

            // GENERAMOS EL TOKEN //
            var _Token = new JwtSecurityToken(
                    _Header,
                    _Payload
                );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}