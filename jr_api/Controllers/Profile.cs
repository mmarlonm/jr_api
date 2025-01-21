using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using jr_api.Models;  // Asegúrate de importar el contexto de tu base de datos
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("me")]
    public IActionResult GetProfile()
    {
        try
        {
            // Obtener el ID del usuario autenticado desde el token JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Usuario no autenticado" });

            int userId = int.Parse(userIdClaim.Value);

            // Buscar al usuario en la base de datos
            var usuario = _context.Usuarios
                .Where(u => u.UsuarioId == userId)
                .Select(u => new
                {
                    u.UsuarioId,
                    u.NombreUsuario,
                    u.Email,
                    Avatar = u.Avatar != null ? Convert.ToBase64String(u.Avatar) : null // Convertir avatar a Base64
                })
                .FirstOrDefault();

            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpPost("upload-avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile foto, [FromForm] int usuarioId)
    {
        if (foto == null || foto.Length == 0)
            return BadRequest("No se ha proporcionado una imagen válida.");

        try
        {
            using var memoryStream = new MemoryStream();
            await foto.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray(); // Convertir a byte[]

            // Buscar usuario en la BD
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            // Guardar avatar en la BD
            usuario.Avatar = imageBytes;  // Asegúrate de que Avatar es VARBINARY(MAX) en la BD
            await _context.SaveChangesAsync();

            // Convertir avatar a Base64 con prefijo para <img src="">
            string avatarBase64 = usuario.Avatar != null
                ? $"data:image/png;base64,{Convert.ToBase64String(usuario.Avatar)}"
                : null;

            return Ok(new { avatarBase64 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno: " + ex.Message);
        }
    }

    [HttpPut("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateRequest request)
    {
        var usuario = await _context.Usuarios.FindAsync(request.id);

        if (usuario == null)
            return NotFound("Usuario no encontrado.");

        usuario.NombreUsuario = request.NombreUsuario ?? usuario.NombreUsuario;
        usuario.Email = request.Email ?? usuario.Email;

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Perfil actualizado correctamente." });
    }

    // 📌 2️⃣ CAMBIAR CONTRASEÑA
    [HttpPost("change-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] PasswordUpdateRequest request)
    {
        if (request.newPassword != request.ConfirmPassword)
            return BadRequest("La nueva contraseña y la confirmación no coinciden.");

        var usuario = await _context.Usuarios.FindAsync(request.UsuarioId);
        if (usuario == null)
            return NotFound("Usuario no encontrado.");

        // Validar la contraseña actual con BCrypt
        if (!BCrypt.Net.BCrypt.Verify(request.currentPassword, usuario.ContraseñaHash))
            return Unauthorized("Contraseña actual incorrecta.");

        // Generar nuevo hash de la contraseña
        var nuevoHash = BCrypt.Net.BCrypt.HashPassword(request.newPassword);

        usuario.ContraseñaHash = nuevoHash;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Contraseña actualizada correctamente." });
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var usuarios = await _context.Usuarios
            .Select(u => new
            {
                u.UsuarioId,
                u.NombreUsuario,
                u.Email,
                Avatar = u.Avatar != null
                ? $"data:image/png;base64,{Convert.ToBase64String(u.Avatar)}"
                : null
             })
            .ToListAsync();

        return Ok(usuarios);
    }
}

    public class AvatarUpdateRequest
{
    public int usuarioId { get; set; }
    public string foto { get; set; }
}

public class UserUpdateRequest
{
    public string? NombreUsuario { get; set; }
    public string? Email { get; set; }
    public int id { get; set; }
}

public class PasswordUpdateRequest
{
    public string currentPassword { get; set; }
    public string newPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public int UsuarioId { get; set; }
}