using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
    }
}
