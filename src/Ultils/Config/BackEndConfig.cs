using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Config
{
    public class BackEndConfig
    {
        public static string ConfigName = "BackEnd";
        public string HostErp { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public string HostHeplDesk { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string GetDailyTargetByCenterID { get; set; } = string.Empty;
        public string GetDailyTargetOverview { get; set; } = string.Empty;
        public string GetDailyTargetByListWorkCenterID { get; set; } = string.Empty;
        public string GetAllWorkCenter { get; set; } = string.Empty;
        public string GetJobOutputSummaryByDate { get; set; } = string.Empty;
        public string GetDepartment { get; set; } = string.Empty;
        public string GetCompany { get; set; } = string.Empty;
        public string GetDailyTargetDetailByDate { get; set; } = string.Empty;
        public string GetSewingWorkCenter { get; set; } = string.Empty;
        public string GetUserByUserName { get; set; } = string.Empty;
        public string GetNavigationPermission { get; set; } = string.Empty;

    }
}
