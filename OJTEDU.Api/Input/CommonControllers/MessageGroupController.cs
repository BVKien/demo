namespace OJTEDU.Api.Input.CommonControllers
{
    public class MessageGroupController
    {
        public class CreateMemberGroupMessageInput
        {
            public int? GroupChatId { get; set; }
        }

        public class CreateMessagesInGroupInput
        {
            public int? GroupChatId { get; set; }
            public string? MessageContent { get; set; }
            public string? MessageFile { get; set; }
            public string? Image { get; set; }
        }
    }
}
