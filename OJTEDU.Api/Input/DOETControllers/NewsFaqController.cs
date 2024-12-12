namespace OJTEDU.Api.Input.DOETControllers
{
    public class NewsFaqController
    {
        public class AddParentNewsRequestForDoet
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentNewsRequestForDoet
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentNewsStatusRequestForDoet
        {
            public string? Status { get; set; }
        }

        public class AddChildNewsRequestForDoet
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildNewscontent { get; set; }
            public int? ParentNewsId { get; set; }
        }

        public class UpdateChildNewsRequestForDoet
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildNewscontent { get; set; }
        }

        public class UpdateChildNewsStatusRequestForDoet
        {
            public string? Status { get; set; }
        }

        public class AddParentFaqRequestForDoet
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentFaqRequestForDoet
        {
            public string? Title { get; set; }
            public string ForRoleIds { get; set; }
        }

        public class UpdateParentFaqStatusRequestForDoet
        {
            public string? Status { get; set; }
        }

        public class AddChildFaqRequestForDoet
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
            public int? ParentFaqId { get; set; }
        }

        public class UpdateChildFaqRequestForDoet
        {
            public string? Title { get; set; }
            public IFormFile? Image { get; set; }
            public string? ChildFaqcontent { get; set; }
        }

        public class UpdateChildFaqStatusRequestForDoet
        {
            public string? Status { get; set; }
        }
    }
}
