using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Kanban.Models
{
    public class NavigationPermissionModel
    {
        public int Id { get; set; }
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
        public bool IsActive { get; set; }
        public RoleModel? Role { get; set; }
        //public UserModel? User { get; set; }
    }
}
