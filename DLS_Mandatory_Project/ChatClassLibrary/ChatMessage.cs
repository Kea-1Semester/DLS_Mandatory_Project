namespace ChatClassLibrary
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public required string UserId { get; set; }
        public required string Message { get; set; }
    }
}
