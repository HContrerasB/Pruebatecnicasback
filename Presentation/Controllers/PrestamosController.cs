using BackPruebaTecnica.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class PrestamosController : ControllerBase
{
    private readonly BackPruebaTecnicaDbContext _context;

    public PrestamosController(BackPruebaTecnicaDbContext context)
    {
        _context = context;
    }

    // POST: api/prestamos
    [Authorize(Roles = "2")] // Solo usuarios comunes pueden solicitar préstamos
    [HttpPost("Solicitud")]
    public async Task<IActionResult> CrearPrestamo([FromBody] Prestamo prestamo)
    {
        if (!ModelState.IsValid) // Validar el modelo
        {
            return BadRequest(ModelState);
        }

        prestamo.FechaSolicitud = DateTime.Now;
        prestamo.Estado = "Solicitud"; // Estado inicial

        // Obtener el ID del usuario desde el contexto actual
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            prestamo.UsuarioId = int.Parse(userIdClaim.Value); // Asignar el ID del usuario
        }
        else
        {
            return Unauthorized("Usuario no autenticado.");
        }

        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerPrestamo), new { id = prestamo.Id }, prestamo);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Prestamo>>> ObtenerPrestamos()
    {
        var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        if (userIdClaim == null)
        {
            return Unauthorized("Usuario no autenticado.");
        }

        // Asegúrate de que el claim se pueda convertir a un entero
        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("El ID del usuario no es válido.");
        }

        return await _context.Prestamos
            .Where(p => p.UsuarioId == userId || User.IsInRole("1")) // Cambia "1" si es necesario
            .Include(p => p.Usuario)
            .ToListAsync();
    }


    // GET: api/prestamos/{id}
    [Authorize] // Cualquiera que esté autenticado puede ver un préstamo específico
    [HttpGet("ObtenerPrestamo {id}")]
    public async Task<ActionResult<Prestamo>> ObtenerPrestamo(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("Usuario no autenticado.");
        }

        int userId = int.Parse(userIdClaim.Value);
        var prestamo = await _context.Prestamos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id && (p.UsuarioId == userId || User.IsInRole("1")));

        if (prestamo == null)
        {
            return NotFound();
        }

        return prestamo;
    }

    // PUT: api/prestamos/{id}
    [Authorize(Roles = "1")] // Solo administradores pueden actualizar préstamos
    [HttpPut("ActualizarPrestamo {id}")]
    public async Task<IActionResult> ActualizarPrestamo(int id, [FromBody] Prestamo prestamo)
    {
        if (id != prestamo.Id)
        {
            return BadRequest("El ID del préstamo no coincide.");
        }

        if (!ModelState.IsValid) // Validar el modelo
        {
            return BadRequest(ModelState);
        }

        _context.Entry(prestamo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PrestamoExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: api/prestamos/{id}
    [Authorize(Roles = "1")] // Solo administradores pueden eliminar préstamos
    [HttpDelete("Cancelar Prestamo{id}")]
    public async Task<IActionResult> EliminarPrestamo(int id)
    {
        var prestamo = await _context.Prestamos.FindAsync(id);
        if (prestamo == null)
        {
            return NotFound();
        }

        _context.Prestamos.Remove(prestamo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PrestamoExists(int id)
    {
        return _context.Prestamos.Any(e => e.Id == id);
    }
}
