namespace jr_api.DTOs
{
    public class UsuarioInformacionDTo
    {
        public int UsuarioInformacioId { get; set; }

        public int UsuarioId { get; set; }
        public int Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? NumeroContacto1 { get; set; }
        public int? NumeroContacto2 { get; set; }
        public string? NombreContacto1 { get; set; }
        public string? NombreContacto2 { get; set; }
        public string? Parentesco1 { get; set; }
        public string? Parentesco2 { get; set; }
        public string? Direccion { get; set; }
        
    }
}
