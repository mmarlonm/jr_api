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
        var usuario = _context.Usuarios.SingleOrDefault(u => u.NombreUsuario == request.Username);
        if (usuario == null) return Unauthorized("Usuario o contraseña incorrectos.");

        // Verificar contraseña usando bcrypt
        if (!VerificarContraseña(request.Password, usuario.ContraseñaHash))
            return Unauthorized("Usuario o contraseña incorrectos.");

        // Convertir avatar a Base64 con prefijo para <img src="">
        string avatarBase64 = usuario.Avatar != null
            ? $"data:image/png;base64,{Convert.ToBase64String(usuario.Avatar)}"
            : null;

        // Obtener roles asociados al usuario
        var roles = _context.UsuarioRoles
            .Where(ur => ur.UsuarioId == usuario.UsuarioId)
            .Join(_context.Roles, ur => ur.RolId, r => r.RolId, (ur, r) => r.NombreRol)
            .ToList();

        // Obtener permisos asociados a los roles del usuario
        var permisos = _context.RolPermisos
            .Where(rp => roles.Contains(rp.Rol.NombreRol))
            .Join(_context.Permisos, rp => rp.PermisoId, p => p.PermisoId, (rp, p) => p.DescripcionPermiso)
            .ToList();

        // Crear el token con roles y permisos como claims
        var token = GenerarToken(usuario, roles, permisos);

        // Devolver token, roles y permisos
        return Ok(new
        {
            Token = token,
            Roles = roles,
            Permisos = permisos,
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