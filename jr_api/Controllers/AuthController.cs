using jr_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using System.Linq;  // Asegúrate de tener este espacio de nombres para LINQ

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
        // Buscar usuario por nombre de usuario
        var usuario = _context.Usuarios.SingleOrDefault(u => u.Email == request.Username);
        if (usuario == null)
            return Unauthorized("Usuario o contraseña incorrectos.");

        // Verificar si el usuario está activo
        if (!usuario.Activo)
            return Unauthorized("El usuario está inactivo. Contacta al administrador.");

        // Verificar la contraseña usando bcrypt
        if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.ContraseñaHash))
            return Unauthorized("Usuario o contraseña incorrectos.");

        // Convertir avatar a Base64 con prefijo para <img src="">
        string avatarBase64 = usuario.Avatar != null
            ? $"data:image/png;base64,{Convert.ToBase64String(usuario.Avatar)}"
            : null;

        // Obtener los roles asociados al usuario
        var roles = _context.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuario.UsuarioId)
            .Select(ur => ur.Rol)
            .ToList();

        // Obtener los permisos y vistas asociadas a los roles
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

        // Obtener vistas permitidas
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

        // Generar el token con la información del usuario
        var token = GenerarToken(usuario, roles.Select(r => r.NombreRol).ToList(), permisos.Select(p => p.DescripcionPermiso).ToList());

        // Devolver el token junto con la información del usuario, roles, permisos y vistas
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
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        // Agregar roles como claims
        claims.AddRange(roles.Select(rol => new Claim(ClaimTypes.Role, rol)));

        // Agregar permisos como claims
        claims.AddRange(permisos.Select(per => new Claim("Permiso", per)));

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
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}