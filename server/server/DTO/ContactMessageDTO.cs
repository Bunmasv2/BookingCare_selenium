namespace server.DTO
{
    public class ContactMessagesDTO
    {
        public class ContactMessages
        {
            public int Id { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; }
            public string Messages { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Status { get; set; }
        }
        public class ReplyDto
        {
            public string Message { get; set; }
        }
    }
}