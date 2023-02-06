using System;

namespace LS_ERP.Mobile.Models
{
    public class NavigationPermissionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
    }
}