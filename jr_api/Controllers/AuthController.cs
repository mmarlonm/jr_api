using jr_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using System.Linq;  // Asegúrate de tener este espacio de nombres para LINQ
using Org.BouncyCastle.Asn1.Cms;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var usuario = _context.Usuarios.SingleOrDefault(u => u.Email == request.Username);
        if (usuario == null)
            return Unauthorized("Usuario o contraseña incorrectos.");

        if (!usuario.Activo)
            return Unauthorized("El usuario está inactivo. Contacta al administrador.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.ContraseñaHash))
            return Unauthorized("Usuario o contraseña incorrectos.");

        string avatarBase64 = usuario.Avatar != null
            ? $"data:image/png;base64,{Convert.ToBase64String(usuario.Avatar)}"
            : null;

        var roles = _context.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuario.UsuarioId)
            .Select(ur => ur.Rol)
            .ToList();

        var permisos = _context.RolPermisos
            .Where(rp => roles.Select(r => r.RolId).Contains(rp.RolId))
            .Select(rp => new
            {
                rp.Permiso.PermisoId,
                rp.Permiso.DescripcionPermiso,
                rp.Permiso.Codigo,
                Vista = rp.Vista != null ? new
                {
                    rp.Vista.VistaId,
                    rp.Vista.NombreVista,
                    rp.Vista.Ruta
                } : null
            })
            .ToList();

        var vistas = _context.RolVistas
            .Where(rv => roles.Select(r => r.RolId).Contains(rv.RolId))
            .Select(rv => new
            {
                rv.Vista.VistaId,
                rv.Vista.NombreVista,
                rv.Vista.Ruta
            })
            .Distinct()
            .ToList();

        var token = GenerarToken(usuario, roles.Select(r => r.NombreRol).ToList(), permisos.Select(p => p.DescripcionPermiso).ToList());

        // Obtener IP del cliente
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        try
        {
            _context.LoginLogs.Add(new LoginLogs
            {
                UsuarioId = usuario.UsuarioId,
                IP = request.Metadata?.Ip ?? "Desconocida",
                Navegador = request.Metadata?.Navegador ?? "Desconocido",
                SistemaOperativo = request.Metadata?.SistemaOperativo ?? "Desconocido",
                Dispositivo = request.Metadata?.Dispositivo ?? "Desconocido",
                Ubicacion = request.Metadata.Ubicacion ?? "Desconocido",
                Mensaje = "Inicio de sesión exitoso",
                Exitoso = true
            });

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Puedes logear internamente el error si quieres, pero no interrumpas el login
            Console.WriteLine($"Error al guardar log de login: {ex.InnerException?.Message ?? ex.Message}");
        }

        return Ok(new
        {
            Token = token,
            Roles = roles.Select(r => r.NombreRol).ToList(),
            Permisos = permisos,
            Vistas = vistas,
            Usuario = new
            {
                Id = usuario.UsuarioId,
                usuario.NombreUsuario,
                usuario.Email,
                Avatar = avatarBase64
            }
        });
    }

    private string GenerarToken(Usuario usuario, List<string> roles, List<string> permisos)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiresInMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerificarContraseña(string contraseña, string hashAlmacenado)
    {
        // Verificar la contraseña utilizando bcrypt
        return BCrypt.Net.BCrypt.Verify(contraseña, hashAlmacenado);
    }

    [Authorize]
    [HttpGet("debug-token")]
    public IActionResult DebugToken()
    {
        return Ok(new
        {
            Usuario = User.Identity?.Name,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    [HttpGet("conectados")]
    public IActionResult GetConectados()
    {
        var usuarios = PresenceHub.GetConnectedUsersWithStatus();
        return Ok(usuarios);
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public MetadataRequest Metadata { get; set; }
}

public class MetadataRequest
{
    public string Ip { get; set; }
    public string Ubicacion { get; set; }
    public string Navegador { get; set; }
    public string SistemaOperativo { get; set; }
    public string Dispositivo { get; set; }
}