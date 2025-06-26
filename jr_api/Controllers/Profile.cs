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

    [HttpGet("me/{id}")]
    public IActionResult GetProfile(int id)
    {
        try
        {

            // Buscar al usuario en la base de datos
            var usuario = _context.Usuarios
                .Where(u => u.UsuarioId == id)
                .Select(u => new
                {
                    u.UsuarioId,
                    u.NombreUsuario,
                    u.Email,
                    u.Telefono,
                    u.Activo,
                    Avatar = u.Avatar != null ? Convert.ToBase64String(u.Avatar) : null, // Convertir avatar a Base64
                    UsuarioInformacion = u.UsuarioInformacion != null ? new
                    {
                        u.UsuarioInformacion.UsuarioInformacionId,
                        u.UsuarioInformacion.Sexo,
                        u.UsuarioInformacion.FechaNacimiento,
                        u.UsuarioInformacion.NumeroContacto1,
                        u.UsuarioInformacion.NumeroContacto2,
                        u.UsuarioInformacion.NombreContacto1,
                        u.UsuarioInformacion.NombreContacto2,
                        u.UsuarioInformacion.Parentesco1,
                        u.UsuarioInformacion.Parentesco2,
                        u.UsuarioInformacion.Direccion
                    } : null
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
    [Consumes("multipart/form-data")]
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
        var UsuarioInformacion = new UsuarioInformacion();
        var _UsuarioInformacion = await _context.UsuarioInformacion.FindAsync(request.UsuarioInformacion.UsuarioInformacionId);
        if (_UsuarioInformacion != null)
        {
            UsuarioInformacion = _UsuarioInformacion;
        }

        if (usuario == null)
            return NotFound("Usuario no encontrado.");

        usuario.NombreUsuario = request.NombreUsuario ?? usuario.NombreUsuario;
        usuario.Email = request.Email ?? usuario.Email;
        usuario.Telefono = request.Telefono ?? usuario.Telefono;
        usuario.Activo = request.Activo ?? usuario.Activo;

        if(request.UsuarioInformacion.UsuarioInformacionId == 0) {
            UsuarioInformacion.UsuarioId = request.id;
            UsuarioInformacion.Sexo = request.UsuarioInformacion.Sexo;
            UsuarioInformacion.FechaNacimiento = request.UsuarioInformacion.FechaNacimiento;
            UsuarioInformacion.NumeroContacto1 = request.UsuarioInformacion.NumeroContacto1;
            UsuarioInformacion.NumeroContacto2 = request.UsuarioInformacion.NumeroContacto2;
            UsuarioInformacion.NombreContacto1 = request.UsuarioInformacion.NombreContacto1;
            UsuarioInformacion.NombreContacto2 = request.UsuarioInformacion.NombreContacto2;
            UsuarioInformacion.Parentesco1 = request.UsuarioInformacion.Parentesco1;
            UsuarioInformacion.Parentesco2 = request.UsuarioInformacion.Parentesco2;
            UsuarioInformacion.Direccion = request.UsuarioInformacion.Direccion;
            _context.UsuarioInformacion.Add(UsuarioInformacion);
        }
        else
        {
            UsuarioInformacion.UsuarioId = usuario.UsuarioId;
            UsuarioInformacion.Sexo = request.UsuarioInformacion.Sexo;
            UsuarioInformacion.FechaNacimiento = request.UsuarioInformacion.FechaNacimiento;
            UsuarioInformacion.NumeroContacto1 = request.UsuarioInformacion.NumeroContacto1;
            UsuarioInformacion.NumeroContacto2 = request.UsuarioInformacion.NumeroContacto2;
            UsuarioInformacion.NombreContacto1 = request.UsuarioInformacion.NombreContacto1;
            UsuarioInformacion.NombreContacto2 = request.UsuarioInformacion.NombreContacto2;
            UsuarioInformacion.Parentesco1 = request.UsuarioInformacion.Parentesco1;
            UsuarioInformacion.Parentesco2 = request.UsuarioInformacion.Parentesco2;
            UsuarioInformacion.Direccion = request.UsuarioInformacion.Direccion;
            _context.UsuarioInformacion.Update(UsuarioInformacion);
        }
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
                u.Telefono,
                u.Activo,
                Avatar = u.Avatar != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(u.Avatar)}"
                    : null,
                RolId = _context.UsuarioRoles
                    .Where(ur => ur.UsuarioId == u.UsuarioId)
                    .Select(ur => ur.RolId)
                    .FirstOrDefault(), // 🔹 Obtener el primer rol del usuario (si tiene varios, podrías manejarlo de otra manera)
                NombreRol = (from ur in _context.UsuarioRoles
                             join r in _context.Roles on ur.RolId equals r.RolId
                             where ur.UsuarioId == u.UsuarioId
                             select r.NombreRol).FirstOrDefault(),

                // Incluir los proyectos creados por este usuario
                ProyectosCreados = _context.Proyectos
                    .Where(p => p.UsuarioId == u.UsuarioId)
                    .Select(p => new
                    {
                        p.ProyectoId,
                        p.Nombre,
                        EstatusNombre = _context.EstatusProyecto
                        .Where(ep => ep.Id == p.Estatus)
                        .Select(ep => ep.Nombre) // Nombre del estatus
                        .FirstOrDefault(),
                        p.FechaCreacion

                    })
                    .ToList()
            })
            .ToListAsync();

        return Ok(usuarios);
    }

    // POST api/usuario/crear
    [HttpPost("created-user")]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioRequest request)
    {
        if (request == null)
        {
            return BadRequest("Datos inválidos.");
        }

        // Convertir la imagen Base64 a byte[]
        byte[] avatarBytes = null;
        if (!string.IsNullOrEmpty(request.avatar))
        {
            try
            {
                string base64String = request.avatar.Contains(",")
                    ? request.avatar.Split(',')[1]
                    : request.avatar;

                avatarBytes = Convert.FromBase64String(base64String);
            }
            catch (FormatException)
            {
                return BadRequest("Formato de imagen inválido.");
            }
        }

        // 📌 Verificar si es un usuario nuevo o si estamos actualizando
        if (request.usuarioId == 0)
        {
            // 1️⃣ Verificar si el correo ya está registrado
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.email);
            if (usuarioExistente != null)
            {
                return Conflict("El correo electrónico ya está registrado.");
            }

            // 2️⃣ Generar una contraseña por defecto "123456" con hashing
            var contrasenaPorDefecto = "123456";
            var contrasenaHash = BCrypt.Net.BCrypt.HashPassword(contrasenaPorDefecto);

            // 3️⃣ Crear el usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = request.nombreUsuario,
                Email = request.email,
                Avatar = avatarBytes,
                Telefono = request.telefono,
                Activo = request.activo,
                ContraseñaHash = contrasenaHash,
                ContraseñaSalt = Guid.NewGuid().ToString()
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync(); // Guardamos para generar el ID

            // 4️⃣ Asignar el rol al usuario
            var usuarioRol = new UsuarioRol
            {
                UsuarioId = nuevoUsuario.UsuarioId, // Se usa el ID generado
                RolId = request.rolId
            };

            _context.UsuarioRoles.Add(usuarioRol);
            await _context.SaveChangesAsync(); // Guardamos la relación usuario-rol

            return Ok(new { UsuarioId = nuevoUsuario.UsuarioId, NombreUsuario = nuevoUsuario.NombreUsuario });
        }
        else
        {
            // 1️⃣ Buscar usuario existente por ID
            var usuarioExistente = await _context.Usuarios.FindAsync(request.usuarioId);
            if (usuarioExistente == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // 2️⃣ Actualizar los datos del usuario
            usuarioExistente.NombreUsuario = request.nombreUsuario;
            usuarioExistente.Email = request.email;
            usuarioExistente.Telefono = request.telefono;
            usuarioExistente.Activo = request.activo;
            if (avatarBytes != null) usuarioExistente.Avatar = avatarBytes;

            _context.Usuarios.Update(usuarioExistente);
            await _context.SaveChangesAsync();

            // 3️⃣ Buscar relación usuario-rol existente
            var usuarioRolExistente = await _context.UsuarioRoles
                .FirstOrDefaultAsync(ur => ur.UsuarioId == usuarioExistente.UsuarioId);

            if (usuarioRolExistente != null)
            {
                // ⚠️ Para cambiar el rol, primero eliminamos la relación anterior
                _context.UsuarioRoles.Remove(usuarioRolExistente);
                await _context.SaveChangesAsync();

                // 🔹 Ahora asignamos el nuevo rol
                _context.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = usuarioExistente.UsuarioId,
                    RolId = request.rolId
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                // Si no tenía rol, simplemente lo asignamos
                _context.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioId = usuarioExistente.UsuarioId,
                    RolId = request.rolId
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { UsuarioId = usuarioExistente.UsuarioId, NombreUsuario = usuarioExistente.NombreUsuario });
        }
    }
}

    public class AvatarUpdateRequest
{
    public int usuarioId { get; set; }
    public string foto { get; set; }
}

public class UserUpdateRequest
{
    public int UsuarioInformacionId { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public bool? Activo { get; set; }
    public int id { get; set; }
    public UsuarioInformacion UsuarioInformacion { get; set; }

}


public class PasswordUpdateRequest
{
    public string currentPassword { get; set; }
    public string newPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public int UsuarioId { get; set; }
}

public class CrearUsuarioRequest
{
    public string nombreUsuario { get; set; }
    public string email { get; set; }
    public string telefono { get; set; }
    public bool activo { get; set; }
    public string avatar { get; set; }
    public int rolId { get; set; }  // El rol a asignar
    public int usuarioId { get; set; }  // El rol a asignar
}