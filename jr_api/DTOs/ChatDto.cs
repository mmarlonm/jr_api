public class ChatDto
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public ContactDto Contact { get; set; }
    public int UnreadCount { get; set; }
    public bool Muted { get; set; }
    public string LastMessage { get; set; }
    public string LastMessageAt { get; set; }
    public List<MensajeDto> Messages { get; set; }
}