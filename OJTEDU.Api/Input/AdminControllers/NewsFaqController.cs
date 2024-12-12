namespace OJTEDU.Api.Input.AdminControllers
{
    public class NewsFaqController
    {
        public class AddParentNewsRequestForAdmin
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentNewsRequestForAdmin
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentNewsStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        public class AddChildNewsRequestForAdmin
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public int? ParentNewsId { get; set; }
        }

        public class UpdateChildNewsRequestForAdmin
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildNewscontent { get; set; }
        }

        public class UpdateChildNewsStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        public class AddParentFaqRequestForAdmin
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentFaqRequestForAdmin
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentFaqStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }

        public class AddChildFaqRequestForAdmin
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public int? ParentFaqId { get; set; }
        }

        public class UpdateChildFaqRequestForAdmin
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
        }

        public class UpdateChildFaqStatusRequestForAdmin
        {
            public string? Status { get; set; }
        }
    }
}
