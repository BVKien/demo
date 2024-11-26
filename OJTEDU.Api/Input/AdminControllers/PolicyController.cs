namespace OJTEDU.Api.Input.AdminControllers
{
    public class PolicyController
    {
        public class AddParentPolicyRequestForAdmin
        {
            public string? ParentPolicycontent { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentPolicyRequestForAdmin
        {
            public string? ParentPolicycontent { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentPolicyStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        public class AddChildPolicyRequestForAdmin
        {
            public string? ChildPolicycontent { get; set; }
            public int? ParentPolicyId { get; set; }
        }

        public class UpdateChildPolicyRequestForAdmin
        {
            public string? ChildPolicycontent { get; set; }
        }

        public class UpdateChildPolicyStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
