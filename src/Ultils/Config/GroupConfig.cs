using System.Collections.Generic;

namespace Ultils.Config
{

    public class GroupConfig
    {
        public static string ConfigName = "Group";
        public List<GroupModel> Groups { get; set; } = new List<GroupModel>();
    }
    public class GroupModel
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
