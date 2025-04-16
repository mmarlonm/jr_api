public class MensajeDto
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public int ContactId { get; set; }
    public bool IsMine { get; set; }
    public string Value { get; set; }
    public string CreatedAt { get; set; }
}