using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public override Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"Usuario conectado: {userId}");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task EnviarMensaje(string chatId, object mensaje)
    {
        await Clients.Group(chatId).SendAsync("MensajeRecibido", mensaje);
    }

    public async Task UnirseAlChat(int chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task SalirDelChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }
    public async Task EnviarMensajeAlGrupo(string grupoId, object mensaje)
    {
        await Clients.Group(grupoId).SendAsync("RecibirMensaje", mensaje);
    }
}