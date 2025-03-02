using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vigen_Repository.Models;
using Microsoft.EntityFrameworkCore;
using Vigen_Repository.Email;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Vigen_Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly vigendbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(vigendbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<ActionResult<List<User>>> getUsers()
        {
            List<User> users = await _context.Users.ToListAsync();
            if (users.Count == 0) return NoContent();
            return Ok(users);
        }

        [HttpGet("{user}/{password}")]
        public async Task<ActionResult<Object>> loginUser(string user, string password)
        {
            // Busca el usuario en la base de datos
            User? userObject = await _context.Users.FirstOrDefaultAsync(u => u.Identification == user);

            // Si no se encuentra el usuario o la contraseña no coincide
            if (userObject == null || userObject.Password != password)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Si la autenticación es exitosa, genera el token JWT
            var secretKey = _configuration["JwtSettings:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                return StatusCode(500, "JWT Secret Key is missing in configuration");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, userObject.Identification),
            new Claim(ClaimTypes.Role, "User") // Si deseas agregar roles u otros claims
        }),
                Expires = DateTime.UtcNow.AddHours(1), // Expira en 1 hora
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = token,
                name = userObject.Name,
                gender = userObject.Gender,
                email = userObject.Email,
                identification = userObject.Identification,
                birthdate = userObject.Birthdate,
                country_code = userObject.CountryCode,
                phone = userObject.Phone,
                occupation = userObject.Occupation,
                postal_code = userObject.PostalCode,
                marital_status = userObject.MaritalStatus,
                ubication = userObject.Ubication,
                code = userObject.Code,
                verification = userObject.Verification
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> getUser(string id)
        {
            User? user = await _context.Users.FindAsync(id);
            if(user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> postUser(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                Send send = new Send();
                var res = send.enviar(user.Email, user.Code);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
            return Ok(new { status = "success", message = "User registered successfully" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(string id, User user)
        {
            if (id != user.Identification)
                return BadRequest(new { message = "El id no coincide con el usuario." });

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Retorna un JSON con el mensaje de error
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("verify/{userId}")]
        public async Task<ActionResult> VerifyUser(string userId, [FromBody] VerificationRequest request)
        {
            // Buscar el usuario por ID
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Verificar que el código coincida
            if (user.Code != request.Code)
            {
                return BadRequest(new { message = "Invalid verification code" });
            }

            // Actualizar el campo Verification a true
            user.Verification = true;

            // Guardar los cambios en la base de datos
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "User verified successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Clase para recibir el código de verificación en el body
        public class VerificationRequest
        {
            public string Code { get; set; }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            try
            {
                User? user = await _context.Users.FindAsync(id);
                if(user==null) return NotFound();
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
    }
}
