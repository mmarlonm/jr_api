public class LoginLogs
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string IP { get; set; }
    public string Navegador { get; set; }
    public string SistemaOperativo { get; set; }
    public string Dispositivo { get; set; }
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; }
    public string Ubicacion { get; set; }
}
