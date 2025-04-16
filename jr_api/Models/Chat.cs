using jr_api.Models;

public class Chat
{
    public int Id { get; set; }
    public int UsuarioOrigenId { get; set; }
    public int UsuarioDestinoId { get; set; }

    public Usuario UsuarioOrigen { get; set; }
    public Usuario UsuarioDestino { get; set; }

    public ICollection<Mensaje> Mensajes { get; set; }
}