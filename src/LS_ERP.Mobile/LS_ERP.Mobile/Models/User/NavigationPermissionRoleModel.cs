using System;

namespace LS_ERP.Mobile.Models
{
    public class NavigationPermissionRoleModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }  
        public string Image { get; set; }
    }
}