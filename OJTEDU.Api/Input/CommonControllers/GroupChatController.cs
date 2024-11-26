namespace OJTEDU.Api.Input.CommonControllers
{
    public class GroupChatController
    {
        public class CreateGroupChatInput
        {
            public string? GroupName { get; set; }
        }

        public class UpdateGroupChatInput
        {
            public string? GroupName { get; set; }
        }
    }
}
