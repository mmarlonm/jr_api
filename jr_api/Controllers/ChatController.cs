using jr_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext; //para real time chat

    public ChatController(ApplicationDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    // GET: api/Chat/chats/{usuarioId}
    [HttpGet("chats/{usuarioId}")]
    public async Task<IActionResult> GetChats(int usuarioId)
    {
        var chats = await _context.Chats
            .Include(c => c.Mensajes)
            .Include(c => c.UsuarioOrigen)
            .Include(c => c.UsuarioDestino)
            .Where(c => c.UsuarioOrigenId == usuarioId || c.UsuarioDestinoId == usuarioId)
            .ToListAsync();

        var result = chats.Select(chat =>
        {
            var contacto = chat.UsuarioOrigenId == usuarioId ? chat.UsuarioDestino : chat.UsuarioOrigen;

            string avatarBase64 = contacto.Avatar != null
            ? $"data:image/png;base64,{Convert.ToBase64String(contacto.Avatar)}"
            : null;
            var mensajes = chat.Mensajes
                .OrderBy(m => m.Fecha)
                .Select(m => new MensajeDto
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    ContactId = m.RemitenteId,
                    IsMine = m.RemitenteId == usuarioId,
                    Value = m.Contenido,
                    CreatedAt = m.Fecha.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

            return new ChatDto
            {
                Id = chat.Id,
                ContactId = contacto.UsuarioId,
                Contact = new ContactDto
                {
                    Id = contacto.UsuarioId,
                    NombreUsuario = contacto.NombreUsuario,
                    Avatar = avatarBase64
                    // Puedes agregar Avatar, Estado, etc.
                },
                UnreadCount = mensajes.Count(m => !m.IsMine && !chat.Mensajes.First(x => x.Id == m.Id).Leido),
                Muted = false, // Ajusta si tienes esta lógica
                LastMessage = mensajes.LastOrDefault()?.Value,
                LastMessageAt = mensajes.LastOrDefault()?.CreatedAt,
                Messages = mensajes
            };
        }).ToList();

        return Ok(result);
    }

    // POST: api/Chat/enviar
    [HttpPost("enviar")]
    public async Task<IActionResult> EnviarMensaje([FromBody] EnviarMensajeDto dto)
    {
        // Ver si ya existe el chat
        var chat = await _context.Chats.FirstOrDefaultAsync(c =>
            (c.UsuarioOrigenId == dto.RemitenteId && c.UsuarioDestinoId == dto.DestinatarioId) ||
            (c.UsuarioOrigenId == dto.DestinatarioId && c.UsuarioDestinoId == dto.RemitenteId));

        // Si no existe, lo creamos
        if (chat == null)
        {
            chat = new Chat
            {
                UsuarioOrigenId = dto.RemitenteId,
                UsuarioDestinoId = dto.DestinatarioId
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync(); // Para generar el ID del nuevo chat
        }

        // Crear mensaje
        var mensaje = new Mensaje
        {
            ChatId = chat.Id,
            RemitenteId = dto.RemitenteId,
            Contenido = dto.Contenido,
            Fecha = DateTime.Now,
            Leido = false
        };

        _context.Mensajes.Add(mensaje);
        await _context.SaveChangesAsync();

        await _hubContext.Clients
        .Group(chat.Id.ToString())
        .SendAsync("MensajeRecibido", new
        {
            Id = mensaje.Id,
            ChatId = mensaje.ChatId,
            ContactId = mensaje.RemitenteId,
            IsMine = false,
            Value = mensaje.Contenido,
            CreatedAt = mensaje.Fecha.ToString("yyyy-MM-dd HH:mm:ss")
        });

        // Retornar mensaje como respuesta
        return Ok(new
        {
            Id = mensaje.Id,
            ChatId = mensaje.ChatId,
            ContactId = mensaje.RemitenteId,
            IsMine = true,
            Value = mensaje.Contenido,
            CreatedAt = mensaje.Fecha.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    [HttpGet("obtener/{chatId}/{usuarioActualId}")]
    public async Task<IActionResult> ObtenerChat(int chatId, int usuarioActualId)
    {
        var chat = await _context.Chats
            .Include(c => c.Mensajes)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            return NotFound("Chat no encontrado");

        var contactoId = chat.UsuarioOrigenId == usuarioActualId
            ? chat.UsuarioDestinoId
            : chat.UsuarioOrigenId;

        var contacto = await _context.Usuarios
    .Where(u => u.UsuarioId == contactoId)
    .Select(u => new
    {
        id = u.UsuarioId,
        name = u.NombreUsuario,
        avatar = u.Avatar != null
            ? $"data:image/jpeg;base64,{Convert.ToBase64String(u.Avatar)}" // Ajusta el tipo de imagen (jpeg, png, etc.)
            : null // Si no hay avatar, puedes enviar null o un valor predeterminado.
    })
    .FirstOrDefaultAsync();

        var mensajes = await _context.Mensajes
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Fecha)
            .Select(m => new
            {
                id = m.Id.ToString(),
                chatId = m.ChatId.ToString(),
                contactId = m.RemitenteId.ToString(),
                isMine = m.RemitenteId == usuarioActualId, // CORRECTO AHORA
                value = m.Contenido,
                createdAt = m.Fecha.ToString("o")
            })
            .ToListAsync();

        var chatDto = new
        {
            id = chat.Id.ToString(),
            contactId = contacto.id.ToString(),
            contact = contacto,
            unreadCount = 0,
            muted = false,
            lastMessage = mensajes.LastOrDefault()?.value,
            lastMessageAt = mensajes.LastOrDefault()?.createdAt,
            messages = mensajes
        };

        return Ok(chatDto);
    }
}