using BackPruebaTecnica.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly PasswordEncryptionService _passwordEncryptionService;
    private readonly BackPruebaTecnicaDbContext _context;

    public AuthController(BackPruebaTecnicaDbContext context, JwtService jwtService, PasswordEncryptionService passwordEncryptionService)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordEncryptionService = passwordEncryptionService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        using (var context = new BackPruebaTecnicaDbContext())
        {
            // Obtener el usuario por su nombre de usuario
            var user = await context.Usuarios
                .FirstOrDefaultAsync(u => u.Usuario == loginRequest.Username);

            // Verificar si el usuario existe y si la contraseña es correcta
            if (user != null && _passwordEncryptionService.VerifyPassword(loginRequest.Password, user.Pass))
            {
                // Generar el token incluyendo el RolId
                var token = _jwtService.GenerateToken(user); // Pasar el objeto user
                return Ok(new { Token = token });
            }
        }

        return Unauthorized("Credenciales inválidas");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        using (var context = new BackPruebaTecnicaDbContext())
        {
            var encryptedPassword = _passwordEncryptionService.EncryptPassword(registerRequest.Password);
            var newUser = new Usuarios
            {
                Usuario = registerRequest.Username,
                Pass = encryptedPassword,
                FechaCreacion = DateTime.Now
            };

            context.Usuarios.Add(newUser);
            await context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente" });
        }
    }
    [Authorize]
    [HttpGet("protected-endpoint")]
    public IActionResult ProtectedEndpoint()
    {
        return Ok("Acceso autorizado a un endpoint protegido");
    }




}
