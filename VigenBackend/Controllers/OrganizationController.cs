using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vigen_Repository.Models;
using Microsoft.Extensions.Configuration;

namespace Vigen_Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly vigendbContext _context;
        private readonly IConfiguration _configuration;
        public OrganizationController(vigendbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<ActionResult<List<Organization>>> getOrganizations()
        {
            List<Organization> organization = await _context.Organizations.ToListAsync();
            if (organization.Count == 0) return NoContent();
            return Ok(organization);
        }

        [HttpGet("{user}/{password}")]
        public async Task<ActionResult<Object>> loginOrganization(string user, string password)
        {
            // Busca la organización en la base de datos
            Organization? orgObject = await _context.Organizations.FirstOrDefaultAsync(o => o.Nit == user); // Asumiendo que `Identification` es el campo que identifica a la organización

            // Si no se encuentra la organización o la contraseña no coincide
            if (orgObject == null || orgObject.Password != password)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET");

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return StatusCode(500, "JWT Secret Key is missing. Set it in environment variables or appsettings.json.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, orgObject.Nit),
            new Claim(ClaimTypes.Role, "Organization") // Cambia el rol si es necesario
        }),
                Expires = DateTime.UtcNow.AddHours(1), // Expira en 1 hora
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString, // Devuelve el token como cadena
                name = orgObject.Name,
                nit = orgObject.Nit,
                tel = orgObject.Tel, // Asume que hay un campo para la información de contacto
                phone = orgObject.Phone, // Asume que hay un campo para la dirección
                organizationType = orgObject.OrganizationTypeID                            // Agrega otros campos que sean relevantes
            });
        }

        [HttpGet("{nit}")]
        public async Task<ActionResult<Organization>> getOrganization(string nit)
        {
            Organization? organization = await _context.Organizations.FindAsync(nit);
            if (organization == null) return NotFound();
            return Ok(organization);
        }

        [HttpPost]
        public async Task<ActionResult<Organization>> postOrganization(Organization organization)
        {
            try
            {
                await _context.Organizations.AddAsync(organization);
                await _context.SaveChangesAsync();
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{nit}")]
        public async Task<ActionResult<Organization>> UpdateOrganization(string nit, Organization organization)
        {
            if (nit != organization.Nit) return BadRequest("El Nit no concide");
            try
            {
                _context.Entry(organization).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(organization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{nit}")]
        public async Task<ActionResult<Organization>> DeleteOrganization(string nit)
        {
            try
            {
                Organization? organization = await _context.Organizations.FindAsync(nit);
                if(organization == null) return NotFound();
                _context.Organizations.Remove(organization);
                await _context.SaveChangesAsync();
                return Ok(organization);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
