namespace OJTEDU.Api.Input.CommonControllers
{
    public class MessageController
    {
        public class CreateFirstMessageConversationInput
        {
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
        }

        public class CreateMessageInput
        {
            public int? ConversationId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
        }
    }
}
