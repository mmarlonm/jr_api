using jr_api.Models;

public class Mensaje
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int RemitenteId { get; set; }
    public string Contenido { get; set; }
    public DateTime Fecha { get; set; }
    public bool Leido { get; set; }

    public Chat Chat { get; set; }
    public Usuario Remitente { get; set; }
}