using System;
using System.Collections.Generic;

namespace AuthService.Models
{
    public partial class RoleMaster
    {
        public RoleMaster()
        {
            UserTables = new HashSet<UserTable>();
        }

        public int RoleId { get; set; }
        public string? RoleName { get; set; }

        public virtual ICollection<UserTable> UserTables { get; set; }
    }
}
